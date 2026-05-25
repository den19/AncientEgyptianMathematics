using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomSceneManager : MonoBehaviour
{
    // Статическая ссылка на единственный экземпляр
    private static CustomSceneManager instance;

    private static StateManager stateManager;

    private void Awake()
    {
        
    }

    // Свойство для обращения к экземпляру
    public static CustomSceneManager Instance
    {
        get
        {
            if (instance == null)
                instance = new GameObject("CustomSceneManager").AddComponent<CustomSceneManager>();

            return instance;
        }
    }

    // Метод для загрузки новой сцены
    public void LoadScene(string sceneName)
    {
        if (StateManager.Lesson1Complete)
        {
            StateManager.Instance.SetLevel1IconComplete();
        }

        if (StateManager.Lesson2Complete)
        {
            StateManager.Instance.SetLevel2IconComplete();
        }

        if (StateManager.Lesson3Complete)
        {
            StateManager.Instance.SetLevel3IconComplete();
        }

        if (StateManager.Lesson4Complete)
        {
            StateManager.Instance.SetLevel4IconComplete();
        }

        if (StateManager.Lesson5Complete)
        {
            StateManager.Instance.SetLevel5IconComplete();
        }

        if (StateManager.Lesson6Complete)
        {
            StateManager.Instance.SetLevel6IconComplete();
        }

        StartCoroutine(LoadAsync(sceneName));
    }

    private IEnumerator LoadAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public void ResetGameState()
    {
        if (StateManager.Instance != null)
        {
            StateManager.Instance.ResetState();
        }
    }

    public void StartCurrentLesson()
    {
        string targetLesson = "Lesson1";

        if (!StateManager.Lesson1Complete)
        {
            targetLesson = "Lesson1";
        }
        else if (!StateManager.Lesson2Complete)
        {
            targetLesson = "Lesson2";
        }
        else if (!StateManager.Lesson3Complete)
        {
            targetLesson = "Lesson3";
        }
        else if (!StateManager.Lesson4Complete)
        {
            targetLesson = "Lesson4";
        }
        else if (!StateManager.Lesson5Complete)
        {
            targetLesson = "Lesson5";
        }
        else if (!StateManager.Lesson6Complete)
        {
            targetLesson = "Lesson6";
        }
        else
        {
            targetLesson = "Lesson6";
        }

        Debug.Log($"[AutoRouter] Первый незавершенный урок: {targetLesson}. Запускаем...");
        LoadScene(targetLesson);
    }

    public void ExitApp()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_ANDROID
        try
        {
            using (var jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (var activity = jc.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    activity.Call("finishAndRemoveTask");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Ошибка при нативном закрытии приложения: " + e.Message);
            Application.Quit();
        }
#else
        Application.Quit();
#endif
    }
}