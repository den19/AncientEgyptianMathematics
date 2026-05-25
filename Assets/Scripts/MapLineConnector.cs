using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapLineConnector : MonoBehaviour
{
    [Header("Waypoints")]
    [Tooltip("The level markers on the map, in order (Lesson 1 to 6).")]
    public List<RectTransform> waypoints = new List<RectTransform>();

    [Header("Line Appearance")]
    [Tooltip("The sprite to use for the dots of the dashed line.")]
    public Sprite dotSprite;

    [Tooltip("Color of the dotted path.")]
    public Color pathColor = new Color(1f, 0.85f, 0.3f, 0.8f); // Золотистый полупрозрачный

    [Tooltip("Size of each dot.")]
    public float dotSize = 8f;

    [Tooltip("Spacing between dots in pixels.")]
    public float dotSpacing = 20f;

    [Header("Sizing and Parent")]
    [Tooltip("Parent GameObject for instantiated dots (to avoid clutter).")]
    public Transform container;

    private List<GameObject> activeDots = new List<GameObject>();

    private void Start()
    {
        GeneratePath();
    }

    private RectTransform pathRect;
    private Canvas rootCanvas;

    private void Awake()
    {
        pathRect = GetComponent<RectTransform>();
        rootCanvas = GetComponentInParent<Canvas>();
    }

    private Vector2 WaypointToLocalPosition(RectTransform waypoint)
    {
        if (pathRect == null || waypoint == null)
            return waypoint != null ? waypoint.anchoredPosition : Vector2.zero;

        var cam = rootCanvas != null && rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay
            ? rootCanvas.worldCamera
            : null;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            pathRect,
            RectTransformUtility.WorldToScreenPoint(cam, waypoint.position),
            cam,
            out Vector2 localPoint);

        return localPoint;
    }

    [ContextMenu("Generate Map Path")]
    public void GeneratePath()
    {
        // Очищаем старые точки перед генерацией
        ClearPath();

        if (waypoints == null || waypoints.Count < 2) return;

        if (pathRect == null)
            pathRect = GetComponent<RectTransform>();

        if (rootCanvas == null)
            rootCanvas = GetComponentInParent<Canvas>();

        Transform dotsParent = container != null ? container : transform;

        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            RectTransform start = waypoints[i];
            RectTransform end = waypoints[i + 1];

            if (start == null || end == null) continue;

            Vector2 startPos = WaypointToLocalPosition(start);
            Vector2 endPos = WaypointToLocalPosition(end);

            float distance = Vector2.Distance(startPos, endPos);
            int dotsToCreate = Mathf.Max(1, Mathf.FloorToInt(distance / dotSpacing));

            for (int d = 1; d < dotsToCreate; d++)
            {
                float t = (float)d / dotsToCreate;
                Vector2 spawnPos = Vector2.Lerp(startPos, endPos, t);

                // Создаем UI-объект точки
                GameObject dotObj = new GameObject($"Dot_{i}_{d}", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                dotObj.transform.SetParent(dotsParent, false);

                RectTransform rect = dotObj.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(dotSize, dotSize);
                rect.anchoredPosition = spawnPos;

                Image img = dotObj.GetComponent<Image>();
                img.sprite = dotSprite;
                img.color = pathColor;

                // Дополнительно можно повесить на каждую четную точку легкий эффект мерцания
                if (d % 2 == 0)
                {
                    PulsingUI pulsar = dotObj.AddComponent<PulsingUI>();
                    pulsar.pulseSpeed = 2f;
                    pulsar.scaleAmount = 0.15f;
                    pulsar.phaseOffset = d * 0.5f; // Смещение фазы для волны мерцания
                }

                activeDots.Add(dotObj);
            }
        }
    }

    public void ClearPath()
    {
        foreach (var dot in activeDots)
        {
            if (dot != null)
            {
                if (Application.isPlaying)
                    Destroy(dot);
                else
                    DestroyImmediate(dot);
            }
        }
        activeDots.Clear();

        if (container != null)
        {
            while (container.childCount > 0)
            {
                DestroyImmediate(container.GetChild(0).gameObject);
            }
        }
    }
}
