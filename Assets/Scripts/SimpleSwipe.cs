using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SimpleSwipe : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public ScrollRect scrollRect;
    public float swipeThreshold = 50f;

    private Vector2 startPos;

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPos = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Пустой метод, но он нужен для интерфейса
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float dragDistance = eventData.position.x - startPos.x;

        if (Mathf.Abs(dragDistance) > swipeThreshold)
        {
            if (dragDistance > 0)
            {
                // Свайп вправо - первое изображение
                scrollRect.horizontalNormalizedPosition = 0;
            }
            else
            {
                // Свайп влево - второе изображение
                scrollRect.horizontalNormalizedPosition = 1;
            }
        }
    }
}