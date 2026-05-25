using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("UI Sounds")]
    public AudioClip buttonClickSound;
    public AudioClip buttonHoverSound;
    public AudioClip buttonDisabledSound;

    [Header("Settings")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float uiVolume = 0.8f;

    private AudioSource audioSource;
    private Dictionary<string, AudioClip> soundLibrary;

    void Awake()
    {
        // Реализуем Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSource();
            InitializeSoundLibrary();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeAudioSource()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        //// Настраиваем AudioSource
        //audioSource.playOnAwake = false;
        //audioSource.loop = false;
        //audioSource.spatialBlend = 0f; // 2D звук
    }

    void InitializeSoundLibrary()
    {
        soundLibrary = new Dictionary<string, AudioClip>
        {
            { "click", buttonClickSound },
            { "hover", buttonHoverSound },
            { "disabled", buttonDisabledSound }
        };
    }

    // Публичные методы для воспроизведения звуков
    public void PlayClickSound()
    {
        PlaySound("click", uiVolume);
    }

    public void PlayHoverSound()
    {
        PlaySound("hover", uiVolume * 0.7f); // Тише чем клик
    }

    public void PlayDisabledSound()
    {
        PlaySound("disabled", uiVolume * 0.5f);
    }

    // Универсальный метод для воспроизведения любого звука
    public void PlaySound(string soundName, float volume = 1f)
    {
        if (soundLibrary.ContainsKey(soundName) && soundLibrary[soundName] != null)
        {
            audioSource.PlayOneShot(soundLibrary[soundName], volume * masterVolume);
        }
        else
        {
            Debug.LogWarning($"Sound '{soundName}' not found in library!");
        }
    }

    // Методы для управления громкостью
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
    }

    public void SetUIVolume(float volume)
    {
        uiVolume = Mathf.Clamp01(volume);
    }

    // Добавление новых звуков в runtime
    public void AddSoundToLibrary(string name, AudioClip clip)
    {
        if (!soundLibrary.ContainsKey(name))
        {
            soundLibrary.Add(name, clip);
        }
        else
        {
            Debug.LogWarning($"Sound '{name}' already exists in library!");
        }
    }
}