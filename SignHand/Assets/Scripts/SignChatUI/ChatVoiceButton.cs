// using UnityEngine;
// using UnityEngine.UI;

// public class ChatVoiceButton : MonoBehaviour
// {
//     public ChatUIBuilder chatUI;
//     public VoiceController voiceController;
//     public Button button;

//     private bool isListening = false;

//     void Start()
//     {
//         if (button == null)
//             button = GetComponent<Button>();

//         // Set callback from VoiceController → Chat UI
//         voiceController.SetCallback(OnVoiceCommandDetected);

//         button.onClick.AddListener(ToggleListening);
//     }

//     private void ToggleListening()
//     {
//         if (!isListening)
//         {
//             isListening = true;
//             chatUI.CreateChatBubble("Listening...", true);
//             voiceController.StartListening();
//         }
//         else
//         {
//             isListening = false;
//             chatUI.CreateChatBubble("Stopped listening.", true);
//             voiceController.StopListening();
//         }
//     }

//     private void OnVoiceCommandDetected(string cmd)
//     {
//         chatUI.CreateChatBubble("Voice command: " + cmd, true);

//         // Stop listening automatically
//         isListening = false;
//     }
// }

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChatVoiceButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public ChatUIBuilder chatUI;
    public VoiceController voice;

    void Start()
    {
        voice.SetSubtitleCallback(OnSubtitle);
        voice.SetGestureCallback(OnGesture);
    }

    public void OnPointerDown(PointerEventData e)
    {
        chatUI.CreateChatBubble("Listening…", true);
        voice.StartListening();
    }

    public void OnPointerUp(PointerEventData e)
    {
        voice.StopListening();
    }

    private void OnSubtitle(string text)
    {
        chatUI.CreateChatBubble(text, true);
    }

    private void OnGesture(string gesture)
    {
        
    }
}

