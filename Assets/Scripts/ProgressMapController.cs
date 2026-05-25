using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Builds the interactive Nile map on the Progress scene at runtime without breaking StateManager icon logic.
/// </summary>
[DefaultExecutionOrder(100)]
public class ProgressMapController : MonoBehaviour
{
    private static readonly string[] LessonOrder = { "Lesson1", "Lesson2", "Lesson3", "Lesson4", "Lesson5", "Lesson6" };
    private static readonly string[] LessonTitles =
    {
        "Урок I: Обитель писцов",
        "Урок II: Разлив Нила",
        "Урок III: Шадуф и каналы",
        "Урок IV: Звезда Сириус",
        "Урок V: Математические свитки",
        "Урок VI: Дворец фараона"
    };

    private bool mapBuilt;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void BootstrapOnProgressScene()
    {
        var scene = SceneManager.GetActiveScene();
        if (!scene.IsValid() || scene.name != "Progress")
            return;

        if (FindObjectOfType<ProgressMapController>() != null)
            return;

        var bootstrap = new GameObject("ProgressMapController");
        bootstrap.AddComponent<ProgressMapController>();
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name != "Progress")
        {
            Destroy(gameObject);
            return;
        }

        StartCoroutine(BuildMapWhenReady());
    }

    private IEnumerator BuildMapWhenReady()
    {
        if (mapBuilt)
            yield break;

        // Ждём инициализацию StateManager и раскладку UI
        for (int i = 0; i < 5; i++)
            yield return null;

        if (StateManager.Instance == null)
        {
            var prefab = Resources.Load<GameObject>("StateManager");
            if (prefab != null)
                Instantiate(prefab);
        }

        yield return null;

        BuildMap();
        mapBuilt = true;
    }

    private void BuildMap()
    {
        var content = FindScrollContent();
        if (content == null)
        {
            Debug.LogWarning("ProgressMapController: Content not found, map setup skipped.");
            return;
        }

        ApplyMapBackground(content);
        var pathRoot = CreatePathRoot(content);
        var nile = CreateNileRiver(content);
        if (nile != null)
            nile.transform.SetSiblingIndex(1);

        var waypoints = CollectLessonWaypoints();
        if (waypoints.Count == 0)
        {
            Debug.LogWarning("ProgressMapController: lesson waypoints not found.");
            return;
        }

        ApplyLandmarks(waypoints);
        ApplyFloatingToMarkers(waypoints);
        BuildGoldenPath(pathRoot, waypoints);

        if (StateManager.Instance != null)
        {
            StateManager.Instance.FindUIElements();
            StateManager.Instance.RefreshProgressMarkers();
        }

        ApplyPulsingToCurrentIcon();
    }

    private static RectTransform FindScrollContent()
    {
        var contentGo = GameObject.Find("Content");
        if (contentGo != null)
            return contentGo.GetComponent<RectTransform>();

        var all = FindObjectsOfType<RectTransform>(true);
        foreach (var rt in all)
        {
            if (rt.name == "Content")
                return rt;
        }

        return null;
    }

    private void ApplyMapBackground(RectTransform content)
    {
        var sprite = ProgressMapAssetLoader.MapBackground ?? ProgressMapAssetLoader.MapBackgroundAlt;
        if (sprite == null)
            return;

        // Фон прокручиваемой области
        var bg = content.GetComponent<Image>();
        if (bg == null)
            bg = content.gameObject.AddComponent<Image>();

        bg.sprite = sprite;
        bg.type = Image.Type.Simple;
        bg.preserveAspect = false;
        bg.color = new Color(0.95f, 0.9f, 0.78f, 1f);
        bg.raycastTarget = false;

        // Дополнительный фон экрана
        var canvases = FindObjectsOfType<Image>(true);
        foreach (var img in canvases)
        {
            if (img.gameObject.name == "Background_canvas" || img.gameObject.name == "Background")
            {
                img.sprite = sprite;
                img.type = Image.Type.Simple;
                img.preserveAspect = true;
                img.color = new Color(0.85f, 0.8f, 0.7f, 1f);
                img.raycastTarget = false;
            }
        }
    }

    private static RectTransform CreatePathRoot(RectTransform content)
    {
        var existing = content.Find("MapPath");
        if (existing != null)
            Destroy(existing.gameObject);

        var pathRoot = new GameObject("MapPath", typeof(RectTransform));
        pathRoot.transform.SetParent(content, false);

        var rt = pathRoot.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        rt.SetAsFirstSibling();
        return rt;
    }

    private static GameObject CreateNileRiver(RectTransform content)
    {
        var tex = ProgressMapAssetLoader.NileWaterTexture;
        if (tex == null)
            return null;

        var existing = content.Find("NileRiver");
        if (existing != null)
            Destroy(existing.gameObject);

        var riverGo = new GameObject("NileRiver", typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage), typeof(WaterEffectUI));
        riverGo.transform.SetParent(content, false);

        var rt = riverGo.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.35f, 0f);
        rt.anchorMax = new Vector2(0.65f, 1f);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        rt.pivot = new Vector2(0.5f, 0.5f);

        var raw = riverGo.GetComponent<RawImage>();
        raw.texture = tex;
        raw.color = new Color(0.55f, 0.75f, 0.95f, 0.45f);
        raw.raycastTarget = false;

        var water = riverGo.GetComponent<WaterEffectUI>();
        water.scrollSpeedX = 0.02f;
        water.scrollSpeedY = 0.04f;

        return riverGo;
    }

    private struct LessonWaypoint
    {
        public int Index;
        public RectTransform Marker;
        public RectTransform Icon;
    }

    private List<LessonWaypoint> CollectLessonWaypoints()
    {
        var list = new List<LessonWaypoint>();

        for (int i = 0; i < LessonOrder.Length; i++)
        {
            int lessonNum = i + 1;
            var iconGo = GameObject.Find($"Level{lessonNum}Icon");
            if (iconGo == null)
                continue;

            var iconRt = iconGo.GetComponent<RectTransform>();
            var marker = iconRt.parent as RectTransform;
            if (marker == null)
                continue;

            list.Add(new LessonWaypoint
            {
                Index = lessonNum,
                Marker = marker,
                Icon = iconRt
            });
        }

        list.Sort((a, b) => a.Index.CompareTo(b.Index));
        return list;
    }

    private void ApplyLandmarks(List<LessonWaypoint> waypoints)
    {
        foreach (var wp in waypoints)
        {
            var sprite = ProgressMapAssetLoader.LessonLandmark(wp.Index);
            if (sprite == null)
            {
                Debug.LogWarning($"ProgressMapController: Landmark sprite for Lesson {wp.Index} is null!");
                continue;
            }

            Image targetImage = null;

            // 1. Попробуем найти фоновое изображение кнопки ("Background_small")
            var bgTr = wp.Marker.Find("Background_small");
            if (bgTr != null)
            {
                targetImage = bgTr.GetComponent<Image>();
            }

            // 2. Если фоновое изображение не найдено, создаем Landmark дочерний объект
            if (targetImage == null)
            {
                var landmarkTr = wp.Marker.Find("Landmark");
                if (landmarkTr == null)
                {
                    var landmarkGo = new GameObject("Landmark", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                    landmarkGo.transform.SetParent(wp.Marker, false);
                    landmarkGo.transform.SetAsFirstSibling();

                    var rt = landmarkGo.GetComponent<RectTransform>();
                    rt.anchorMin = new Vector2(0.5f, 0.5f);
                    rt.anchorMax = new Vector2(0.5f, 0.5f);
                    rt.pivot = new Vector2(0.5f, 0.5f);
                    rt.sizeDelta = new Vector2(140f, 140f);
                    rt.anchoredPosition = Vector2.zero;

                    targetImage = landmarkGo.GetComponent<Image>();
                }
                else
                {
                    targetImage = landmarkTr.GetComponent<Image>();
                }
            }

            if (targetImage == null)
                continue;

            targetImage.sprite = sprite;
            targetImage.preserveAspect = false; // Растягиваем, чтобы заполнить кнопку красиво
            targetImage.type = Image.Type.Simple;
            targetImage.raycastTarget = true; // Кликабельно, так как это фон

            bool complete = IsLessonComplete(wp.Index);
            targetImage.color = complete
                ? Color.white
                : new Color(0.75f, 0.75f, 0.75f, 1f); // Немного затемняем заблокированные уроки
        }
    }

    private static bool IsLessonComplete(int lessonIndex)
    {
        switch (lessonIndex)
        {
            case 1: return StateManager.Lesson1Complete;
            case 2: return StateManager.Lesson2Complete;
            case 3: return StateManager.Lesson3Complete;
            case 4: return StateManager.Lesson4Complete;
            case 5: return StateManager.Lesson5Complete;
            case 6: return StateManager.Lesson6Complete;
            default: return false;
        }
    }

    private static void ApplyFloatingToMarkers(List<LessonWaypoint> waypoints)
    {
        float phase = 0f;
        foreach (var wp in waypoints)
        {
            if (wp.Marker.GetComponent<FloatingUI>() == null)
            {
                var floating = wp.Marker.gameObject.AddComponent<FloatingUI>();
                floating.floatSpeed = 1.2f;
                floating.floatAmplitude = 6f;
                floating.phaseOffset = phase;
            }

            phase += 0.9f;
        }
    }

    private void BuildGoldenPath(RectTransform pathRoot, List<LessonWaypoint> waypoints)
    {
        var connector = pathRoot.gameObject.GetComponent<MapLineConnector>();
        if (connector == null)
            connector = pathRoot.gameObject.AddComponent<MapLineConnector>();

        connector.container = pathRoot;
        connector.waypoints.Clear();

        foreach (var wp in waypoints)
            connector.waypoints.Add(wp.Marker);

        connector.dotSprite = CreateDotSprite();
        connector.pathColor = new Color(1f, 0.82f, 0.25f, 0.75f);
        connector.dotSize = 10f;
        connector.dotSpacing = 24f;
        connector.GeneratePath();
    }

    private static Sprite CreateDotSprite()
    {
        const int size = 16;
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;

        float center = (size - 1) * 0.5f;
        float radius = size * 0.35f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = x - center;
                float dy = y - center;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);
                float alpha = dist <= radius ? 1f : 0f;
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }

        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
    }

    private static void ApplyPulsingToCurrentIcon()
    {
        for (int i = 1; i <= 6; i++)
        {
            var icon = GameObject.Find($"Level{i}Icon");
            if (icon == null)
                continue;

            var pulsing = icon.GetComponent<PulsingUI>();
            if (pulsing != null)
                Destroy(pulsing);
        }

        int current = GetCurrentLessonIndex();
        if (current < 1)
            return;

        var currentIcon = GameObject.Find($"Level{current}Icon");
        if (currentIcon == null)
            return;

        var img = currentIcon.GetComponent<Image>();
        if (img == null || !img.enabled || img.color.a < 0.5f)
            return;

        var pulse = currentIcon.AddComponent<PulsingUI>();
        pulse.pulseSpeed = 2.5f;
        pulse.scaleAmount = 0.14f;
    }

    private static int GetCurrentLessonIndex()
    {
        bool[] done =
        {
            StateManager.Lesson1Complete,
            StateManager.Lesson2Complete,
            StateManager.Lesson3Complete,
            StateManager.Lesson4Complete,
            StateManager.Lesson5Complete,
            StateManager.Lesson6Complete
        };

        for (int i = 0; i < done.Length; i++)
        {
            if (!done[i])
                return i + 1;
        }

        return 0;
    }
}
