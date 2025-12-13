using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GestureChatController : MonoBehaviour
{
    public UDPReceiver gestureReceiver;
    public ChatUIBuilder chatUI;
    public TMP_InputField chatInput;

    private bool isCapturing = false;

    void Start()
    {
        // Όταν έρθει gesture από UDP → βάλ’ το στο chat
        gestureReceiver.OnGestureParsed += OnGestureReceived;
    }

    public void StartGestureCapture()
    {
        isCapturing = true;
        chatInput.text = "";
        chatUI.CreateChatBubble("Gesture input: Listening…", true);
    }

    private void OnGestureReceived(string gesture)
    {
        if (!isCapturing) return;

        // Μετατροπή gesture → κείμενο
        string text = GestureToText(gesture);

        // Βάζουμε το κείμενο στο chat input
        chatInput.text = text;

        chatUI.CreateChatBubble("Detected gesture: " + text, true);
    }

    public void StopGestureCapture()
    {
        isCapturing = false;
        chatUI.CreateChatBubble("Gesture input stopped.", true);
    }

    private string GestureToText(string g)
    {
        switch (g)
        {
            case "fist": return "FIST";
            case "rest": return "REST";
            case "index": return "INDEX POINT";
            case "middle": return "MIDDLE POINT";
            default: return g.ToUpper();
        }
    }
}
