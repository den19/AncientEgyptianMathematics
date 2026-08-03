using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Shows Application.version at the bottom of MainMenu using an Itten-inspired
/// warm/cool palette (sand/gold + nile teal), soft contrast without pure black.
/// </summary>
public sealed class MainMenuVersionLabel : MonoBehaviour
{
    const string HostObjectName = "MainMenuVersionHost";
    const string LabelObjectName = "VersionLabel";
    const string SceneName = "MainMenu";

    // Itten warm–cool harmony (matches marketing posters for this app).
    static readonly Color GoldSoft = new Color(255f / 255f, 228f / 255f, 150f / 255f, 1f);
    static readonly Color Nile = new Color(74f / 255f, 168f / 255f, 170f / 255f, 0.72f);
    static readonly Color InkSoft = new Color(62f / 255f, 48f / 255f, 36f / 255f, 0.95f);
    static readonly Color SandBand = new Color(245f / 255f, 230f / 255f, 200f / 255f, 0.38f);

    static Sprite s_WhiteSprite;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AfterSceneLoad()
    {
        if (SceneManager.GetActiveScene().name != SceneName)
            return;

        if (Object.FindFirstObjectByType<MainMenuVersionLabel>() != null)
            return;

        var canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null)
            return;

        var host = new GameObject(HostObjectName, typeof(RectTransform), typeof(MainMenuVersionLabel));
        host.layer = canvas.gameObject.layer;
        host.transform.SetParent(canvas.transform, false);
        host.transform.SetAsLastSibling();
    }

    void Start()
    {
        EnsureLabel();
    }

    void EnsureLabel()
    {
        var canvasRoot = transform.parent as RectTransform;
        if (canvasRoot == null)
            return;

        var rect = GetComponent<RectTransform>();
        float bottomInset = ResolveBottomSafeInset(canvasRoot);

        rect.anchorMin = new Vector2(0f, 0f);
        rect.anchorMax = new Vector2(1f, 0f);
        rect.pivot = new Vector2(0.5f, 0f);
        rect.anchoredPosition = new Vector2(0f, bottomInset);
        rect.sizeDelta = new Vector2(0f, 84f);
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
        text.fontSize = 42;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = GoldSoft;
        text.raycastTarget = false;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow = VerticalWrapMode.Overflow;

        // Warm outline + cool accent for stronger light–dark contrast (Itten).
        var outline = text.GetComponent<Outline>();
        if (outline == null)
            outline = text.gameObject.AddComponent<Outline>();
        outline.effectColor = InkSoft;
        outline.effectDistance = new Vector2(1.8f, -1.8f);
        outline.useGraphicAlpha = true;

        var accent = text.GetComponent<Shadow>();
        if (accent == null)
            accent = text.gameObject.AddComponent<Shadow>();
        accent.effectColor = Nile;
        accent.effectDistance = new Vector2(0f, 2.2f);
        accent.useGraphicAlpha = true;
    }

    Text EnsureText()
    {
        var existing = transform.Find(LabelObjectName);
        if (existing != null)
        {
            var t = existing.GetComponent<Text>();
            if (t != null)
                return t;
        }

        var go = new GameObject(LabelObjectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        go.layer = gameObject.layer;
        go.transform.SetParent(transform, false);

        var labelRect = go.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = new Vector2(24f, 6f);
        labelRect.offsetMax = new Vector2(-24f, -6f);
        labelRect.pivot = new Vector2(0.5f, 0.5f);

        var text = go.GetComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (text.font == null)
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        return text;
    }

    static float ResolveBottomSafeInset(RectTransform canvasRoot)
    {
        if (canvasRoot == null || Screen.height <= 0)
            return 18f;

        float canvasHeight = canvasRoot.rect.height;
        if (canvasHeight <= 1f)
            return 18f;

        Rect safe = Screen.safeArea;
        float bottomPixels = Mathf.Max(0f, safe.yMin);
        float inset = bottomPixels * (canvasHeight / Screen.height);
        return Mathf.Max(18f, inset + 8f);
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
