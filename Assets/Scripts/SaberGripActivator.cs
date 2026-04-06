using UnityEngine;

public class SaberGripActivator : MonoBehaviour
{
    [Header("Meta Hand Tracking")]
    [SerializeField] private OVRHand rightHand;

    [Header("Saber Objects")]
    [SerializeField] private GameObject saberVisualRoot;   // usually pivot_lightsaber
    [SerializeField] private Collider bladeTrigger;        // BladeHitbox trigger collider

    [Header("Grip Heuristic")]
    [Range(0f, 1f)]
    [SerializeField] private float gripOnThreshold = 0.55f;

    [Range(0f, 1f)]
    [SerializeField] private float gripOffThreshold = 0.35f;

    [SerializeField] private bool requireHighConfidence = true;

    [Header("Debug")]
    [SerializeField] private bool logGripValue = false;

    private bool _isHolding;

    private void Start()
    {
        ApplyState(false);
    }

    private void Update()
    {
        if (rightHand == null)
        {
            ApplyState(false);
            return;
        }

        // Tracking validity first
        if (!rightHand.IsTracked)
        {
            ApplyState(false);
            return;
        }

        if (requireHighConfidence && rightHand.HandConfidence != OVRHand.TrackingConfidence.High)
        {
            ApplyState(false);
            return;
        }

        float gripValue = ComputeGripValue();

        if (logGripValue)
        {
            Debug.Log($"Grip Value: {gripValue:F2}");
        }

        // Hysteresis to avoid flicker
        if (!_isHolding && gripValue >= gripOnThreshold)
        {
            ApplyState(true);
        }
        else if (_isHolding && gripValue <= gripOffThreshold)
        {
            ApplyState(false);
        }
    }

    private float ComputeGripValue()
    {
        // For “holding an object”, use non-index fingers more heavily.
        // Index is often freer than the other fingers.
        float middle = rightHand.GetFingerPinchStrength(OVRHand.HandFinger.Middle);
        float ring   = rightHand.GetFingerPinchStrength(OVRHand.HandFinger.Ring);
        float pinky  = rightHand.GetFingerPinchStrength(OVRHand.HandFinger.Pinky);
        float thumb  = rightHand.GetFingerPinchStrength(OVRHand.HandFinger.Thumb);

        // Weighted average: middle/ring/pinky matter most for a wrapped grip
        float grip = (middle * 0.35f) + (ring * 0.30f) + (pinky * 0.25f) + (thumb * 0.10f);

        return Mathf.Clamp01(grip);
    }

    private void ApplyState(bool holding)
    {
        _isHolding = holding;

        if (saberVisualRoot != null)
        {
            saberVisualRoot.SetActive(holding);
        }

        if (bladeTrigger != null)
        {
            bladeTrigger.enabled = holding;
        }
    }

    public bool IsHoldingSaber()
    {
        return _isHolding;
    }
}