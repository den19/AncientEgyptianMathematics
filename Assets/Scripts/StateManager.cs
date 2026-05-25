using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StateManager : MonoBehaviour
{
    // Singleton instance
    public static StateManager Instance { get; private set; }

    // Состояние приложения
    public static int Energy { get; private set; }
    private static int rank;
    public static int Rank
    {
        get { return rank; }
        private set { rank = Mathf.Clamp(value, 0, 100); }
    }
    public static float SessionTime { get; private set; }

    // Ссылки на эффекты в инспекторе
    [Header("VFX Effects")]
    public ParticleSystem clickVFX; // Эффект при каждом клике
    public ParticleSystem energy10VFX; // Эффект при кликах кратных 10
    public ParticleSystem energy20VFX; // Эффект при кликах кратных 20

    [Header("SFX Effects")]
    public AudioClip clickSFX; // SFX при каждом клике
    public AudioClip energy10SFX; // SFX при кликах кратных 10
    public AudioClip energy20SFX; // SFX при кликах кратных 20
    public AudioClip energy30SFX; // SFX при 30 кликах

    [Header("UI Elements")]
    public GameObject achievementPanel1; // Панель достижения Уровня1
    public GameObject achievementPanel2; // Панель достижения Уровня2
    public GameObject achievementPanel3; // Панель достижения Уровня3
    public GameObject achievementPanel4; // Панель достижения Уровня4
    public GameObject achievementPanel5; // Панель достижения Уровня5
    public GameObject achievementPanel6; // Панель достижения Уровня6

    public Sprite currenLevelIcon; // Стрелка вправо
    public Sprite doneLevelIcon; // Корона

    [Header("Rank Increments")]
    public float rankIncrement10 = 0.1f; // Увеличение Rank при кликах кратных 10
    public float rankIncrement20 = 0.2f; // Увеличение Rank при кликах кратных 20

    // Таймер и компоненты UI
    private Coroutine timeCoroutine;
    private Text sessionTimeText;
    private TextMeshProUGUI energyText;
    private TextMeshProUGUI rankText;
    private AudioSource audioSource;
    
    // Состояния Lesson1
    public static bool Lesson1Complete;
    private static bool AchievementPanel1Passed;

    private static bool Lesson1EdiniciDecatkiSotniPassed;
    private static bool Lesson1TiciachiPassed;
    private static bool Lesson1AlikvotnieDrobiPassed;
    private static bool Lesson1PrimeriPassed;
    private static bool Lesson1ReadingPassed;

    private const int Lesson1EdiniciDecatkiSotniValue = 3;
    private const int Lesson1TiciachiValue = 3;
    private const int Lesson1AlikvotnieDrobiValue = 3;
    private const int Lesson1PrimeriValue = 3;
    private const int Lesson1ReadingValue = 3;

    // Состояния Lesson2
    public static bool Lesson2Complete;
    private static bool AchievementPanel2Passed;

    private static bool Lesson2RazvlivNilaPassed;
    private static bool Lesson2ProblemaPloshadiUchastkaPassed;
    private static bool Lesson2GerpedonaptPassed;
    private static bool Lesson2TeoremaPifagoraPassed;
    private static bool Lesson2EgipetskiyTreugolnikPassed;
    private static bool Lesson2ReadingPassed;

    private const int Lesson2RazvlivNilaValue = 3 ;
    private const int Lesson2ProblemaPloshadiUchastkaValue = 3 ;
    private const int Lesson2GerpedonaptValue =3 ;
    private const int Lesson2TeoremaPifagoraValue = 3;
    private const int Lesson2EgipetskiyTreugolnikValue = 3;
    private const int Lesson2ReadingValue = 3;

    // Состояния Lesson3
    public static bool Lesson3Complete;
    private static bool AchievementPanel3Passed;

    private static bool Lesson3ShadufPassed;
    private static bool Lesson3PodemVodiCherezShadufPassed;
    private static bool Lesson3ShluziPassed;
    private static bool Lesson3VodniePutiPyramidPassed;
    private static bool Lesson3ReadingPassed;

    private const int Lesson3ShadufValue = 3;
    private const int Lesson3PodemVodiCherezShadufValue = 3;
    private const int Lesson3ShluziValue = 3;
    private const int Lesson3VodniePutiPyramidValue = 3;
    private const int Lesson3ReadingValue = 3;


    // Состояния Lesson4
    public static bool Lesson4Complete;
    private static bool AchievementPanel4Passed;

    private static bool Lesson4SiriusAPassed;
    private static bool Lesson4SiriusAAndSunPassed;
    private static bool Lesson4DvoynayaZvezdaPassed;
    private static bool Lesson4SozvezdieBolshoyPesPassed;
    private static bool Lesson4ReadingPassed;

    private const int Lesson4SiriusValue = 3;
    private const int Lesson4SiriusAAndSunValue = 3;
    private const int Lesson4DvoynayaZvezdaValue = 3;
    private const int Lesson4SozvezdieBolshoyPesValue = 3;
    private const int Lesson4ReadingValue = 3;


    // Состояния Lesson5
    public static bool Lesson5Complete;
    private static bool AchievementPanel5Passed;

    private static bool Lesson5MoscowMathPapirusPassed;
    private static bool Lesson5PapirusAhmesaPassed;
    private static bool Lesson5EgypetskayaPloshadChetirehugolnikaPassed;
    private static bool Lesson5ChisloPiPassed;
    private static bool Lesson5KakPoyavilosChisloPiPassed;
    private static bool Lesson5ReadingPassed;

    private const int Lesson5MoscowMathPapirusValue = 3;
    private const int Lesson5PapirusAhmesaValue = 3;
    private const int Lesson5EgypetskayaPloshadChetirehugolnikaValue = 3;
    private const int Lesson5ChisloPiValue = 3;
    private const int Lesson5KakPoyavilosChisloPiValue = 3;
    private const int Lesson5ReadingValue = 3;

    // Состояния Lesson6
    private static bool Lesson6PicesPassed;
    private static bool Lesson6ReadingPassed;

    private const int Lesson6PicesValue = 11;
    private const int Lesson6ReadingValue = 11;

    public static bool Lesson6Complete;
    private static bool AchievementPanel6Passed;

    // Ссылка на контроллер карты
    private static ProgressMapController mapController;
    public static ProgressMapController MapController
    {
        get
        {
            if (mapController == null)
            {
                mapController = FindObjectOfType<ProgressMapController>();
            }
            return mapController;
        }
    }

    private void Awake()
    {
        // Реализация Singleton
        if (Instance == null)
        {
            Instance = this;            
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Initialize()
    {
        // Добавляем AudioSource если его нет
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        // Загрузка сохраненных данных
        LoadState();

        // Подписка на событие смены сцены
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Запуск таймера
        StartTimeTracking();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Поиск UI компонентов на новой сцене
        FindUIElements();
    }

    public void FindUIElements()
    {
        // Поиск текста времени сессии
        GameObject timeObject = GameObject.FindWithTag("SessionTime");
        if (timeObject != null)
        {
            sessionTimeText = timeObject.GetComponent<Text>();
            UpdateTimeDisplay();
        }

        // Поиск текста энергии
        GameObject energyObject = GameObject.FindWithTag("Energy");
        if (energyObject != null)
        {
            energyText = energyObject.GetComponent<TextMeshProUGUI>();
            UpdateEnergyDisplay();
        }

        // Поиск текста процентов
        GameObject rankObject = GameObject.FindWithTag("Rank");
        if (rankObject != null)
        {
            rankText = rankObject.GetComponent<TextMeshProUGUI>();
            UpdateRankDisplay();
        }
        
        if (Lesson1Complete)
        {
            SetLevel1IconComplete();
        }
        
        if (Lesson2Complete)
        {
            SetLevel2IconComplete();
        }

        if (Lesson3Complete)
        {
            SetLevel3IconComplete();
        }

        if (Lesson4Complete)
        {
            SetLevel4IconComplete();
        }

        if (Lesson5Complete)
        {
            SetLevel5IconComplete();
        }

        if (Lesson6Complete)
        {
            SetLevel6IconComplete();
        }

        SetProgress(Rank / 100f);
        RefreshProgressMarkers();
    }

    /// <summary>
    /// Показывает стрелку на первом незавершённом уроке; скрывает иконки остальных незавершённых.
    /// </summary>
    public void RefreshProgressMarkers()
    {
        bool[] complete =
        {
            Lesson1Complete,
            Lesson2Complete,
            Lesson3Complete,
            Lesson4Complete,
            Lesson5Complete,
            Lesson6Complete
        };

        int currentLesson = 0;
        for (int i = 0; i < complete.Length; i++)
        {
            if (!complete[i])
            {
                currentLesson = i + 1;
                break;
            }
        }

        for (int i = 1; i <= 6; i++)
        {
            var iconGo = GameObject.Find("Level" + i + "Icon");
            if (iconGo == null)
                continue;

            var image = iconGo.GetComponent<Image>();
            if (image == null)
                continue;

            if (complete[i - 1])
            {
                image.enabled = true;
                image.color = Color.white;
                continue;
            }

            if (i == currentLesson)
            {
                switch (i)
                {
                    case 1: SetLevel1IconCurrent(); break;
                    case 2: SetLevel2IconCurrent(); break;
                    case 3: SetLevel3IconCurrent(); break;
                    case 4: SetLevel4IconCurrent(); break;
                    case 5: SetLevel5IconCurrent(); break;
                    case 6: SetLevel6IconCurrent(); break;
                }
                image.enabled = true;
                image.color = Color.white;
            }
            else
            {
                // Показываем золотой ключик для закрытых/заблокированных уровней
                var goldKey = Resources.Load<Sprite>("ProgressMap/goldKey");
                if (goldKey != null)
                {
                    image.sprite = goldKey;
                }
                image.enabled = true;
                image.color = new Color(1f, 1f, 1f, 0.75f); // Слегка полупрозрачный для заблокированных
            }
        }
    }

    private void StartTimeTracking()
    {
        if (timeCoroutine != null)
            StopCoroutine(timeCoroutine);

        timeCoroutine = StartCoroutine(TimeTrackerCoroutine());
    }

    private IEnumerator TimeTrackerCoroutine()
    {
        int saveCounter = 0;
        while (true)
        {
            yield return new WaitForSeconds(1f);
            SessionTime += 1f;
            UpdateTimeDisplay();

            saveCounter++;
            if (saveCounter >= 10)
            {
                saveCounter = 0;
                SaveState();
            }
        }
    }

    private void UpdateTimeDisplay()
    {
        if (sessionTimeText != null)
        {
            sessionTimeText.text = FormatTime(SessionTime);
        }
    }

    private void UpdateEnergyDisplay()
    {
        if (energyText != null)
        {
            energyText.text = Energy.ToString();
        }
    }

    private void UpdateRankDisplay()
    {
        if (rankText != null)
        {
            rankText.text = Rank.ToString();
        }
    }

    private string FormatTime(float seconds)
    {
        TimeSpan time = TimeSpan.FromSeconds(seconds);
        return string.Format("{0:D2}:{1:D2}:{2:D2}",
                            time.Hours,
                            time.Minutes,
                            time.Seconds);
    }

    // Метод для добавления кликов
    public void AddClick(Vector3 clickPosition)
    {
        Energy++;

        // Воспроизведение эффектов при клике
        PlayClickVFX(clickPosition);
        PlayClickSFX();

        // Проверка достижений (кратность 10 и 20)
        CheckAchievements();

        // Обновление UI
        UpdateEnergyDisplay();
        UpdateRankDisplay();
        SaveState();
    }

    private void PlayClickVFX(Vector3 position)
    {
        if (clickVFX != null)
        {
            // Создаем эффект в позиции клика
            ParticleSystem vfxInstance = Instantiate(clickVFX, position, Quaternion.identity);
            vfxInstance.Play();

            // Уничтожаем после завершения
            Destroy(vfxInstance.gameObject, vfxInstance.main.duration);
        }
    }

    private void PlayClickSFX()
    {
        if (clickSFX != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSFX);
        }
    }

    private void CheckAchievements()
    {
        // Проверка кратности 10
        if (Energy % 10 == 0 && Energy > 0)
        {
            PlayEnergy10VFX();
            PlayEnergy10SFX();
        }

        // Проверка кратности 20
        if (Energy % 20 == 0 && Energy > 0)
        {
            PlayEnergy20VFX();
            PlayEnergy20SFX();
        }

        if (Lesson1AlikvotnieDrobiPassed && Lesson1EdiniciDecatkiSotniPassed &&
            Lesson1PrimeriPassed && Lesson1TiciachiPassed && Lesson1ReadingPassed)
        {
            Lesson1Complete = true;
        }

        if (Rank >= 15)
        {
            ShowAchievementPanel1();            
        }

        if (Lesson2EgipetskiyTreugolnikPassed && Lesson2GerpedonaptPassed && 
            Lesson2ProblemaPloshadiUchastkaPassed && Lesson2RazvlivNilaPassed && Lesson2TeoremaPifagoraPassed &&
            Lesson2ReadingPassed)
        {
            Lesson2Complete = true;
        }

        if (Rank >= 33)
        {
            ShowAchievementPanel2();
        }

        if (Lesson3ShadufPassed && Lesson3ShluziPassed && Lesson3VodniePutiPyramidPassed &&
            Lesson3PodemVodiCherezShadufPassed && Lesson3ReadingPassed)
        {
            Lesson3Complete = true;
        }    

        if (Rank >= 48)
        {
            ShowAchievementPanel3();
        }

        if (Lesson4SiriusAPassed && Lesson4SiriusAAndSunPassed && Lesson4DvoynayaZvezdaPassed &&
            Lesson4SozvezdieBolshoyPesPassed && Lesson4ReadingPassed)
        {
            Lesson4Complete = true;
        }

        if (Rank >= 63)
        {
            ShowAchievementPanel4();
        }

        if (Lesson5MoscowMathPapirusPassed && Lesson5PapirusAhmesaPassed && Lesson5EgypetskayaPloshadChetirehugolnikaPassed &&
            Lesson5ChisloPiPassed && Lesson5KakPoyavilosChisloPiPassed && Lesson5ReadingPassed)
        {
            Lesson5Complete = true;
        }

        if (Rank >= 81)
        {
            ShowAchievementPanel5();
        }

        if (Lesson6PicesPassed && Lesson6ReadingPassed)
        {
            Lesson6Complete = true;
        }

        if (Rank >= 100)
        {
            ShowAchievementPanel6();
        }

        // Проверка достижения 30 кликов (однократное срабатывание)
        if (Energy == 30)
        {
            PlayEnergy30SFX();
        }
    }

    public void SetProgress(float value)
    {
        // Находим компонент RadialFillComponent
        GameObject radialFillComponent = GameObject.Find("RadialFillComponent");

        if (radialFillComponent != null)
        {
            // Находим компонент Image с типом Filled
            Image image = radialFillComponent.GetComponent<Image>();

            if (image != null && image.type == Image.Type.Filled)
            {
                // Устанавливаем Fill Amount
                image.fillAmount = value;
            }
        }
    }

    public void SetLevel1IconComplete()
    {
        var level1Icon = GameObject.Find("Level1Icon");
        if(level1Icon != null)
        {
            var imageComponent = level1Icon.GetComponent<Image>();
            if (imageComponent != null && doneLevelIcon != null)
            {
                imageComponent.sprite = doneLevelIcon;
            }            
        }
    }

    public void SetLevel2IconComplete()
    {
        var level2Icon = GameObject.Find("Level2Icon");
        if (level2Icon != null)
        {
            var imageComponent = level2Icon.GetComponent<Image>();
            if (imageComponent != null && doneLevelIcon != null)
            {
                imageComponent.sprite = doneLevelIcon;
            }
        }
    }

    public void SetLevel3IconComplete()
    {
        var level3Icon = GameObject.Find("Level3Icon");
        if (level3Icon != null)
        {
            var imageComponent = level3Icon.GetComponent<Image>();
            if (imageComponent != null && doneLevelIcon != null)
            {
                imageComponent.sprite = doneLevelIcon;
            }
        }
    }

    public void SetLevel4IconComplete()
    {
        var level4Icon = GameObject.Find("Level4Icon");
        if (level4Icon != null)
        {
            var imageComponent = level4Icon.GetComponent<Image>();
            if (imageComponent != null && doneLevelIcon != null)
            {
                imageComponent.sprite = doneLevelIcon;
            }
        }
    }

    public void SetLevel5IconComplete()
    {
        var level5Icon = GameObject.Find("Level5Icon");
        if (level5Icon != null)
        {
            var imageComponent = level5Icon.GetComponent<Image>();
            if (imageComponent != null && doneLevelIcon != null)
            {
                imageComponent.sprite = doneLevelIcon;
            }
        }
    }

    public void SetLevel6IconComplete()
    {
        var level6Icon = GameObject.Find("Level6Icon");
        if (level6Icon != null)
        {
            var imageComponent = level6Icon.GetComponent<Image>();
            if (imageComponent != null && doneLevelIcon != null)
            {
                imageComponent.sprite = doneLevelIcon;
            }
        }
    }
    public void SetLevel1IconCurrent()
    {
        var level1Icon = GameObject.Find("Level1Icon");
        if (level1Icon != null)
        {
            var imageComponent = level1Icon.GetComponent<Image>();
            if (imageComponent != null && currenLevelIcon != null)
            {
                imageComponent.sprite = currenLevelIcon;
            }
        }
    }

    public void SetLevel2IconCurrent()
    {
        var level2Icon = GameObject.Find("Level2Icon");
        if (level2Icon != null)
        {
            var imageComponent = level2Icon.GetComponent<Image>();
            if (imageComponent != null && currenLevelIcon != null)
            {
                imageComponent.sprite = currenLevelIcon;
            }
        }
    }

    public void SetLevel3IconCurrent()
    {
        var level3Icon = GameObject.Find("Level3Icon");
        if (level3Icon != null)
        {
            var imageComponent = level3Icon.GetComponent<Image>();
            if (imageComponent != null && currenLevelIcon != null)
            {
                imageComponent.sprite = currenLevelIcon;
            }
        }
    }

    public void SetLevel4IconCurrent()
    {
        var level4Icon = GameObject.Find("Level4Icon");
        if (level4Icon != null)
        {
            var imageComponent = level4Icon.GetComponent<Image>();
            if (imageComponent != null && currenLevelIcon != null)
            {
                imageComponent.sprite = currenLevelIcon;
            }
        }
    }

    public void SetLevel5IconCurrent()
    {
        var level5Icon = GameObject.Find("Level5Icon");
        if (level5Icon != null)
        {
            var imageComponent = level5Icon.GetComponent<Image>();
            if (imageComponent != null && currenLevelIcon != null)
            {
                imageComponent.sprite = currenLevelIcon;
            }
        }
    }

    public void SetLevel6IconCurrent()
    {
        var level6Icon = GameObject.Find("Level6Icon");
        if (level6Icon != null)
        {
            var imageComponent = level6Icon.GetComponent<Image>();
            if (imageComponent != null && currenLevelIcon != null)
            {
                imageComponent.sprite = currenLevelIcon;
            }
        }
    }

    private void PlayEnergy10VFX()
    {
        if (energy10VFX != null)
        {
            ParticleSystem vfxInstance = Instantiate(energy10VFX, Vector3.zero, Quaternion.identity);
            vfxInstance.Play();
            Destroy(vfxInstance.gameObject, vfxInstance.main.duration);
        }
    }

    private void PlayEnergy10SFX()
    {
        if (energy10SFX != null && audioSource != null)
        {
            audioSource.PlayOneShot(energy10SFX);
        }
    }

    private void PlayEnergy20VFX()
    {
        if (energy20VFX != null)
        {
            ParticleSystem vfxInstance = Instantiate(energy20VFX, Vector3.zero, Quaternion.identity);
            vfxInstance.Play();
            Destroy(vfxInstance.gameObject, vfxInstance.main.duration);
        }
    }

    private void PlayEnergy20SFX()
    {
        if (energy20SFX != null && audioSource != null)
        {
            audioSource.PlayOneShot(energy20SFX);
        }
    }

    private void PlayEnergy30SFX()
    {
        if (energy30SFX != null && audioSource != null)
        {
            audioSource.PlayOneShot(energy30SFX);
        }
    }

    private void ShowAchievementPanel1()
    {
        if (achievementPanel1 != null && !AchievementPanel1Passed)
        {
            var go = Resources.FindObjectsOfTypeAll<GameObject>();
            var panel = go.FirstOrDefault(g => g.name == "MainCanvas");
            if (panel != null)
            {
                Instantiate(achievementPanel1, panel.transform);
                AchievementPanel1Passed = true;
            }
        }        
    }

    private void ShowAchievementPanel2()
    {
        if (achievementPanel2 != null && !AchievementPanel2Passed)
        {
            var go = Resources.FindObjectsOfTypeAll<GameObject>();
            var panel = go.FirstOrDefault(g => g.name == "MainCanvas");
            if (panel != null)
            {
                Instantiate(achievementPanel2, panel.transform);
                AchievementPanel2Passed = true;
            }
        }
    }

    private void ShowAchievementPanel3()
    {
        if (achievementPanel3 != null && !AchievementPanel3Passed)
        {
            var go = Resources.FindObjectsOfTypeAll<GameObject>();
            var panel = go.FirstOrDefault(g => g.name == "MainCanvas");
            if (panel != null)
            {
                Instantiate(achievementPanel3, panel.transform);
                AchievementPanel3Passed = true;
            }
        }
    }

    private void ShowAchievementPanel4()
    {
        if (achievementPanel4 != null && !AchievementPanel4Passed)
        {
            var go = Resources.FindObjectsOfTypeAll<GameObject>();
            var panel = go.FirstOrDefault(g => g.name == "MainCanvas");
            if (panel != null)
            {
                Instantiate(achievementPanel4, panel.transform);
                AchievementPanel4Passed = true;
            }
        }
    }

    private void ShowAchievementPanel5()
    {
        if (achievementPanel5 != null && !AchievementPanel5Passed)
        {
            var go = Resources.FindObjectsOfTypeAll<GameObject>();
            var panel = go.FirstOrDefault(g => g.name == "MainCanvas");
            if (panel != null)
            {
                Instantiate(achievementPanel5, panel.transform);
                AchievementPanel5Passed = true;
            }
        }
    }

    private void ShowAchievementPanel6()
    {
        if (achievementPanel6 != null && !AchievementPanel6Passed)
        {
            var go = Resources.FindObjectsOfTypeAll<GameObject>();
            var panel = go.FirstOrDefault(g => g.name == "MainCanvas");
            if (panel != null)
            {
                Instantiate(achievementPanel6, panel.transform);
                AchievementPanel6Passed = true;
            }
        }
    }

    public void UpdateRank(float progress)
    {
        Rank = (int)Mathf.Clamp(progress, 0f, 100f);
        SaveState();
    }

    public void AddRank(float amount)
    {
        Rank = (int)Mathf.Clamp(Rank + amount, 0f, 100f);
        SaveState();

        // Можно добавить логику для обновления UI Rank если нужно
        Debug.Log($"Rank increased to: {Rank}");
    }

    // Сохранение и загрузка
    private void SaveState()
    {
        PlayerPrefs.SetInt("Energy", Energy);
        PlayerPrefs.SetInt("Rank", Rank);
        PlayerPrefs.SetFloat("SessionTime", SessionTime);

        PlayerPrefs.SetInt("Lesson1EdiniciDecatkiSotniPassed", Lesson1EdiniciDecatkiSotniPassed == true ? 1 : 0);
        PlayerPrefs.SetInt("Lesson1TiciachiPassed", Lesson1TiciachiPassed == true ? 1 : 0);
        PlayerPrefs.SetInt("Lesson1AlikvotnieDrobiPassed", Lesson1AlikvotnieDrobiPassed == true ? 1 : 0);
        PlayerPrefs.SetInt("Lesson1PrimeriPassed", Lesson1PrimeriPassed == true ? 1 : 0);
        PlayerPrefs.SetInt("Lesson1ReadingPassed", Lesson1ReadingPassed == true ? 1 : 0);
        PlayerPrefs.SetInt("AchievementPanel1Passed", AchievementPanel1Passed == true ? 1 : 0);

        PlayerPrefs.SetInt("Lesson2RazvlivNilaPassed", Lesson2RazvlivNilaPassed == true ? 1 : 0);
        PlayerPrefs.SetInt("Lesson2ProblemaPloshadiUchastkaPassed", Lesson2ProblemaPloshadiUchastkaPassed == true ? 1 : 0);
        PlayerPrefs.SetInt("Lesson2GerpedonaptPassed", Lesson2GerpedonaptPassed == true ? 1 : 0);
        PlayerPrefs.SetInt("Lesson2TeoremaPifagoraPassed", Lesson2TeoremaPifagoraPassed == true ? 1 : 0);
        PlayerPrefs.SetInt("Lesson2EgipetskiyTreugolnikPassed", Lesson2EgipetskiyTreugolnikPassed == true ? 1 : 0);
        PlayerPrefs.SetInt("Lesson2ReadingPassed", Lesson2ReadingPassed == true ? 1 : 0);
        PlayerPrefs.SetInt("AchievementPanel2Passed", AchievementPanel2Passed == true ? 1 : 0);

        PlayerPrefs.SetInt("Lesson3ShadufPassed", Lesson3ShadufPassed == true ? 1 : 0);
        PlayerPrefs.SetInt("Lesson3PodemVodiCherezShadufPassed", Lesson3PodemVodiCherezShadufPassed == true ? 1 : 0);
        PlayerPrefs.SetInt("Lesson3ShluziPassed", Lesson3ShluziPassed == true ? 1 : 0);
        PlayerPrefs.SetInt("Lesson3VodniePutiPyramidPassed", Lesson3VodniePutiPyramidPassed == true ? 1 : 0);
        PlayerPrefs.SetInt("Lesson3ReadingPassed", Lesson3ReadingPassed == true ? 1 : 0);
        PlayerPrefs.SetInt("AchievementPanel3Passed", AchievementPanel3Passed == true ? 1 : 0);


        PlayerPrefs.SetInt("Lesson4SiriusAPassed", Lesson4SiriusAPassed == true ? 1 : 0);
        PlayerPrefs.SetInt("Lesson4SiriusAAndSunPassed", Lesson4SiriusAAndSunPassed == true ? 1 : 0);
        PlayerPrefs.SetInt("Lesson4DvoynayaZvezdaPassed", Lesson4DvoynayaZvezdaPassed == true ? 1 : 0);
        PlayerPrefs.SetInt("Lesson4SozvezdieBolshoyPesPassed", Lesson4SozvezdieBolshoyPesPassed == true ? 1 : 0);
        PlayerPrefs.SetInt("Lesson4ReadingPassed", Lesson4ReadingPassed == true ? 1 : 0);
        PlayerPrefs.SetInt("AchievementPanel4Passed", AchievementPanel4Passed == true ? 1 : 0);

        PlayerPrefs.SetInt("Lesson5MoscowMathPapirusPassed", Lesson5MoscowMathPapirusPassed == true ? 1 : 0);
        PlayerPrefs.SetInt("Lesson5PapirusAhmesaPassed", Lesson5PapirusAhmesaPassed == true ? 1 : 0);
        PlayerPrefs.SetInt("Lesson5EgypetskayaPloshadChetirehugolnikaPassed", Lesson5EgypetskayaPloshadChetirehugolnikaPassed == true ? 1 : 0);
        PlayerPrefs.SetInt("Lesson5ChisloPiPassed", Lesson5ChisloPiPassed == true ? 1 : 0);
        PlayerPrefs.SetInt("Lesson5KakPoyavilosChisloPiPassed", Lesson5KakPoyavilosChisloPiPassed == true ? 1 : 0);
        PlayerPrefs.SetInt("Lesson5ReadingPassed", Lesson5ReadingPassed == true ? 1 : 0);
        PlayerPrefs.SetInt("AchievementPanel5Passed", AchievementPanel5Passed == true ? 1 : 0);

        PlayerPrefs.SetInt("Lesson6PicesPassed", Lesson6PicesPassed == true ? 1 : 0);
        PlayerPrefs.SetInt("Lesson6ReadingPassed", Lesson6ReadingPassed == true ? 1 : 0);
        PlayerPrefs.SetInt("AchievementPanel6Passed", AchievementPanel6Passed == true ? 1 : 0);

        PlayerPrefs.SetInt("Lesson1Complete", Lesson1Complete == true ? 1 : 0);
        PlayerPrefs.SetInt("Lesson2Complete", Lesson2Complete == true ? 1 : 0);
        PlayerPrefs.SetInt("Lesson3Complete", Lesson3Complete == true ? 1 : 0);
        PlayerPrefs.SetInt("Lesson4Complete", Lesson4Complete == true ? 1 : 0);
        PlayerPrefs.SetInt("Lesson5Complete", Lesson5Complete == true ? 1 : 0);
        PlayerPrefs.SetInt("Lesson6Complete", Lesson6Complete == true ? 1 : 0);

        PlayerPrefs.Save();
        UpdateRankDisplay();
    }

    public static void LoadState()
    {
        Energy = PlayerPrefs.GetInt("Energy", 0);
        Rank = PlayerPrefs.GetInt("Rank", 0);
        SessionTime = PlayerPrefs.GetFloat("SessionTime", 0f);

        Lesson1EdiniciDecatkiSotniPassed = PlayerPrefs.GetInt("Lesson1EdiniciDecatkiSotniPassed", 0) == 1 ? true : false;
        Lesson1TiciachiPassed = PlayerPrefs.GetInt("Lesson1TiciachiPassed", 0) == 1 ? true : false;
        Lesson1AlikvotnieDrobiPassed = PlayerPrefs.GetInt("Lesson1AlikvotnieDrobiPassed", 0) == 1 ? true : false;
        Lesson1PrimeriPassed = PlayerPrefs.GetInt("Lesson1PrimeriPassed", 0) == 1 ? true : false;
        Lesson1ReadingPassed = PlayerPrefs.GetInt("Lesson1ReadingPassed", 0) == 1 ? true : false;
        AchievementPanel1Passed = PlayerPrefs.GetInt("AchievementPanel1Passed", 0) == 1 ? true : false;

        Lesson2RazvlivNilaPassed = PlayerPrefs.GetInt("Lesson2RazvlivNilaPassed", 0) == 1 ? true : false;
        Lesson2ProblemaPloshadiUchastkaPassed = PlayerPrefs.GetInt("Lesson2ProblemaPloshadiUchastkaPassed", 0) == 1 ? true : false;
        Lesson2GerpedonaptPassed = PlayerPrefs.GetInt("Lesson2GerpedonaptPassed", 0) == 1 ? true : false;
        Lesson2TeoremaPifagoraPassed = PlayerPrefs.GetInt("Lesson2TeoremaPifagoraPassed", 0) == 1 ? true : false;
        Lesson2EgipetskiyTreugolnikPassed = PlayerPrefs.GetInt("Lesson2EgipetskiyTreugolnikPassed", 0) == 1 ? true : false;
        Lesson2ReadingPassed = PlayerPrefs.GetInt("Lesson2ReadingPassed", 0) == 1 ? true : false;
        AchievementPanel2Passed = PlayerPrefs.GetInt("AchievementPanel2Passed", 0) == 1 ? true : false;

        Lesson3ShadufPassed = PlayerPrefs.GetInt("Lesson3ShadufPassed", 0) == 1 ? true : false;
        Lesson3ShluziPassed = PlayerPrefs.GetInt("Lesson3ShluziPassed", 0) == 1 ? true : false;
        Lesson3PodemVodiCherezShadufPassed = PlayerPrefs.GetInt("Lesson3PodemVodiCherezShadufPassed", 0) == 1 ? true : false;
        Lesson3VodniePutiPyramidPassed = PlayerPrefs.GetInt("Lesson3VodniePutiPyramidPassed", 0) == 1 ? true : false;
        Lesson3ReadingPassed = PlayerPrefs.GetInt("Lesson3ReadingPassed", 0) == 1 ? true : false;
        AchievementPanel3Passed = PlayerPrefs.GetInt("AchievementPanel3Passed", 0) == 1 ? true : false;

        Lesson4SiriusAPassed = PlayerPrefs.GetInt("Lesson4SiriusAPassed", 0) == 1 ? true : false;
        Lesson4SiriusAAndSunPassed = PlayerPrefs.GetInt("Lesson4SiriusAAndSunPassed", 0) == 1 ? true : false;
        Lesson4DvoynayaZvezdaPassed = PlayerPrefs.GetInt("Lesson4DvoynayaZvezdaPassed", 0) == 1 ? true : false;
        Lesson4SozvezdieBolshoyPesPassed = PlayerPrefs.GetInt("Lesson4SozvezdieBolshoyPesPassed", 0) == 1 ? true : false;
        Lesson4ReadingPassed = PlayerPrefs.GetInt("Lesson4ReadingPassed", 0) == 1 ? true : false;
        AchievementPanel4Passed = PlayerPrefs.GetInt("AchievementPanel4Passed", 0) == 1 ? true : false;

        Lesson5MoscowMathPapirusPassed = PlayerPrefs.GetInt("Lesson5MoscowMathPapirusPassed", 0) == 1 ? true : false;
        Lesson5PapirusAhmesaPassed = PlayerPrefs.GetInt("Lesson5PapirusAhmesaPassed", 0) == 1 ? true : false;
        Lesson5EgypetskayaPloshadChetirehugolnikaPassed = PlayerPrefs.GetInt("Lesson5EgypetskayaPloshadChetirehugolnikaPassed", 0) == 1 ? true : false;
        Lesson5ChisloPiPassed = PlayerPrefs.GetInt("Lesson5ChisloPiPassed", 0) == 1 ? true : false;
        Lesson5KakPoyavilosChisloPiPassed = PlayerPrefs.GetInt("Lesson5KakPoyavilosChisloPiPassed", 0) == 1 ? true : false;
        Lesson5ReadingPassed = PlayerPrefs.GetInt("Lesson5ReadingPassed", 0) == 1 ? true : false;
        AchievementPanel5Passed = PlayerPrefs.GetInt("AchievementPanel5Passed", 0) == 1 ? true : false;

        Lesson6PicesPassed = PlayerPrefs.GetInt("Lesson6PicesPassed", 0) == 1 ? true : false;
        Lesson6ReadingPassed = PlayerPrefs.GetInt("Lesson6ReadingPassed", 0) == 1 ? true : false;
        AchievementPanel6Passed = PlayerPrefs.GetInt("AchievementPanel6Passed", 0) == 1 ? true : false;

        Lesson1Complete = PlayerPrefs.GetInt("Lesson1Complete", 0) == 1 ? true : false;
        Lesson2Complete = PlayerPrefs.GetInt("Lesson2Complete", 0) == 1 ? true : false;
        Lesson3Complete = PlayerPrefs.GetInt("Lesson3Complete", 0) == 1 ? true : false;
        Lesson4Complete = PlayerPrefs.GetInt("Lesson4Complete", 0) == 1 ? true : false;
        Lesson5Complete = PlayerPrefs.GetInt("Lesson5Complete", 0) == 1 ? true : false;
        Lesson6Complete = PlayerPrefs.GetInt("Lesson6Complete", 0) == 1 ? true : false;
    }

    // Сброс состояния (опционально)
    public void ResetState()
    {
        Energy = 0;
        Rank = 0;
        SessionTime = 0f;
        SetProgress(Rank / 100f);

        Lesson1EdiniciDecatkiSotniPassed = false;
        Lesson1TiciachiPassed = false;
        Lesson1AlikvotnieDrobiPassed = false;
        Lesson1PrimeriPassed = false;
        Lesson1ReadingPassed = false;
        AchievementPanel1Passed = false;

        Lesson2RazvlivNilaPassed = false;
        Lesson2ProblemaPloshadiUchastkaPassed = false;
        Lesson2GerpedonaptPassed = false;
        Lesson2TeoremaPifagoraPassed = false;
        Lesson2EgipetskiyTreugolnikPassed = false;
        Lesson2ReadingPassed = false;
        AchievementPanel2Passed = false;

        Lesson3ShadufPassed = false;
        Lesson3PodemVodiCherezShadufPassed = false;
        Lesson3ShluziPassed = false;
        Lesson3VodniePutiPyramidPassed = false;
        Lesson3ReadingPassed = false;
        AchievementPanel3Passed = false;

        Lesson4SiriusAPassed = false;
        Lesson4SiriusAAndSunPassed = false;
        Lesson4DvoynayaZvezdaPassed = false;
        Lesson4SozvezdieBolshoyPesPassed = false;
        Lesson4ReadingPassed = false;
        AchievementPanel4Passed = false;

        Lesson5MoscowMathPapirusPassed = false;
        Lesson5PapirusAhmesaPassed = false;
        Lesson5EgypetskayaPloshadChetirehugolnikaPassed = false;
        Lesson5ChisloPiPassed = false;
        Lesson5KakPoyavilosChisloPiPassed = false;
        Lesson5ReadingPassed = false;
        AchievementPanel5Passed = false;

        Lesson6PicesPassed = false;
        Lesson6ReadingPassed = false;
        AchievementPanel6Passed = false;

        Lesson1Complete = false;
        Lesson2Complete = false;
        Lesson3Complete = false;
        Lesson4Complete = false;
        Lesson5Complete = false;
        Lesson6Complete = false;

        SaveState();
        UpdateTimeDisplay();
        UpdateEnergyDisplay();
        UpdateRankDisplay();
        RefreshProgressMarkers();
    }

    // Метод для получения текущего Rank
    public float GetRank()
    {
        return Rank;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SaveState();
        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveState();
        }
    }

    private void OnApplicationQuit()
    {
        SaveState();
    }

    public void Notation2RazlivNilaSetter()
    {
        if (!Lesson2RazvlivNilaPassed)
        {
            Lesson2RazvlivNilaPassed = true;
            Rank += Lesson2RazvlivNilaValue;
            SaveState();
            CheckAchievements();
        }
    }

    public void Notation2ProblemaPloshadiUchastkaSetter()
    {
        if (!Lesson2ProblemaPloshadiUchastkaPassed)
        {
            Lesson2ProblemaPloshadiUchastkaPassed = true;
            Rank += Lesson2ProblemaPloshadiUchastkaValue;
            SaveState();
            CheckAchievements();
        }
    }

    public void Notation2GerpedonaptSetter()
    {
        if (!Lesson2GerpedonaptPassed)
        {
            Lesson2GerpedonaptPassed = true;
            Rank += Lesson2GerpedonaptValue;
            SaveState();
            CheckAchievements();
        }
    }

    public void Notation2TeoremaPifagoraSetter()
    {
        if (!Lesson2TeoremaPifagoraPassed)
        {
            Lesson2TeoremaPifagoraPassed = true;
            Rank += Lesson2TeoremaPifagoraValue;
            SaveState();
            CheckAchievements();
        }
    }

    public void Notation2EgipetskiyTreugolnikSetter()
    {
        if (!Lesson2EgipetskiyTreugolnikPassed)
        {
            Lesson2EgipetskiyTreugolnikPassed = true;
            Rank += Lesson2EgipetskiyTreugolnikValue;
            SaveState();
            CheckAchievements();
        }
    }

    public void Notation2ReadingSetter()
    {
        if (!Lesson2ReadingPassed)
        {
            Lesson2ReadingPassed = true;
            Rank += Lesson2ReadingValue;
            SaveState();
        }
    }

    public void Notation1EdiniciDecatkiSotniSetter()
    {
        if(!Lesson1EdiniciDecatkiSotniPassed)
        {
            Lesson1EdiniciDecatkiSotniPassed = true;
            Rank += Lesson1EdiniciDecatkiSotniValue;
            SaveState();
            CheckAchievements();
        }
    }
    public void Notation1TiciachiSetter()
    {
        if (!Lesson1TiciachiPassed)
        {
            Lesson1TiciachiPassed = true;
            Rank += Lesson1TiciachiValue;
            SaveState();
        }
    }
    
    public void Notation1AlikvotnieDrobiSetter()
    {
        if (!Lesson1AlikvotnieDrobiPassed)
        {
            Lesson1AlikvotnieDrobiPassed = true;
            Rank += Lesson1AlikvotnieDrobiValue;
            SaveState();
        }
    }


    public void Notation1PrimeriSetter()
    {
        if (!Lesson1PrimeriPassed)
        {
            Lesson1PrimeriPassed = true;
            Rank += Lesson1PrimeriValue;
            SaveState();
        }
    }
    
    public void Notation1ReadingSetter()
    { 
        if(!Lesson1ReadingPassed)
        {
            Lesson1ReadingPassed = true;
            Rank += Lesson1ReadingValue;
            SaveState();
        }
    }

    public void Notation3ShadufSetter()
    {
        if (!Lesson3ShadufPassed)
        {
            Lesson3ShadufPassed = true;
            Rank += Lesson3ShadufValue;
             SaveState();
            CheckAchievements();
        }
    }

    public void Notation3PodemVodiSetter()
    {
        if (!Lesson3PodemVodiCherezShadufPassed)
        {
            Lesson3PodemVodiCherezShadufPassed = true;
            Rank += Lesson3PodemVodiCherezShadufValue;
            SaveState();
            CheckAchievements();
        }
    }

    public void Notation3ShluziSetter()
    {
        if (!Lesson3ShluziPassed)
        {
            Lesson3ShluziPassed = true;
            Rank += Lesson3ShluziValue;
            SaveState();
            CheckAchievements();
        }
    }

    public void Notation3VodniePutiSetter()
    {
        if (!Lesson3VodniePutiPyramidPassed)
        {
            Lesson3VodniePutiPyramidPassed = true;
            Rank += Lesson3VodniePutiPyramidValue;
            SaveState();
            CheckAchievements();
        }
    }

    public void Notation3ReadingSetter()
    {
        if (!Lesson3ReadingPassed)
        {
            Lesson3ReadingPassed = true;
            Rank += Lesson3ReadingValue;
            SaveState();
            CheckAchievements();
        }
    }

    public void Notation4SiriusSetter()
    {
        if (!Lesson4SiriusAPassed)
        {
            Lesson4SiriusAPassed = true;
            Rank += Lesson4SiriusValue;
            SaveState();
            CheckAchievements();
        }
    }

    public void Notation4SiriusAAndSunSetter()
    {
        if (!Lesson4SiriusAAndSunPassed)
        {
            Lesson4SiriusAAndSunPassed = true;
            Rank += Lesson4SiriusAAndSunValue;
            SaveState();
            CheckAchievements();
        }
    }

    public void Notation4DvoynayaZvezdaSetter()
    {
        if (!Lesson4DvoynayaZvezdaPassed)
        {
            Lesson4DvoynayaZvezdaPassed = true;
            Rank += Lesson4DvoynayaZvezdaValue;
            SaveState();
            CheckAchievements();
        }
    }

    public void Notation4SozvezdieBolshoyPesSetter()
    {
        if (!Lesson4SozvezdieBolshoyPesPassed)
        {
            Lesson4SozvezdieBolshoyPesPassed = true;
            Rank += Lesson4SozvezdieBolshoyPesValue;
            SaveState();
            CheckAchievements();
        }
    }

    public void Notation4ReadingSetter()
    {
        if (!Lesson4ReadingPassed)
        {
            Lesson4ReadingPassed = true;
            Rank += Lesson4ReadingValue;
            SaveState();
            CheckAchievements();
        }
    }

    public void Notation5MoscowMathPapirusSetter()
    {
        if (!Lesson5MoscowMathPapirusPassed)
        {
            Lesson5MoscowMathPapirusPassed = true;
            Rank += Lesson5MoscowMathPapirusValue;
            SaveState();
            CheckAchievements();
        }
    }

    public void Notation5PapirusAhmesaSetter()
    {
        if (!Lesson5PapirusAhmesaPassed)
        {
            Lesson5PapirusAhmesaPassed = true;
            Rank += Lesson5PapirusAhmesaValue;
            SaveState();
            CheckAchievements();
        }
    }

    public void Notation5EgypetskayaPloshadChetirehugolnikaSetter()
    {
        if (!Lesson5EgypetskayaPloshadChetirehugolnikaPassed)
        {
            Lesson5EgypetskayaPloshadChetirehugolnikaPassed = true;
            Rank += Lesson5EgypetskayaPloshadChetirehugolnikaValue;
            SaveState();
            CheckAchievements();
        }
    }

    public void Notation5ChisloPiSetter()
    {
        if (!Lesson5ChisloPiPassed)
        {
            Lesson5ChisloPiPassed = true;
            Rank += Lesson5ChisloPiValue;
            SaveState();
            CheckAchievements();
        }
    }

    public void Notation5KakPoyavilosChisloPiSetter()
    {
        if (!Lesson5KakPoyavilosChisloPiPassed)
        {
            Lesson5KakPoyavilosChisloPiPassed = true;
            Rank += Lesson5KakPoyavilosChisloPiValue;
            SaveState();
            CheckAchievements();
        }
    }

    public void Notation5ReadingSetter()
    {
        if (!Lesson5ReadingPassed)
        {
            Lesson5ReadingPassed = true;
            Rank += Lesson5ReadingValue;
            SaveState();
            CheckAchievements();
        }
    }

    public void Notation6PicesSetter()
    {
        if (!Lesson6PicesPassed)
        {
            Lesson6PicesPassed = true;
            Rank += Lesson6PicesValue;
            SaveState();
            CheckAchievements();
        }
    }

    public void Notation6ReadingSetter()
    {
        if (!Lesson6ReadingPassed)
        {
            Lesson6ReadingPassed = true;
            Rank += Lesson6ReadingValue;
            SaveState();
            CheckAchievements();
        }
    }
}