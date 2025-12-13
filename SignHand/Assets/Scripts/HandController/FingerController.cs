using UnityEngine;

public class FingerController : MonoBehaviour
{
    [Header("Index Finger Bones")]
    public Transform index1;
    public Transform index2;
    public Transform index3;

    [Header("Middle Finger Bones")]
    public Transform middle1;
    public Transform middle2;
    public Transform middle3;

    [Header("Ring Finger Bones")]
    public Transform ring1;
    public Transform ring2;
    public Transform ring3;

    [Header("Little Finger Bones")]
    public Transform little1;
    public Transform little2;
    public Transform little3;

    [Header("Thumb Bones")]
    public Transform thumb1;
    public Transform thumb2;
    public Transform thumb3;

    [Header("Rotation Settings")]
    public float maxBendAngle = 70f;   // how much we bend inward
    public float smoothSpeed = 10f;

    // Bend targets
    public float targetIndexBend = 0f;
    public float targetMiddleBend = 0f;
    public float targetRingBend = 0f;
    public float targetLittleBend = 0f;
    public float targetThumbBend = 0f;

    void Update()
    {
        ApplyBend(index1, index2, index3, targetIndexBend);
        ApplyBend(middle1, middle2, middle3, targetMiddleBend);
        ApplyBend(ring1, ring2, ring3, targetRingBend);
        ApplyBend(little1, little2, little3, targetLittleBend);
        ApplyBend(thumb1, thumb2, thumb3, targetThumbBend);
    }

    private void ApplyBend(Transform b1, Transform b2, Transform b3, float bend)
    {
        // Bend inward
        float angle = -bend * maxBendAngle;
        Quaternion targetRot = Quaternion.Euler(0, 0, angle);

        if (b1 != null)
            b1.localRotation = Quaternion.Lerp(b1.localRotation, targetRot, Time.deltaTime * smoothSpeed);

        if (b2 != null)
            b2.localRotation = Quaternion.Lerp(b2.localRotation, targetRot, Time.deltaTime * smoothSpeed);

        if (b3 != null)
            b3.localRotation = Quaternion.Lerp(b3.localRotation, targetRot, Time.deltaTime * smoothSpeed);
    }

    // ---- setters ----
    public void SetIndexBend(float v) { targetIndexBend = Mathf.Clamp01(v); }
    public void SetMiddleBend(float v) { targetMiddleBend = Mathf.Clamp01(v); }
    public void SetRingBend(float v) { targetRingBend = Mathf.Clamp01(v); }
    public void SetLittleBend(float v) { targetLittleBend = Mathf.Clamp01(v); }
    public void SetThumbBend(float v) { targetThumbBend = Mathf.Clamp01(v); }
}
