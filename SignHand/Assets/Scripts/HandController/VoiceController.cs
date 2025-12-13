using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Text.RegularExpressions;

public class VoiceController : MonoBehaviour
{
    public GestureController gestures;

    private DictationRecognizer recognizer;
    private System.Action<string> onTextCallback;
    private System.Action<string> onGestureCallback;

    void Start()
    {
        recognizer = new DictationRecognizer();
        recognizer.InitialSilenceTimeoutSeconds = 5;
        recognizer.AutoSilenceTimeoutSeconds = 2;

        recognizer.DictationResult += OnDictationResult;
        recognizer.DictationComplete += (cause) =>
        {
            Debug.Log("Dictation completed: " + cause);
        };
    }

    public void StartListening()
    {
        if (recognizer.Status != SpeechSystemStatus.Running)
            recognizer.Start();
    }

    public void StopListening()
    {
        if (recognizer.Status == SpeechSystemStatus.Running)
            recognizer.Stop();
    }

    public void SetSubtitleCallback(System.Action<string> cb)
    {
        onTextCallback = cb;   // sends subtitles to UI
    }

    public void SetGestureCallback(System.Action<string> cb)
    {
        onGestureCallback = cb; // sends gestures to UI
    }

    private void OnDictationResult(string text, ConfidenceLevel confidence)
    {
        string lower = text.ToLower();

        // 1️⃣ SUBTITLES → CHAT
        onTextCallback?.Invoke(text);

        // 2️⃣ GESTURE DETECTION
        if (lower.Contains("fist"))
        {
            gestures.DoFist();
            onGestureCallback?.Invoke("fist");
        }
        else if (lower.Contains("rest"))
        {
            gestures.DoRest();
            onGestureCallback?.Invoke("rest");
        }
        else if (lower.Contains("index"))
        {
            gestures.DoIndexPoint();
            onGestureCallback?.Invoke("index");
        }
        else if (lower.Contains("middle"))
        {
            gestures.DoMiddlePoint();
            onGestureCallback?.Invoke("middle");
        }
    }
}
