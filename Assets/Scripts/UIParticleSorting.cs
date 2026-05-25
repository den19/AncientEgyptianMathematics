using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class UIParticleSorting : MonoBehaviour
{
    [Header("Sorting Settings")]
    [Tooltip("The sorting order for this Particle System inside the UI Canvas.")]
    public int sortingOrder = 30;

    [Tooltip("The sorting layer name (usually 'UI' or 'Default').")]
    public string sortingLayerName = "UI";

    private void Start()
    {
        // Задаем слой UI (Layer 5) для всего объекта и его детей
        SetLayerRecursive(gameObject, 5);

        // Настраиваем все системы частиц на масштабирование по иерархии (Hierarchy),
        // чтобы они корректно увеличивались вместе с кнопками UI Canvas.
        var allParticleSystems = GetComponentsInChildren<ParticleSystem>(true);
        foreach (var ps in allParticleSystems)
        {
            var main = ps.main;
            main.scalingMode = ParticleSystemScalingMode.Hierarchy;

            // Добавляем компонент Canvas для принудительного рендеринга в Overlay UI
            var canvas = ps.gameObject.GetComponent<Canvas>();
            if (canvas == null)
            {
                canvas = ps.gameObject.AddComponent<Canvas>();
            }
            canvas.overrideSorting = true;
            canvas.sortingLayerName = sortingLayerName;
            canvas.sortingOrder = sortingOrder;
        }

        // Настраиваем рендерер частиц
        var sysRenderer = GetComponent<ParticleSystemRenderer>();
        if (sysRenderer != null)
        {
            sysRenderer.sortingLayerName = sortingLayerName;
            sysRenderer.sortingOrder = sortingOrder;
        }

        // Поддержка для вложенных систем частиц
        var childRenderers = GetComponentsInChildren<ParticleSystemRenderer>(true);
        foreach (var r in childRenderers)
        {
            r.sortingLayerName = sortingLayerName;
            r.sortingOrder = sortingOrder;
        }

        Debug.Log($"[UIParticleSorting] Системы частиц ({allParticleSystems.Length}) успешно интегрированы в UI Canvas с сортировкой {sortingOrder}");
    }

    private void SetLayerRecursive(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursive(child.gameObject, layer);
        }
    }
}
