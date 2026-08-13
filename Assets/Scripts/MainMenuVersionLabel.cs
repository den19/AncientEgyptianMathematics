using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Shows Application.version at the bottom of MainMenu using an Itten-inspired
/// warm/cool palette (sand/gold + nile teal). Fixed Constant Pixel Size layout.
/// </summary>
public sealed class MainMenuVersionLabel : MonoBehaviour
{
    const string HostObjectName = "MainMenuVersionHost";
    const string LabelObjectName = "VersionLabel";
    const string SceneName = "MainMenu";

    // Fixed layout — same Constant Pixel Size approach as menu buttons / settings chips.
    const float BandHeight = 100f;
    const float BandY = 24f;
    const int FontSize = 48;

    // Itten warm–cool harmony (matches marketing posters for this app).
    static readonly Color Gold = new Color(1f, 0.92f, 0.55f, 1f);
    static readonly Color Nile = new Color(74f / 255f, 168f / 255f, 170f / 255f, 0.85f);
    static readonly Color Ink = new Color(48f / 255f, 36f / 255f, 26f / 255f, 1f);
    static readonly Color SandBand = new Color(36f / 255f, 28f / 255f, 20f / 255f, 0.55f);

    static Sprite s_WhiteSprite;
    bool built;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void RegisterSceneHook()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AfterSceneLoad()
    {
        TrySpawn();
    }

    static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != SceneName)
            return;
        TrySpawn();
    }

    static void TrySpawn()
    {
        if (SceneManager.GetActiveScene().name != SceneName)
            return;

        if (Object.FindFirstObjectByType<MainMenuVersionLabel>() != null)
            return;

        var canvas = FindActiveSceneCanvas();
        if (canvas == null)
            return;

        var host = new GameObject(HostObjectName, typeof(RectTransform), typeof(MainMenuVersionLabel));
        host.layer = canvas.gameObject.layer;
        host.transform.SetParent(canvas.transform, false);
        host.transform.SetAsLastSibling();
    }

    static Canvas FindActiveSceneCanvas()
    {
        var scene = SceneManager.GetActiveScene();
        if (!scene.IsValid())
            return null;

        foreach (var root in scene.GetRootGameObjects())
        {
            if (root.name == "Canvas")
            {
                var named = root.GetComponent<Canvas>();
                if (named != null)
                    return named;
            }
        }

        foreach (var root in scene.GetRootGameObjects())
        {
            var canvas = root.GetComponentInChildren<Canvas>(true);
            if (canvas != null)
                return canvas;
        }

        return null;
    }

    void OnEnable()
    {
        if (!built)
            BuildOnce();
        else
            transform.SetAsLastSibling();
    }

    void BuildOnce()
    {
        var rect = GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 0f);
        rect.anchorMax = new Vector2(1f, 0f);
        rect.pivot = new Vector2(0.5f, 0f);
        rect.anchoredPosition = new Vector2(0f, BandY);
        rect.sizeDelta = new Vector2(0f, BandHeight);
        rect.localScale = Vector3.one;
        rect.localRotation = Quaternion.identity;

        var band = GetComponent<Image>();
        if (band == null)
            band = gameObject.AddComponent<Image>();
        band.sprite = GetWhiteSprite();
        band.type = Image.Type.Simple;
        band.color = SandBand;
        band.raycastTarget = false;

        var text = EnsureText();
        text.text = "v" + Application.version;
        text.fontSize = FontSize;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Gold;
        text.raycastTarget = false;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        text.resizeTextForBestFit = false;

        var outline = text.GetComponent<Outline>();
        if (outline == null)
            outline = text.gameObject.AddComponent<Outline>();
        outline.effectColor = Ink;
        outline.effectDistance = new Vector2(2.2f, -2.2f);
        outline.useGraphicAlpha = true;

        var accent = text.GetComponent<Shadow>();
        if (accent == null)
            accent = text.gameObject.AddComponent<Shadow>();
        accent.effectColor = Nile;
        accent.effectDistance = new Vector2(0f, 2.5f);
        accent.useGraphicAlpha = true;

        transform.SetAsLastSibling();
        built = true;
    }

    Text EnsureText()
    {
        var existing = transform.Find(LabelObjectName);
        if (existing != null)
        {
            var t = existing.GetComponent<Text>();
            if (t != null)
            {
                var existingRect = existing.GetComponent<RectTransform>();
                existingRect.anchorMin = Vector2.zero;
                existingRect.anchorMax = Vector2.one;
                existingRect.offsetMin = new Vector2(16f, 10f);
                existingRect.offsetMax = new Vector2(-16f, -10f);
                return t;
            }
        }

        var go = new GameObject(LabelObjectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        go.layer = gameObject.layer;
        go.transform.SetParent(transform, false);

        var labelRect = go.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = new Vector2(16f, 10f);
        labelRect.offsetMax = new Vector2(-16f, -10f);
        labelRect.pivot = new Vector2(0.5f, 0.5f);

        var text = go.GetComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (text.font == null)
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        return text;
    }

    static Sprite GetWhiteSprite()
    {
        if (s_WhiteSprite != null)
            return s_WhiteSprite;

        var tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        tex.name = "VersionLabelWhite";
        tex.hideFlags = HideFlags.HideAndDontSave;
        tex.SetPixel(0, 0, Color.white);
        tex.Apply(false, false);
        s_WhiteSprite = Sprite.Create(tex, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 100f);
        s_WhiteSprite.name = "VersionLabelWhiteSprite";
        s_WhiteSprite.hideFlags = HideFlags.HideAndDontSave;
        return s_WhiteSprite;
    }
}
