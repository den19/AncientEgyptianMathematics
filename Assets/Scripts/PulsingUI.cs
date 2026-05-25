using UnityEngine;

public class PulsingUI : MonoBehaviour
{
    [Header("Pulse Settings")]
    [Tooltip("How fast the UI element pulses.")]
    public float pulseSpeed = 3f;

    [Tooltip("The range of the pulsing scale.")]
    public float scaleAmount = 0.12f;

    [Tooltip("An optional phase offset so different elements pulse out of sync.")]
    public float phaseOffset = 0f;

    private Vector3 originalScale;
    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            originalScale = rectTransform.localScale;
        }
        else
        {
            originalScale = transform.localScale;
        }
    }

    private void Update()
    {
        float pulse = 1f + Mathf.Sin((Time.time * pulseSpeed) + phaseOffset) * scaleAmount;
        
        if (rectTransform != null)
        {
            rectTransform.localScale = originalScale * pulse;
        }
        else
        {
            transform.localScale = originalScale * pulse;
        }
    }
}
