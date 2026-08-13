using UnityEngine;

/// <summary>
/// Global audio / VFX preferences. Stored in PlayerPrefs separately from progress
/// so «Обнулить счёт» does not reset them.
/// </summary>
public static class GameSettings
{
    const string SoundKey = "SoundEnabled";
    const string VfxKey = "VfxEnabled";

    public static bool SoundEnabled
    {
        get => PlayerPrefs.GetInt(SoundKey, 1) == 1;
        private set
        {
            PlayerPrefs.SetInt(SoundKey, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public static bool VfxEnabled
    {
        get => PlayerPrefs.GetInt(VfxKey, 1) == 1;
        private set
        {
            PlayerPrefs.SetInt(VfxKey, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public static void SetSoundEnabled(bool enabled)
    {
        SoundEnabled = enabled;
    }

    public static void SetVfxEnabled(bool enabled)
    {
        VfxEnabled = enabled;
    }
}
