using UnityEngine;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

[Serializable]
public class MessageData
{
    public string type;
    public string status;
    public string msg;
    public string username;
    public string email;
    public string password;
    public string query;
    public string target_user;
    public string requester;
    public string action; // "accept" or "reject"
    public string room;
    public string[] friends;
    public string[] requests;

    // 👇 για chat
    public string sender;
}

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance;

    private ClientWebSocket _ws;
    private CancellationTokenSource _cts;
    private string _serverUrl = "ws://localhost:8765";

    // --- STATE ---
    public string CurrentUsername = "";
    public List<string> Friends = new List<string>();
    public List<string> PendingRequests = new List<string>();

    // Σε ποιον "peer" μιλάω τώρα (αν θες DM λογική)
    public string CurrentChatPeer = "";

    // --- EVENTS (UI, game κλπ θα κάνουν subscribe εδώ) ---
    public Action<string> OnLoginSuccess;
    public Action<string> OnLoginFailed;
    public Action<string> OnRegisterSuccess;
    public Action<string> OnRegisterFailed;
    public Action<string> OnSearchFound;
    public Action OnSearchNotFound;
    public Action OnListsUpdated;

    // Chat: (fromUser, text)
    public event Action<string, string> OnChatMessageReceived;

    // Queue για να φέρνουμε πράγματα στο main thread
    private readonly Queue<Action> _executionQueue = new Queue<Action>();

    //GestureEvent
    public event Action<string> OnGestureReceived;

    // ------------------------------------------------
    //  Unity lifecycle
    // ------------------------------------------------
    private void Awake()
    {
        // Fix για το DontDestroyOnLoad error
        if (transform.parent != null)
            transform.SetParent(null);

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private async void Start()
    {
        await Connect();
    }

    private void Update()
    {
        lock (_executionQueue)
        {
            while (_executionQueue.Count > 0)
                _executionQueue.Dequeue().Invoke();
        }
    }

    private void OnDestroy()
    {
        if (_cts != null) _cts.Cancel();
        if (_ws != null) _ws.Dispose();
    }

    // ------------------------------------------------
    //  WebSocket Connect / Receive / Send
    // ------------------------------------------------
    private async Task Connect()
    {
        _ws = new ClientWebSocket();
        _cts = new CancellationTokenSource();

        try
        {
            await _ws.ConnectAsync(new Uri(_serverUrl), _cts.Token);
            Debug.Log("<color=green>[Network] Connected to server</color>");
            ReceiveLoop();
        }
        catch (Exception e)
        {
            Debug.LogError("[Network] Connection Error: " + e.Message);
        }
    }

    private async void ReceiveLoop()
    {
        var buffer = new byte[8192];

        try
        {
            while (_ws.State == WebSocketState.Open && !_cts.IsCancellationRequested)
            {
                var result = await _ws.ReceiveAsync(new ArraySegment<byte>(buffer), _cts.Token);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    string json = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    lock (_executionQueue)
                    {
                        _executionQueue.Enqueue(() => HandleMessage(json));
                    }
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    Debug.LogWarning("[Network] Server closed connection.");
                    break;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("[Network] ReceiveLoop Exception: " + e.Message);
        }
    }

    private async void Send(MessageData data)
    {
        if (_ws == null || _ws.State != WebSocketState.Open)
        {
            Debug.LogWarning("[Network] Tried to send, but WebSocket is not open.");
            return;
        }

        try
        {
            string json = JsonUtility.ToJson(data);
            byte[] buffer = Encoding.UTF8.GetBytes(json);
            await _ws.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, _cts.Token);
            // Debug.Log("[Network] Sent: " + json);
        }
        catch (Exception e)
        {
            Debug.LogWarning("[Network] Send Exception: " + e.Message);
        }
    }

    // ------------------------------------------------
    //  Handle incoming messages
    // ------------------------------------------------
    private void HandleMessage(string json)
    {
        Debug.Log("<color=yellow>[Network] Server: " + json + "</color>");

        try
        {
            MessageData data = JsonUtility.FromJson<MessageData>(json);
            if (data == null || string.IsNullOrEmpty(data.type))
            {
                Debug.LogWarning("[Network] Invalid JSON or missing type.");
                return;
            }

            switch (data.type)
            {
                // -------- LOGIN RESPONSE --------
                case "login_response":
                    if (data.status == "success")
                    {
                        CurrentUsername = data.username;
                        UpdateLocalLists(data);
                        OnLoginSuccess?.Invoke(data.username);

                        // Μπαίνουμε σε ένα room (π.χ. "main") για chat/broadcast
                        JoinRoom("main");
                    }
                    
                    else
                    {
                        OnLoginFailed?.Invoke(data.msg);
                    }
                    break;

                // -------- REGISTER RESPONSE --------
                case "register_response":
                    if (data.status == "success")
                    {
                        OnRegisterSuccess?.Invoke(data.username);
                    }
                    else
                    {
                        OnRegisterFailed?.Invoke(data.msg);
                    }
                    break;

                // -------- SEARCH USER RESPONSE --------
                case "search_response":
                    if (data.status == "found")
                    {
                        OnSearchFound?.Invoke(data.username);
                    }
                    else
                    {
                        OnSearchNotFound?.Invoke();
                    }
                    break;
                case "gesture":
                    Debug.Log("[Network] Gesture received: " + data.msg);
                    OnGestureReceived?.Invoke(data.msg);
                    break;

                // -------- FRIENDS / REQUESTS LIST UPDATE --------
                case "friends_list":
                case "friend_added":
                case "new_request":
                    // Αν έρθει "new_request" χωρίς πακέτο friends/requests, ζητάμε μόνοι μας
                    if (data.type == "new_request" &&
                        (data.requests == null || data.requests.Length == 0) &&
                        (data.friends == null || data.friends.Length == 0))
                    {
                        FetchFriendsAndRequests();
                    }
                    else
                    {
                        UpdateLocalLists(data);
                    }
                    break;

                // -------- CHAT MESSAGE --------
                case "chat":
                    if (OnChatMessageReceived != null)
                    {
                        string fromUser = string.IsNullOrEmpty(data.sender) ? "Unknown" : data.sender;
                        string text = data.msg;
                        OnChatMessageReceived.Invoke(fromUser, text);
                    }
                    break;

                // Μπορείς να προσθέσεις case "error", "request_sent" κλπ αν θέλεις
                default:
                    // Debug.Log("[Network] Unhandled message type: " + data.type);
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("[Network] JSON Error: " + e.Message);
        }
    }

    // ------------------------------------------------
    //  Update local friends / requests
    // ------------------------------------------------
    private void UpdateLocalLists(MessageData data)
    {
        bool changed = false;

        if (data.friends != null)
        {
            Friends = new List<string>(data.friends);
            changed = true;
        }
        if (data.requests != null)
        {
            PendingRequests = new List<string>(data.requests);
            changed = true;
        }

        if (changed)
        {
            Debug.Log($"[Network] Lists Updated. Friends: {Friends.Count}, Requests: {PendingRequests.Count}");
            OnListsUpdated?.Invoke();
        }
    }

    // ------------------------------------------------
    //  Public API – AUTH / FRIENDS
    // ------------------------------------------------
    public void SignIn(string u, string p)
    {
        Send(new MessageData
        {
            type = "login",
            username = u,
            password = p
        });
    }

    public void Register(string u, string e, string p)
    {
        Send(new MessageData
        {
            type = "register",
            username = u,
            email = e,
            password = p
        });
    }

    public void SearchUser(string q)
    {
        Send(new MessageData
        {
            type = "search_user",
            query = q
        });
    }

    public void SendFriendRequest(string target)
    {
        Send(new MessageData
        {
            type = "send_request",
            target_user = target
        });
    }

    public void RespondToRequest(string reqUser, bool accept)
    {
        Send(new MessageData
        {
            type = "respond_request",
            requester = reqUser,
            action = accept ? "accept" : "reject"
        });
    }

    public void FetchFriendsAndRequests()
    {
        Send(new MessageData
        {
            type = "get_friends",
            username = CurrentUsername
        });
    }

    // ------------------------------------------------
    //  Rooms / Chat
    // ------------------------------------------------
    public void JoinRoom(string roomName)
    {
        if (string.IsNullOrEmpty(roomName))
            roomName = "main";

        Send(new MessageData
        {
            type = "join",
            room = roomName,
            username = CurrentUsername
        });

        Debug.Log("[Network] JoinRoom: " + roomName);
    }

    /// <summary>
    /// Καλείται από το Chat UI όταν πατάς Send.
    /// Στέλνει μήνυμα τύπου "chat" στον server.
    /// </summary>
    public void SendChatMessageToCurrentPeer(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            Debug.LogWarning("[Chat] Empty message, ignoring.");
            return;
        }

        if (_ws == null || _ws.State != WebSocketState.Open)
        {
            Debug.LogWarning("[Chat] WebSocket not connected.");
            return;
        }

        if (string.IsNullOrEmpty(CurrentUsername))
        {
            Debug.LogWarning("[Chat] Not logged in.");
            return;
        }

        Debug.Log($"[Chat] Send '{text}' to room via server");

        Send(new MessageData
        {
            type = "chat",
            msg = text
            // room το βλέπει ο server από CLIENTS[ws]["room"]
        });

        // ΠΡΟΣΟΧΗ:
        // ΔΕΝ καλούμε OnChatMessageReceived εδώ, γιατί αυτό σημαίνει ότι
        // θα φτιάχνουμε bubble *δύο φορές* αν το UI κάνει listen.
        // Το UI μπορεί να δημιουργεί ΤΟ ΔΙΚΟ ΜΟΥ bubble αμέσως
        // όταν πατάω Send (πχ. SimpleChatUI.AddOwnBubble),
        // και να χρησιμοποιεί το event μόνο για μηνύματα από άλλους χρήστες.
    }

    /// <summary>
    /// Αν ποτέ θελήσεις να προωθήσεις chat από άλλο σημείο (όχι από WebSocket),
    /// μπορείς να καλέσεις αυτή τη μέθοδο.
    /// </summary>
    public void ReceiveChatFromServer(string fromUser, string text)
    {
        Debug.Log($"[Chat] Received from {fromUser}: {text}");
        OnChatMessageReceived?.Invoke(fromUser, text);
    }


    public void SendGesture(string gesture)
{
    Send(new MessageData
    {
        type = "gesture",
        msg = gesture
    });
}


}
