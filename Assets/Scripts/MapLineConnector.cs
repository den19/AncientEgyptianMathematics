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

    [Tooltip("Color of the active/unlocked path.")]
    public Color pathColor = new Color(1f, 0.85f, 0.3f, 0.85f); // Золотистый насыщенный

    [Tooltip("Color of the locked/inactive path.")]
    public Color lockedPathColor = new Color(0.65f, 0.65f, 0.65f, 0.45f); // Тусклый серый/полупрозрачный

    [Tooltip("Size of each dot.")]
    public float dotSize = 10f;

    [Tooltip("Spacing between dots in pixels.")]
    public float dotSpacing = 24f;

    [Header("Curve Settings")]
    [Tooltip("How much the path curves dynamically around the Nile river.")]
    public float curveAmount = 45f;

    [Header("Sizing and Parent")]
    [Tooltip("Parent GameObject for instantiated dots (to avoid clutter).")]
    public Transform container;

    private List<GameObject> activeDots = new List<GameObject>();
    private RectTransform pathRect;
    private Canvas rootCanvas;

    private void Start()
    {
        GeneratePath();
    }

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

    private bool IsSegmentUnlocked(int segmentIndex)
    {
        // Сегмент i соединяет Урок i+1 и Урок i+2.
        // Он считается открытым (золотым), если Урок i+1 уже пройден.
        // Если StateManager не инициализирован, по умолчанию считаем открытым.
        if (StateManager.Instance == null) return true;

        switch (segmentIndex)
        {
            case 0: return StateManager.Lesson1Complete; // Путь от 1 к 2
            case 1: return StateManager.Lesson2Complete; // Путь от 2 к 3
            case 2: return StateManager.Lesson3Complete; // Путь от 3 к 4
            case 3: return StateManager.Lesson4Complete; // Путь от 4 к 5
            case 4: return StateManager.Lesson5Complete; // Путь от 5 к 6
            default: return false;
        }
    }

    [ContextMenu("Generate Map Path")]
    public void GeneratePath()
    {
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

            // Определяем, открыт ли этот сегмент игроком
            bool isUnlocked = IsSegmentUnlocked(i);
            Color segmentColor = isUnlocked ? pathColor : lockedPathColor;

            // Расчет плавной кривой Безье (альтернируем кривизну влево/вправо для красоты)
            Vector2 dir = endPos - startPos;
            Vector2 perp = new Vector2(-dir.y, dir.x).normalized;
            float sideOffset = ((i % 2 == 0) ? 1f : -1f) * curveAmount;
            Vector2 controlPos = (startPos + endPos) * 0.5f + perp * sideOffset;

            // Приблизительная длина кривой Безье для расчета шага точек
            float chord = dir.magnitude;
            float controlHeight = Vector2.Distance((startPos + endPos) * 0.5f, controlPos);
            float estimatedLength = chord + (controlHeight * 0.5f); // простая аппроксимация

            int dotsToCreate = Mathf.Max(1, Mathf.FloorToInt(estimatedLength / dotSpacing));

            for (int d = 1; d < dotsToCreate; d++)
            {
                float t = (float)d / dotsToCreate;
                
                // Формула квадратичной кривой Безье
                Vector2 spawnPos = (1f - t) * (1f - t) * startPos + 2f * (1f - t) * t * controlPos + t * t * endPos;

                // Создаем UI-объект точки
                GameObject dotObj = new GameObject($"Dot_{i}_{d}", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                dotObj.transform.SetParent(dotsParent, false);

                RectTransform rect = dotObj.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(dotSize, dotSize);
                rect.anchoredPosition = spawnPos;

                Image img = dotObj.GetComponent<Image>();
                img.sprite = dotSprite;
                img.color = segmentColor;

                // Если сегмент разблокирован, добавляем анимацию волшебного мерцания и дыхания
                if (isUnlocked)
                {
                    PulsingUI pulsar = dotObj.AddComponent<PulsingUI>();
                    // Смещаем фазу в зависимости от позиции, чтобы огоньки "бежали" волной
                    pulsar.pulseSpeed = 2.5f;
                    pulsar.scaleAmount = 0.18f;
                    pulsar.phaseOffset = (i * dotsToCreate + d) * 0.3f;
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
