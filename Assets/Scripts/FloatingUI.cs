using UnityEngine;

public class FloatingUI : MonoBehaviour
{
    [Header("Float Settings")]
    [Tooltip("The speed of the floating motion.")]
    public float floatSpeed = 1.5f;

    [Tooltip("The vertical distance the UI element floats.")]
    public float floatAmplitude = 8f;

    [Tooltip("The horizontal distance the UI element floats (optional).")]
    public float floatHorizontalAmplitude = 0f;

    [Tooltip("An optional phase offset so different elements bob out of sync.")]
    public float phaseOffset = 0f;

    private Vector2 originalPosition;
    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            originalPosition = rectTransform.anchoredPosition;
        }
    }

    private void Update()
    {
        if (rectTransform == null) return;

        float timeFactor = (Time.time * floatSpeed) + phaseOffset;
        float newY = originalPosition.y + Mathf.Sin(timeFactor) * floatAmplitude;
        float newX = originalPosition.x + Mathf.Cos(timeFactor * 0.7f) * floatHorizontalAmplitude;

        rectTransform.anchoredPosition = new Vector2(newX, newY);
    }
}
