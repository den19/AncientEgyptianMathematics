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
    }
}