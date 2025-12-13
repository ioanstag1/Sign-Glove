using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuLogic : MonoBehaviour
{
    [Header("Panels")]
    public GameObject startPanel;
    public GameObject loginPanel;
    public GameObject registerPanel;
    public GameObject selectionPanel;

    [Header("Login")]
    public TMP_InputField loginUsernameInput;
    public TMP_InputField loginPasswordInput;
    public TextMeshProUGUI loginErrorText;

    [Header("Register")]
    public TMP_InputField regUsernameInput;
    public TMP_InputField regEmailInput;
    public TMP_InputField regPasswordInput;
    public TextMeshProUGUI regErrorText;

    [Header("Top Bar")]
    public TextMeshProUGUI welcomeText;

    [Header("Friends Drawer")]
    public GameObject dimmerObj;
    public GameObject friendsPanel;
    public TMP_InputField searchInput;
    public TextMeshProUGUI searchResultText;
    public GameObject addFriendBtn;
    public RectTransform requestsListContent;
    public RectTransform friendsListContent;

    [Header("Style")]
    public TMP_FontAsset uiFont;          // Font που θα χρησιμοποιούν ΟΛΑ τα δυναμικά texts

    [Header("Navigation")]
    public SceneNavigator navigator;      // Scene navigation helper

    private string _foundUser = "";

    // Row colors
    private static readonly Color ROW_BG    = new Color(0.98f, 0.98f, 0.99f);
    private static readonly Color ROW_BORDER = new Color(0, 0, 0, 0.05f);

    private void Start()
    {
        ShowStart();

        if (friendsPanel) friendsPanel.SetActive(false);
        if (dimmerObj) dimmerObj.SetActive(false);
        if (addFriendBtn) addFriendBtn.SetActive(false);

        if (NetworkManager.Instance != null)
        {
            NetworkManager.Instance.OnLoginSuccess    += HandleLoginSuccess;
            NetworkManager.Instance.OnLoginFailed     += (msg) => ShowError(loginErrorText, msg);
            NetworkManager.Instance.OnRegisterSuccess += (u) =>
            {
                ShowStart();
                ShowError(loginErrorText, "Account created! Please log in.", Color.green);
            };
            NetworkManager.Instance.OnRegisterFailed  += (msg) => ShowError(regErrorText, msg);

            NetworkManager.Instance.OnSearchFound     += OnSearchFound;
            NetworkManager.Instance.OnSearchNotFound  += OnSearchNotFound;
            NetworkManager.Instance.OnListsUpdated    += RefreshFriendsUI;
        }
    }

    private void OnDestroy()
    {
        if (NetworkManager.Instance != null)
        {
            NetworkManager.Instance.OnLoginSuccess    -= HandleLoginSuccess;
            NetworkManager.Instance.OnSearchFound     -= OnSearchFound;
            NetworkManager.Instance.OnSearchNotFound  -= OnSearchNotFound;
            NetworkManager.Instance.OnListsUpdated    -= RefreshFriendsUI;
        }
    }

    // ---------- Navigation between panels ----------
    private void ToggleAllOff()
    {
        if (startPanel)     startPanel.SetActive(false);
        if (loginPanel)     loginPanel.SetActive(false);
        if (registerPanel)  registerPanel.SetActive(false);
        if (selectionPanel) selectionPanel.SetActive(false);
        if (friendsPanel)   friendsPanel.SetActive(false);
        if (dimmerObj)      dimmerObj.SetActive(false);
    }

    public void ShowStart()
    {
        ToggleAllOff();
        if (startPanel) startPanel.SetActive(true);
    }

    public void ShowLogin()
    {
        ToggleAllOff();
        if (loginPanel) loginPanel.SetActive(true);
        if (loginErrorText) loginErrorText.gameObject.SetActive(false);
    }

    public void ShowRegister()
    {
        ToggleAllOff();
        if (registerPanel) registerPanel.SetActive(true);
        if (regErrorText) regErrorText.gameObject.SetActive(false);
    }

    // ---------- Auth ----------
    public void AttemptLogin()
    {
        if (loginUsernameInput == null || loginPasswordInput == null) return;

        string u = loginUsernameInput.text;
        string p = loginPasswordInput.text;

        if (string.IsNullOrEmpty(u) || string.IsNullOrEmpty(p))
        {
            ShowError(loginErrorText, "Fields cannot be empty.");
            return;
        }

        if (NetworkManager.Instance != null)
            NetworkManager.Instance.SignIn(u, p);
    }

    public void AttemptRegister()
    {
        if (NetworkManager.Instance != null)
            NetworkManager.Instance.Register(regUsernameInput.text, regEmailInput.text, regPasswordInput.text);
    }

    public void LogOut()
    {
        ShowStart();
        if (NetworkManager.Instance != null)
        {
            NetworkManager.Instance.CurrentUsername = "";
            NetworkManager.Instance.Friends.Clear();
            NetworkManager.Instance.PendingRequests.Clear();
        }
    }

    private void HandleLoginSuccess(string username)
    {
        ToggleAllOff();
        if (selectionPanel) selectionPanel.SetActive(true);
        if (welcomeText) welcomeText.text = "Hello, " + username + "!";

        if (NetworkManager.Instance != null)
            NetworkManager.Instance.FetchFriendsAndRequests();
    }

    private void ShowError(TextMeshProUGUI txt, string msg, Color? col = null)
    {
        if (txt == null) return;
        txt.text  = msg;
        txt.color = col ?? new Color(0.9f, 0.3f, 0.3f);
        txt.gameObject.SetActive(true);
    }

    // ---------- Friends Drawer ----------
    public void ToggleFriendsMenu()
    {
        if (!friendsPanel) return;

        bool show = !friendsPanel.activeSelf;
        friendsPanel.SetActive(show);
        if (dimmerObj) dimmerObj.SetActive(show);

        if (show)
        {
            RefreshFriendsUI();

            if (NetworkManager.Instance != null)
                NetworkManager.Instance.FetchFriendsAndRequests();
        }
    }

    private void OnSearchFound(string username)
    {
        _foundUser = username;
        if (searchResultText)
        {
            searchResultText.text  = "User found: " + username;
            searchResultText.color = new Color(0, 0.65f, 0.5f);
        }
        if (addFriendBtn) addFriendBtn.SetActive(true);
    }

    private void OnSearchNotFound()
    {
        _foundUser = "";
        if (searchResultText)
        {
            searchResultText.text  = "User not found.";
            searchResultText.color = Color.red;
        }
        if (addFriendBtn) addFriendBtn.SetActive(false);
    }

    public void SearchUser()
    {
        if (NetworkManager.Instance == null || searchInput == null) return;
        if (string.IsNullOrEmpty(searchInput.text)) return;

        NetworkManager.Instance.SearchUser(searchInput.text);
    }

    public void SendFriendRequestUI()
    {
        if (NetworkManager.Instance == null) return;
        if (string.IsNullOrEmpty(_foundUser)) return;

        NetworkManager.Instance.SendFriendRequest(_foundUser);

        if (searchResultText)
        {
            searchResultText.text  = "Request sent to " + _foundUser;
            searchResultText.color = new Color(0, 0.65f, 0.5f);
        }
        if (addFriendBtn) addFriendBtn.SetActive(false);
        if (searchInput)  searchInput.text = "";
        _foundUser = "";
    }

    // ---------- Build friends & requests lists ----------
    private void RefreshFriendsUI()
    {
        if (NetworkManager.Instance == null || friendsListContent == null || requestsListContent == null) return;

        Debug.Log($"[MainMenuLogic] RefreshFriendsUI → Friends={NetworkManager.Instance.Friends.Count}, Requests={NetworkManager.Instance.PendingRequests.Count}");

        foreach (Transform child in friendsListContent)   Destroy(child.gameObject);
        foreach (Transform child in requestsListContent)  Destroy(child.gameObject);

        foreach (string reqUser in NetworkManager.Instance.PendingRequests)
            CreateRequestRow(reqUser);

        foreach (string friend in NetworkManager.Instance.Friends)
            CreateFriendRow(friend);

        Canvas.ForceUpdateCanvases();
    }

    private void CreateRequestRow(string username)
    {
        GameObject row = new GameObject("ReqRow", typeof(RectTransform));
        row.transform.SetParent(requestsListContent, false);

        Image bg = row.AddComponent<Image>();
        bg.color = ROW_BG;

        HorizontalLayoutGroup hlg = row.AddComponent<HorizontalLayoutGroup>();
        hlg.padding               = new RectOffset(10, 10, 4, 4);
        hlg.spacing               = 6;
        hlg.childAlignment        = TextAnchor.MiddleLeft;
        hlg.childControlWidth     = true;
        hlg.childControlHeight    = true;
        hlg.childForceExpandWidth = false;
        hlg.childForceExpandHeight = false;

        LayoutElement le = row.AddComponent<LayoutElement>();
        le.minHeight       = 32;
        le.preferredHeight = 36;

        // Name
        GameObject txtObj = new GameObject("Name", typeof(RectTransform));
        txtObj.transform.SetParent(row.transform, false);
        TextMeshProUGUI txt = txtObj.AddComponent<TextMeshProUGUI>();
        txt.text = username;
        if (uiFont != null) txt.font = uiFont;
        txt.color            = new Color(0.15f, 0.16f, 0.20f);
        txt.fontSize         = 16;
        txt.alignment        = TextAlignmentOptions.MidlineLeft;
        txt.enableWordWrapping = false;

        LayoutElement nameLE = txtObj.AddComponent<LayoutElement>();
        nameLE.flexibleWidth = 1;

        // ACCEPT
        Button acceptBtn = CreateTagButton(row.transform, "ACCEPT", new Color(0.18f, 0.65f, 0.38f));
        acceptBtn.onClick.AddListener(() =>
        {
            if (NetworkManager.Instance != null)
            {
                NetworkManager.Instance.PendingRequests.Remove(username);
                if (!NetworkManager.Instance.Friends.Contains(username))
                    NetworkManager.Instance.Friends.Add(username);

                NetworkManager.Instance.RespondToRequest(username, true);
                RefreshFriendsUI();
            }
        });

        // REJECT
        Button rejectBtn = CreateTagButton(row.transform, "REJECT", new Color(0.86f, 0.32f, 0.32f));
        rejectBtn.onClick.AddListener(() =>
        {
            if (NetworkManager.Instance != null)
            {
                NetworkManager.Instance.PendingRequests.Remove(username);
                NetworkManager.Instance.RespondToRequest(username, false);
                RefreshFriendsUI();
            }
        });
    }

    private void CreateFriendRow(string username)
    {
        GameObject row = new GameObject("FriendRow", typeof(RectTransform));
        row.transform.SetParent(friendsListContent, false);

        Image bg = row.AddComponent<Image>();
        bg.color = ROW_BG;

        HorizontalLayoutGroup hlg = row.AddComponent<HorizontalLayoutGroup>();
        hlg.padding               = new RectOffset(10, 10, 4, 4);
        hlg.spacing               = 6;
        hlg.childAlignment        = TextAnchor.MiddleLeft;
        hlg.childControlWidth     = true;
        hlg.childControlHeight    = true;
        hlg.childForceExpandWidth = false;
        hlg.childForceExpandHeight = false;

        LayoutElement le = row.AddComponent<LayoutElement>();
        le.minHeight       = 32;
        le.preferredHeight = 36;

        // Icon
        GameObject icon = new GameObject("Icon", typeof(RectTransform));
        icon.transform.SetParent(row.transform, false);
        Image iconImg = icon.AddComponent<Image>();
        iconImg.color = new Color(0.82f, 0.84f, 0.88f);
        LayoutElement iconLE = icon.AddComponent<LayoutElement>();
        iconLE.preferredWidth  = 24;
        iconLE.preferredHeight = 24;

        // Name
        GameObject txtObj = new GameObject("Name", typeof(RectTransform));
        txtObj.transform.SetParent(row.transform, false);
        RectTransform nameRT = txtObj.GetComponent<RectTransform>();
        nameRT.anchorMin = new Vector2(0, 0.5f);
        nameRT.anchorMax = new Vector2(1, 0.5f);
        nameRT.sizeDelta = new Vector2(0, 20);

        TextMeshProUGUI txt = txtObj.AddComponent<TextMeshProUGUI>();
        txt.text = username;
        if (uiFont != null) txt.font = uiFont;
        txt.color            = new Color(0.15f, 0.16f, 0.20f);
        txt.fontSize         = 16;
        txt.alignment        = TextAlignmentOptions.MidlineLeft;
        txt.enableWordWrapping = false;
        txt.overflowMode     = TextOverflowModes.Ellipsis;
        txt.raycastTarget    = false;

        LayoutElement nameLE = txtObj.AddComponent<LayoutElement>();
        nameLE.flexibleWidth = 1;

        // CHAT button
        Button chatBtn = CreateTagButton(row.transform, "CHAT", new Color(0.2f, 0.5f, 0.95f));
        // εδώ μπορείς αργότερα να κάνεις chatBtn.onClick.AddListener(() => OpenChat(username));
    }

    // Μικρό "tag" κουμπί
    private Button CreateTagButton(Transform parent, string label, Color bg)
    {
        GameObject btnObj = new GameObject(label + "Btn", typeof(RectTransform));
        btnObj.transform.SetParent(parent, false);

        Image img = btnObj.AddComponent<Image>();
        img.color = bg;

        Button btn = btnObj.AddComponent<Button>();

        LayoutElement le = btnObj.AddComponent<LayoutElement>();
        le.preferredWidth = 80;
        le.minHeight      = 26;

        GameObject txtObj = new GameObject("Txt", typeof(RectTransform));
        txtObj.transform.SetParent(btnObj.transform, false);
        RectTransform rt = txtObj.GetComponent<RectTransform>();
        rt.anchorMin  = Vector2.zero;
        rt.anchorMax  = Vector2.one;
        rt.offsetMin  = Vector2.zero;
        rt.offsetMax  = Vector2.zero;

        TextMeshProUGUI txt = txtObj.AddComponent<TextMeshProUGUI>();
        txt.text = label;
        if (uiFont != null) txt.font = uiFont;
        txt.color    = Color.white;
        txt.fontSize = 13;
        txt.alignment = TextAlignmentOptions.Center;

        return btn;
    }

    // ---------- Mode buttons (καλούν SceneNavigator) ----------
    public void GoToGestureMode()
    {
        Debug.Log("Gesture Mode button clicked");
        if (navigator != null) navigator.GoToGestureMode();
    }

    public void GoToChatMode()
    {
        Debug.Log("Chat Mode button clicked");
        if (navigator != null) navigator.GoToChatMode();
    }

    public void GoToLearnMode()
    {
        Debug.Log("Learn Mode button clicked");
        if (navigator != null) navigator.GoToLearnMode();
    }
}
