using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CustomSceneManager : MonoBehaviour
{
    // Статическая ссылка на единственный экземпляр
    private static CustomSceneManager instance;

    private static StateManager stateManager;

    private GameObject resetConfirmOverlay;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
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
        if (StateManager.Instance == null)
        {
            StartCoroutine(LoadAsync(sceneName));
            return;
        }

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

    /// <summary>
    /// Shows a confirmation dialog before wiping all progress (Обнулить счёт).
    /// </summary>
    public void RequestResetGameState()
    {
        if (resetConfirmOverlay != null)
            return;

        ShowResetConfirmDialog();
    }

    public void ResetGameState()
    {
        if (StateManager.Instance != null)
        {
            StateManager.Instance.ResetState();
        }
    }

    private void ShowResetConfirmDialog()
    {
        Canvas canvas = FindMainCanvas();
        if (canvas == null)
        {
            Debug.LogWarning("CustomSceneManager: MainCanvas not found, cannot show reset confirm.");
            return;
        }

        var teal = new Color(0.369f, 0.812f, 0.722f, 1f);
        var tealPressed = new Color(0.271f, 0.678f, 0.596f, 1f);
        var dim = new Color(0f, 0f, 0f, 0.65f);
        var panelBg = new Color(0.12f, 0.18f, 0.2f, 0.95f);

        resetConfirmOverlay = new GameObject("ResetConfirmOverlay", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        resetConfirmOverlay.transform.SetParent(canvas.transform, false);
        resetConfirmOverlay.transform.SetAsLastSibling();

        var overlayRect = resetConfirmOverlay.GetComponent<RectTransform>();
        StretchFull(overlayRect);
        var overlayImage = resetConfirmOverlay.GetComponent<Image>();
        overlayImage.color = dim;
        overlayImage.raycastTarget = true;

        var panel = new GameObject("Panel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panel.transform.SetParent(resetConfirmOverlay.transform, false);
        var panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(720f, 360f);
        panel.GetComponent<Image>().color = panelBg;

        var message = CreateUiText(panel.transform, "Message", "Сбросить весь прогресс?", 42);
        var messageRect = message.GetComponent<RectTransform>();
        messageRect.anchorMin = new Vector2(0.08f, 0.45f);
        messageRect.anchorMax = new Vector2(0.92f, 0.9f);
        messageRect.offsetMin = Vector2.zero;
        messageRect.offsetMax = Vector2.zero;
        message.alignment = TextAnchor.MiddleCenter;

        var yesBtn = CreateDialogButton(panel.transform, "YesButton", "Да", teal, tealPressed);
        var yesRect = yesBtn.GetComponent<RectTransform>();
        yesRect.anchorMin = new Vector2(0.08f, 0.12f);
        yesRect.anchorMax = new Vector2(0.46f, 0.38f);
        yesRect.offsetMin = Vector2.zero;
        yesRect.offsetMax = Vector2.zero;
        yesBtn.onClick.AddListener(ConfirmReset);

        var noBtn = CreateDialogButton(panel.transform, "NoButton", "Отмена",
            new Color(0.45f, 0.5f, 0.52f, 1f),
            new Color(0.35f, 0.4f, 0.42f, 1f));
        var noRect = noBtn.GetComponent<RectTransform>();
        noRect.anchorMin = new Vector2(0.54f, 0.12f);
        noRect.anchorMax = new Vector2(0.92f, 0.38f);
        noRect.offsetMin = Vector2.zero;
        noRect.offsetMax = Vector2.zero;
        noBtn.onClick.AddListener(DismissResetConfirm);
    }

    private void ConfirmReset()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayClickSound();

        ResetGameState();
        DismissResetConfirm();
    }

    private void DismissResetConfirm()
    {
        if (resetConfirmOverlay != null)
        {
            Destroy(resetConfirmOverlay);
            resetConfirmOverlay = null;
        }
    }

    private static Canvas FindMainCanvas()
    {
        foreach (var name in new[] { "MainCanvas", "Canvas" })
        {
            var go = GameObject.Find(name);
            if (go != null)
            {
                var c = go.GetComponent<Canvas>();
                if (c != null)
                    return c;
            }
        }

        return FindObjectOfType<Canvas>();
    }

    private static void StretchFull(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.localScale = Vector3.one;
    }

    private static Text CreateUiText(Transform parent, string name, string content, int fontSize)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        go.transform.SetParent(parent, false);
        var text = go.GetComponent<Text>();
        text.text = content;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (text.font == null)
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = fontSize;
        text.fontStyle = FontStyle.Bold;
        text.color = Color.white;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        return text;
    }

    private static Button CreateDialogButton(Transform parent, string name, string label, Color normal, Color pressed)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);

        var image = go.GetComponent<Image>();
        image.color = normal;

        var button = go.GetComponent<Button>();
        var colors = button.colors;
        colors.normalColor = normal;
        colors.highlightedColor = Color.Lerp(normal, Color.white, 0.2f);
        colors.pressedColor = pressed;
        colors.selectedColor = colors.highlightedColor;
        button.colors = colors;
        button.targetGraphic = image;

        var text = CreateUiText(go.transform, "Label", label, 36);
        var textRect = text.GetComponent<RectTransform>();
        StretchFull(textRect);
        text.alignment = TextAnchor.MiddleCenter;

        return button;
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
        return;
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
        // finishAndRemoveTask alone can leave the Unity process alive; the next launch
        // then skips splash screens and can hang on a dark/empty frame (GameActivity).
        // Kill the process after finishing so every launch is a clean cold start.
        try
        {
            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                if (activity != null)
                    activity.Call("finishAndRemoveTask");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Ошибка при finishAndRemoveTask: " + e.Message);
        }

        try
        {
            using (var process = new AndroidJavaClass("android.os.Process"))
            {
                int pid = process.CallStatic<int>("myPid");
                process.CallStatic("killProcess", pid);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Ошибка при killProcess: " + e.Message);
        }
#endif

        Application.Quit();
    }
}
