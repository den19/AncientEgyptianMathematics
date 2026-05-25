using UnityEngine;

/// <summary>
/// Loads map textures for Progress scene (editor paths + Resources fallback for builds).
/// </summary>
public static class ProgressMapAssetLoader
{
    private const string ResourcesPrefix = "ProgressMap/";

    private const string ArtRoot = "Assets/Art/";

    public static Sprite LoadSprite(string resourcesName, string editorTexturePath)
    {
        var fromResources = Resources.Load<Sprite>(ResourcesPrefix + resourcesName);
        if (fromResources != null)
        {
            Debug.Log($"ProgressMapAssetLoader: Successfully loaded Sprite '{resourcesName}' from Resources.");
            return fromResources;
        }

        // Fallback if imported as default Texture2D
        var texFromResources = Resources.Load<Texture2D>(ResourcesPrefix + resourcesName);
        if (texFromResources != null)
        {
            Debug.Log($"ProgressMapAssetLoader: Loaded Texture2D '{resourcesName}' from Resources and converted to Sprite.");
            return Sprite.Create(texFromResources, new Rect(0, 0, texFromResources.width, texFromResources.height), new Vector2(0.5f, 0.5f), 100f);
        }

#if UNITY_EDITOR
        var sprites = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(editorTexturePath);
        foreach (var obj in sprites)
        {
            if (obj is Sprite sprite)
            {
                Debug.Log($"ProgressMapAssetLoader: Loaded Sprite from local Art path '{editorTexturePath}'.");
                return sprite;
            }
        }

        var texture = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(editorTexturePath);
        if (texture != null)
        {
            Debug.Log($"ProgressMapAssetLoader: Loaded Texture2D from local Art path '{editorTexturePath}' and converted to Sprite.");
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
        }
#endif

        Debug.LogError($"ProgressMapAssetLoader: Failed to load Sprite/Texture '{resourcesName}' (Art path: '{editorTexturePath}')!");
        return null;
    }

    public static Texture2D LoadTexture(string resourcesName, string editorTexturePath)
    {
        var fromResources = Resources.Load<Texture2D>(ResourcesPrefix + resourcesName);
        if (fromResources != null)
        {
            fromResources.wrapMode = TextureWrapMode.Repeat;
            Debug.Log($"ProgressMapAssetLoader: Successfully loaded Texture2D '{resourcesName}' from Resources (set wrapMode to Repeat).");
            return fromResources;
        }

        // Fallback if imported as Sprite
        var spriteFromResources = Resources.Load<Sprite>(ResourcesPrefix + resourcesName);
        if (spriteFromResources != null && spriteFromResources.texture != null)
        {
            spriteFromResources.texture.wrapMode = TextureWrapMode.Repeat;
            Debug.Log($"ProgressMapAssetLoader: Loaded Sprite '{resourcesName}' from Resources, extracted texture and set wrapMode to Repeat.");
            return spriteFromResources.texture;
        }

#if UNITY_EDITOR
        var texture = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(editorTexturePath);
        if (texture != null)
        {
            texture.wrapMode = TextureWrapMode.Repeat;
            Debug.Log($"ProgressMapAssetLoader: Loaded Texture2D from local Art path '{editorTexturePath}' (set wrapMode to Repeat).");
            return texture;
        }

        var sprites = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(editorTexturePath);
        foreach (var obj in sprites)
        {
            if (obj is Sprite sprite && sprite.texture != null)
            {
                sprite.texture.wrapMode = TextureWrapMode.Repeat;
                Debug.Log($"ProgressMapAssetLoader: Loaded Sprite from local Art path '{editorTexturePath}', extracted texture and set wrapMode to Repeat.");
                return sprite.texture;
            }
        }
#endif

        Debug.LogError($"ProgressMapAssetLoader: Failed to load Texture/Sprite '{resourcesName}' (Art path: '{editorTexturePath}')!");
        return null;
    }

    public static Sprite MapBackground =>
        LoadSprite("pathway", ArtRoot + "Pathway/pathway-ancient-egypt.jpg");

    public static Sprite MapBackgroundAlt =>
        LoadSprite("bgPathway", ArtRoot + "Pathway/bgPathway.jpg");

    public static Texture2D NileWaterTexture =>
        LoadTexture("nile", ArtRoot + "3StroitelstvoPiramidi/Lesson3Nil.jpeg")
        ?? LoadTexture("nile2", ArtRoot + "2NileMath/Lesson2Nile1.jpeg");

    public static Sprite LessonLandmark(int lessonIndex)
    {
        switch (lessonIndex)
        {
            case 1:
                return LoadSprite("lesson1", ArtRoot + "1ancient-egypt/egyptian-numbers-note.jpg");
            case 2:
                return LoadSprite("lesson2", ArtRoot + "2NileMath/Lesson2Nile2.jpeg");
            case 3:
                return LoadSprite("lesson3", ArtRoot + "3StroitelstvoPiramidi/Lesson3Shaduf1.jpg");
            case 4:
                return LoadSprite("lesson4", ArtRoot + "4AstronomiyaKalendarOtSiriusa/Lesson4Sirius1Yarchayshaya.jpg");
            case 5:
                return LoadSprite("lesson5", ArtRoot + "5DostigeniyaIzobreteniya/Lesson5PapirusAhmesa.jpg");
            case 6:
                return LoadSprite("lesson6", ArtRoot + "6Mathematicians/Lesson6Pisec.jpg");
            default:
                return null;
        }
    }
}
