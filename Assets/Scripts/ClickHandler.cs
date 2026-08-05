using UnityEngine;

public class ClickHandler : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Левый клик мыши/тап
        {
            if (StateManager.Instance != null)
            {
                // Получаем позицию клика в мировых координатах
                Vector3 clickPosition = GetClickWorldPosition();

                // Передаем позицию клика в StateManager
                StateManager.Instance.AddClick(clickPosition);
            }
        }
    }

    private Vector3 GetClickWorldPosition()
    {
        // Для 2D игры
        Camera cam = Camera.main;
        if (cam == null)
            return Vector3.zero;

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -cam.transform.position.z;
        return cam.ScreenToWorldPoint(mousePos);

        // Для 3D игры раскомментируйте следующее:
        // Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // RaycastHit hit;
        // if (Physics.Raycast(ray, out hit))
        // {
        //     return hit.point;
        // }
        // return Vector3.zero;
    }
}