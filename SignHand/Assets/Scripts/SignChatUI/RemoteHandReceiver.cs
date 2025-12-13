using UnityEngine;

public class RemoteHandReceiver : MonoBehaviour
{
    public GestureController remoteHand;
    public ChatUIBuilder chatUI; // optional

    private void OnEnable()
    {
        if (NetworkManager.Instance != null)
            NetworkManager.Instance.OnGestureReceived += HandleGesture;
    }

    private void OnDisable()
    {
        if (NetworkManager.Instance != null)
            NetworkManager.Instance.OnGestureReceived -= HandleGesture;
    }

    private void HandleGesture(string gesture)
    {
        if (remoteHand == null)
        {
            Debug.LogError("RemoteHandReceiver: No remote hand assigned!");
            return;
        }

        Debug.Log("<color=cyan>[REMOTE] Gesture received: " + gesture + "</color>");

        switch (gesture.ToLower())
        {
            case "rest":   remoteHand.DoRest(); break;
            case "fist":   remoteHand.DoFist(); break;
            case "index":  remoteHand.DoIndexPoint(); break;
            case "middle": remoteHand.DoMiddlePoint(); break;
        }

        // OPTIONAL → show in chat
        if (chatUI != null)
            chatUI.CreateChatBubble("Remote gesture: " + gesture, false);
    }
}
