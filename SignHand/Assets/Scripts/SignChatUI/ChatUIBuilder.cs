using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChatUIBuilder : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField inputField;
    public Button sendButton;
    public RectTransform messagesContent;

    [Header("Bubble Colors")]
    public Color myBubbleColor = new Color(0.2f, 0.6f, 1f);
    public Color otherBubbleColor = new Color(0.85f, 0.85f, 0.85f);
    public GestureController GestureControllerInstance;

    private void Awake()
    {
        if (sendButton != null)
            sendButton.onClick.AddListener(OnSendClicked);
    }

    private void OnEnable()
    {
        if (NetworkManager.Instance != null)
            NetworkManager.Instance.OnChatMessageReceived += HandleIncomingMessage;
    }

    private void OnDisable()
    {
        if (NetworkManager.Instance != null)
            NetworkManager.Instance.OnChatMessageReceived -= HandleIncomingMessage;
    }

    private void Update()
    {
        if (inputField != null && inputField.isFocused && Input.GetKeyDown(KeyCode.Return))
            OnSendClicked();
    }


    private void CheckForGesture(string txt)
    {
        txt = txt.ToLower();

        if (GestureControllerInstance == null) return;

        if (txt.Contains("fist"))
            GestureControllerInstance.DoFist();
        else if (txt.Contains("rest"))
            GestureControllerInstance.DoRest();
        else if (txt.Contains("index"))
            GestureControllerInstance.DoIndexPoint();
        else if (txt.Contains("middle"))
            GestureControllerInstance.DoMiddlePoint();
    }

    private void OnSendClicked()
    {
        string txt = inputField.text.Trim();
        if (string.IsNullOrEmpty(txt)) return;

        CreateChatBubble(txt, true);

        // 1️⃣ DETECT GESTURE
        CheckForGesture(txt);

        // 2️⃣ Send to network
        if (NetworkManager.Instance != null)
            NetworkManager.Instance.SendChatMessageToCurrentPeer(txt);

        inputField.text = "";
        inputField.ActivateInputField();
    }


    private void HandleIncomingMessage(string fromUser, string text)
    {
        bool isMine = (fromUser == NetworkManager.Instance.CurrentUsername);
        CreateChatBubble(text, isMine);
    }
    public void AddIncomingVoiceMessage(string cmd)
    {
        CreateChatBubble("Voice command: " + cmd, false);
    }

    public void CreateChatBubble(string message, bool isMine)
    {
        if (messagesContent == null) return;

        GameObject rowGO = new GameObject(isMine ? "Row_Me" : "Row_Other", typeof(RectTransform));
        rowGO.transform.SetParent(messagesContent, false);

        var rowHLG = rowGO.AddComponent<HorizontalLayoutGroup>();
        rowHLG.childControlHeight = false;
        rowHLG.childControlWidth = false;
        rowHLG.childForceExpandWidth = false;
        rowHLG.childForceExpandHeight = false;
        rowHLG.spacing = 4;

        if (isMine)
            rowHLG.childAlignment = TextAnchor.MiddleRight;
        else
            rowHLG.childAlignment = TextAnchor.MiddleLeft;

        GameObject bubbleGO = new GameObject("Bubble", typeof(RectTransform));
        bubbleGO.transform.SetParent(rowGO.transform, false);

        Image bubbleImg = bubbleGO.AddComponent<Image>();
        bubbleImg.color = isMine ? myBubbleColor : otherBubbleColor;

        var bubbleVLG = bubbleGO.AddComponent<VerticalLayoutGroup>();
        bubbleVLG.padding = new RectOffset(10, 10, 6, 6);

        var bubbleLE = bubbleGO.AddComponent<LayoutElement>();
        bubbleLE.preferredWidth = 260;

        var csf = bubbleGO.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        GameObject textGO = new GameObject("Text", typeof(RectTransform));
        textGO.transform.SetParent(bubbleGO.transform, false);

        TextMeshProUGUI tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = message;
        tmp.fontSize = 16;
        tmp.color = isMine ? Color.white : Color.black;
        tmp.enableWordWrapping = true;

        // Refresh
        Canvas.ForceUpdateCanvases();
        var scroll = messagesContent.GetComponentInParent<ScrollRect>();
        if (scroll) scroll.verticalNormalizedPosition = 0f;
        Canvas.ForceUpdateCanvases();
    }
}
