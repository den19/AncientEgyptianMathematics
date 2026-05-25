using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class WaterEffectUI : MonoBehaviour
{
    [Header("Water Scrolling Settings")]
    [Tooltip("The scrolling speed along the X axis.")]
    public float scrollSpeedX = 0.05f;

    [Tooltip("The scrolling speed along the Y axis.")]
    public float scrollSpeedY = 0.1f;

    private RawImage rawImage;
    private Rect uvRect;

    private void Start()
    {
        rawImage = GetComponent<RawImage>();
        if (rawImage != null)
        {
            uvRect = rawImage.uvRect;
        }
    }

    private void Update()
    {
        if (rawImage == null) return;

        // Плавное смещение текстурных координат RawImage
        float offsetX = (uvRect.x + scrollSpeedX * Time.deltaTime) % 1f;
        float offsetY = (uvRect.y + scrollSpeedY * Time.deltaTime) % 1f;

        rawImage.uvRect = new Rect(offsetX, offsetY, uvRect.width, uvRect.height);
    }
}
