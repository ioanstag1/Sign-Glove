using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class UDPReceiver : MonoBehaviour
{
    public GestureController gestureController;

    // 🔵 Event ώστε το Chat να παίρνει gesture text
    public event Action<string> OnGestureParsed;

    UdpClient client;
    Thread receiveThread;

    void Start()
    {
        if (gestureController == null)
            gestureController = FindObjectOfType<GestureController>();

        client = new UdpClient(5005);

        receiveThread = new Thread(ReceiveData);
        receiveThread.IsBackground = true;
        receiveThread.Start();

        Debug.Log("<color=green>[UDP] Receiver started on port 5005</color>");
    }

    void ReceiveData()
    {
        IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

        while (true)
        {
            try
            {
                byte[] data = client.Receive(ref remoteEP);
                string msg = Encoding.UTF8.GetString(data);

                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    TriggerGesture(msg);
                });
            }
            catch { }
        }
    }

    void TriggerGesture(string g)
    {
        if (gestureController == null)
        {
            Debug.LogError("<color=red>[UDP] GestureController NOT FOUND!</color>");
            return;
        }

        g = g.ToLower().Trim();

        Debug.Log($"<color=yellow>[UDP] Parsed gesture: {g}</color>");

        // 🔵 🔥 NEW: Notify GestureChatController
        OnGestureParsed?.Invoke(g);

        // 🔥 SEND TO SERVER
        if (NetworkManager.Instance != null)
        {
            Debug.Log($"<color=magenta>[WS] Sending gesture to server: {g}</color>");
            NetworkManager.Instance.SendGesture(g);
        }
        else
        {
            Debug.LogError("<color=red>[WS] NetworkManager.Instance is NULL!</color>");
        }

        // 🔥 LOCAL hand animation
        switch (g)
        {
            case "rest":
            case "open":
                Debug.Log("<color=orange>[LOCAL] Executing REST gesture</color>");
                gestureController.DoRest(false);
                break;

            case "fist":
                Debug.Log("<color=orange>[LOCAL] Executing FIST gesture</color>");
                gestureController.DoFist(false);
                break;

            case "index":
            case "index_point":
                Debug.Log("<color=orange>[LOCAL] Executing INDEX gesture</color>");
                gestureController.DoIndexPoint(false);
                break;

            case "middle":
            case "middle_point":
                Debug.Log("<color=orange>[LOCAL] Executing MIDDLE gesture</color>");
                gestureController.DoMiddlePoint(false);
                break;

            default:
                Debug.Log($"<color=red>[UDP] UNKNOWN gesture received: {g}</color>");
                break;
        }
    }
}
