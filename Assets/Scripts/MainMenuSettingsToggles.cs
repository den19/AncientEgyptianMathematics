using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Child-friendly Sound / VFX on-off chips at the bottom of MainMenu,
/// above the version band. Fixed Constant Pixel Size layout (matches menu buttons).
/// </summary>
public sealed class MainMenuSettingsToggles : MonoBehaviour
{
    const string HostObjectName = "MainMenuSettingsHost";
    const string SceneName = "MainMenu";
    const string VersionHostName = "MainMenuVersionHost";

    // Fixed layout — same Constant Pixel Size approach as btnStart / btnExit / btnOptions.
    const float ChipHeight = 120f;
    const float ChipY = 160f; // above version band on bottom-anchored host
    const float SidePad = 40f;
    const float Gap = 28f;
    const int FontSize = 44;

    static readonly Color GoldOn = new Color(1f, 0.72f, 0.3f, 1f);
    static readonly Color NileOn = new Color(94f / 255f, 207f / 255f, 184f / 255f, 1f);
    static readonly Color OffBg = new Color(72f / 255f, 58f / 255f, 46f / 255f, 0.75f);
    static readonly Color Ink = new Color(48f / 255f, 36f / 255f, 26f / 255f, 1f);
    static readonly Color LabelOn = new Color(0.15f, 0.1f, 0.06f, 1f);
    static readonly Color LabelOff = new Color(0.92f, 0.86f, 0.72f, 1f);

    static Sprite s_RoundedSprite;

    Button soundButton;
    Button vfxButton;
    Text soundLabel;
    Text vfxLabel;
    Image soundBg;
    Image vfxBg;
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

        if (Object.FindFirstObjectByType<MainMenuSettingsToggles>() != null)
            return;

        var canvas = FindActiveSceneCanvas();
        if (canvas == null)
            return;

        var host = new GameObject(HostObjectName, typeof(RectTransform), typeof(MainMenuSettingsToggles));
        host.layer = canvas.gameObject.layer;
        host.transform.SetParent(canvas.transform, false);

        var version = canvas.transform.Find(VersionHostName);
        if (version != null)
            host.transform.SetSiblingIndex(version.GetSiblingIndex());
        else
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
            RefreshVisuals();
    }

    void BuildOnce()
    {
        var rect = GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 0f);
        rect.anchorMax = new Vector2(1f, 0f);
        rect.pivot = new Vector2(0.5f, 0f);
        rect.anchoredPosition = new Vector2(0f, ChipY);
        rect.sizeDelta = new Vector2(0f, ChipHeight);
        rect.localScale = Vector3.one;
        rect.localRotation = Quaternion.identity;

        soundButton = CreateChip("SoundToggle", out soundBg, out soundLabel);
        soundButton.onClick.AddListener(ToggleSound);
        LayoutChip(soundButton.GetComponent<RectTransform>(), 0f, 0.5f, SidePad, Gap * 0.5f);

        vfxButton = CreateChip("VfxToggle", out vfxBg, out vfxLabel);
        vfxButton.onClick.AddListener(ToggleVfx);
        LayoutChip(vfxButton.GetComponent<RectTransform>(), 0.5f, 1f, Gap * 0.5f, SidePad);

        soundLabel.fontSize = FontSize;
        vfxLabel.fontSize = FontSize;
        StretchLabel(soundLabel.rectTransform);
        StretchLabel(vfxLabel.rectTransform);

        built = true;
        RefreshVisuals();
    }

    static void LayoutChip(RectTransform chip, float anchorMinX, float anchorMaxX, float left, float right)
    {
        chip.anchorMin = new Vector2(anchorMinX, 0f);
        chip.anchorMax = new Vector2(anchorMaxX, 1f);
        chip.offsetMin = new Vector2(left, 0f);
        chip.offsetMax = new Vector2(-right, 0f);
        chip.pivot = new Vector2(0.5f, 0.5f);
    }

    static void StretchLabel(RectTransform labelRect)
    {
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = new Vector2(12f, 8f);
        labelRect.offsetMax = new Vector2(-12f, -8f);
    }

    Button CreateChip(string name, out Image bg, out Text label)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        go.layer = gameObject.layer;
        go.transform.SetParent(transform, false);

        bg = go.GetComponent<Image>();
        bg.sprite = GetRoundedSprite();
        bg.type = Image.Type.Sliced;
        bg.pixelsPerUnitMultiplier = 1f;
        bg.raycastTarget = true;

        var button = go.GetComponent<Button>();
        button.targetGraphic = bg;
        button.transition = Selectable.Transition.None;

        var labelGo = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        labelGo.layer = gameObject.layer;
        labelGo.transform.SetParent(go.transform, false);

        label = labelGo.GetComponent<Text>();
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (label.font == null)
            label.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        label.fontStyle = FontStyle.Bold;
        label.alignment = TextAnchor.MiddleCenter;
        label.horizontalOverflow = HorizontalWrapMode.Overflow;
        label.verticalOverflow = VerticalWrapMode.Overflow;
        label.raycastTarget = false;

        var outline = labelGo.AddComponent<Outline>();
        outline.effectColor = Ink;
        outline.effectDistance = new Vector2(1.8f, -1.8f);
        outline.useGraphicAlpha = true;

        return button;
    }

    void ToggleSound()
    {
        bool next = !GameSettings.SoundEnabled;
        if (!next)
        {
            if (SoundManager.Instance != null)
                SoundManager.Instance.PlayClickSound();
            GameSettings.SetSoundEnabled(false);
        }
        else
        {
            GameSettings.SetSoundEnabled(true);
            if (SoundManager.Instance != null)
                SoundManager.Instance.PlayClickSound();
        }

        RefreshVisuals();
    }

    void ToggleVfx()
    {
        GameSettings.SetVfxEnabled(!GameSettings.VfxEnabled);
        if (GameSettings.SoundEnabled && SoundManager.Instance != null)
            SoundManager.Instance.PlayClickSound();
        RefreshVisuals();
    }

    void RefreshVisuals()
    {
        if (soundLabel == null || vfxLabel == null)
            return;

        bool soundOn = GameSettings.SoundEnabled;
        bool vfxOn = GameSettings.VfxEnabled;

        soundBg.color = soundOn ? GoldOn : OffBg;
        vfxBg.color = vfxOn ? NileOn : OffBg;

        soundLabel.text = soundOn ? "Звук  Вкл" : "Звук  Выкл";
        vfxLabel.text = vfxOn ? "Эффекты  Вкл" : "Эффекты  Выкл";
        soundLabel.color = soundOn ? LabelOn : LabelOff;
        vfxLabel.color = vfxOn ? LabelOn : LabelOff;
    }

    static Sprite GetRoundedSprite()
    {
        if (s_RoundedSprite != null)
            return s_RoundedSprite;

        const int size = 64;
        const int radius = 20;
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.name = "SettingsChipRounded";
        tex.hideFlags = HideFlags.HideAndDontSave;
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Bilinear;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float alpha = CornerAlpha(x, y, size, radius);
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }

        tex.Apply(false, false);
        s_RoundedSprite = Sprite.Create(
            tex,
            new Rect(0f, 0f, size, size),
            new Vector2(0.5f, 0.5f),
            100f,
            0,
            SpriteMeshType.FullRect,
            new Vector4(radius, radius, radius, radius));
        s_RoundedSprite.name = "SettingsChipRoundedSprite";
        s_RoundedSprite.hideFlags = HideFlags.HideAndDontSave;
        return s_RoundedSprite;
    }

    static float CornerAlpha(int x, int y, int size, int radius)
    {
        int max = size - 1;
        Vector2 center;
        if (x < radius && y < radius)
            center = new Vector2(radius, radius);
        else if (x > max - radius && y < radius)
            center = new Vector2(max - radius, radius);
        else if (x < radius && y > max - radius)
            center = new Vector2(radius, max - radius);
        else if (x > max - radius && y > max - radius)
            center = new Vector2(max - radius, max - radius);
        else
            return 1f;

        float dist = Vector2.Distance(new Vector2(x, y), center);
        return dist <= radius - 0.5f ? 1f : (dist <= radius + 0.5f ? 0.5f : 0f);
    }
}
