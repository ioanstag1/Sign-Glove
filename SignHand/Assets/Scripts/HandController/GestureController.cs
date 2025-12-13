using UnityEngine;

public class GestureController : MonoBehaviour
{
    public FingerController fingers;

    void Start()
    {
        if (fingers == null)
            fingers = GetComponent<FingerController>();
    }

    // -----------------------------
    //  SYNC TO SERVER (safe call)
    // -----------------------------
    private void SendGestureToNetwork(string gesture)
    {
        if (NetworkManager.Instance != null)
            NetworkManager.Instance.SendGesture(gesture);
    }

    // -----------------------------
    //      GESTURES
    // -----------------------------

    // REST → όλα χαλαρά
    public void DoRest(bool sendToNetwork = true)
    {
        fingers.SetIndexBend(0f);
        fingers.SetMiddleBend(0f);
        fingers.SetRingBend(0f);
        fingers.SetLittleBend(0f);
        fingers.SetThumbBend(0f);

        if (sendToNetwork) SendGestureToNetwork("rest");
    }

    // FIST → όλα λυγισμένα
    public void DoFist(bool sendToNetwork = true)
    {
        fingers.SetIndexBend(1f);
        fingers.SetMiddleBend(1f);
        fingers.SetRingBend(1f);
        fingers.SetLittleBend(1f);
        fingers.SetThumbBend(1f);

        if (sendToNetwork) SendGestureToNetwork("fist");
    }

    // INDEX POINT → μόνο ο δείκτης σηκωμένος
    public void DoIndexPoint(bool sendToNetwork = true)
    {
        fingers.SetIndexBend(0f);   // index open
        fingers.SetMiddleBend(1f);  // others closed
        fingers.SetRingBend(1f);
        fingers.SetLittleBend(1f);
        fingers.SetThumbBend(1f);

        if (sendToNetwork) SendGestureToNetwork("index");
    }

    // MIDDLE POINT → μόνο το μεσαίο σηκωμένο
    public void DoMiddlePoint(bool sendToNetwork = true)
    {
        fingers.SetIndexBend(1f);
        fingers.SetMiddleBend(0f);  // middle open
        fingers.SetRingBend(1f);
        fingers.SetLittleBend(1f);
        fingers.SetThumbBend(1f);

        if (sendToNetwork) SendGestureToNetwork("middle");
    }
}
