using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    void Start()
    {
        // Убеждаемся, что StateManager существует
        if (StateManager.Instance == null)
        {
            Instantiate(Resources.Load("StateManager"));
        }

        // Находим UI компоненты на текущей сцене
        StateManager.Instance.FindUIElements();

        if (SceneManager.GetActiveScene().name == "Progress"
            && FindObjectOfType<ProgressMapController>() == null)
        {
            var mapGo = new GameObject("ProgressMapController");
            mapGo.AddComponent<ProgressMapController>();
        }
    }
}