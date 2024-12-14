using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Tambahkan ini untuk EventSystem

public class MainMenu : MonoBehaviour
{
    public Dropdown qualityDropdown;
    public Dropdown resolutionDropdown; // Dropdown untuk memilih resolusi
    public Toggle FullscreenToggle; // Toggle untuk fullscreen
    public Slider volumeSlider; // Slider untuk volume
    public AudioSource[] gameAudioSources; // Array untuk semua audio dalam game
    public EventSystem eventSystem; // Referensi ke EventSystem
    public GameObject resolutionDropdownObject; // Referensi ke GameObject Resolution Dropdown

    private float lastMouseMoveTime;
    private float cursorHideDelay = 2.0f;
    private const string GRAPHICS_QUALITY_KEY = "GraphicsQuality";
    private const string FULLSCREEN_KEY = "Fullscreen"; // Key untuk fullscreen di PlayerPrefs
    private const string RESOLUTION_KEY = "GameResolution"; // Key untuk resolusi di PlayerPrefs
    private const string VOLUME_KEY = "GameVolume"; // Key untuk volume di PlayerPrefs
    private const int DEFAULT_QUALITY = 5; // Ultra quality as default
    private const float DEFAULT_VOLUME = 0.5f; // Volume default
    private const int DEFAULT_RESOLUTION_INDEX = 1; // Default index untuk resolusi 1920x1080

    private Resolution[] predefinedResolutions = new Resolution[]
    {
        new Resolution { width = 1280, height = 720 },
        new Resolution { width = 1920, height = 1080 },
        new Resolution { width = 2560, height = 1440 },
        new Resolution { width = 3840, height = 2160 }
    };

    void Start()
    {
        InitializeQualitySettings();
        SetupQualityDropdown();
        InitializeResolutionSettings(); // Inisialisasi resolusi
        InitializeVolumeSettings(); // Inisialisasi volume
        InitializeFullscreenSettings(); // Inisialisasi fullscreen


        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        lastMouseMoveTime = Time.time;
        StartCoroutine(SceneTransition.Instance.FadeOut());
    }

    private void InitializeQualitySettings()
    {
        int savedQuality = PlayerPrefs.GetInt(GRAPHICS_QUALITY_KEY, DEFAULT_QUALITY);
        QualitySettings.SetQualityLevel(savedQuality);
    }

    private void SetupQualityDropdown()
    {
        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(new List<string>(QualitySettings.names));
        qualityDropdown.value = QualitySettings.GetQualityLevel();
        qualityDropdown.onValueChanged.AddListener(SetQuality);
    }

    private void InitializeResolutionSettings()
    {
        List<string> options = new List<string>();
        int defaultResolutionIndex = DEFAULT_RESOLUTION_INDEX;

        for (int i = 0; i < predefinedResolutions.Length; i++)
        {
            Resolution res = predefinedResolutions[i];
            string option = res.width + " x " + res.height;
            options.Add(option);
        }

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(options);

        int savedResolutionIndex = PlayerPrefs.GetInt(RESOLUTION_KEY, defaultResolutionIndex);
        resolutionDropdown.value = savedResolutionIndex;
        resolutionDropdown.onValueChanged.AddListener(SetResolution);

        SetResolution(savedResolutionIndex);
    }

    private void InitializeVolumeSettings()
    {
        // Atur slider volume ke nilai tersimpan atau default
        float savedVolume = PlayerPrefs.GetFloat(VOLUME_KEY, DEFAULT_VOLUME);
        volumeSlider.value = savedVolume;
        ApplyVolume(savedVolume);

        // Tambahkan listener untuk slider
        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    private void InitializeFullscreenSettings()
    {
        bool isFullscreen = PlayerPrefs.GetInt(FULLSCREEN_KEY, 1) == 1; // Default fullscreen aktif
        Screen.fullScreen = isFullscreen;
        FullscreenToggle.isOn = isFullscreen;
        FullscreenToggle.onValueChanged.AddListener(SetFullscreen);
    }

    private void ApplyVolume(float volume)
    {
        if (gameAudioSources == null || gameAudioSources.Length == 0)
        {
            return; // Tidak ada audio source untuk diproses
        }

        // Terapkan volume ke semua sumber audio di array
        foreach (AudioSource audioSource in gameAudioSources)
        {
            if (audioSource != null)
            {
                audioSource.volume = volume;
            }
        }
    }

    public void SetResolution(int resolutionIndex)
    {
        if (resolutionIndex >= 0 && resolutionIndex < predefinedResolutions.Length)
        {
            Resolution selectedResolution = predefinedResolutions[resolutionIndex];
            Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreen);

            // Simpan resolusi ke PlayerPrefs
            PlayerPrefs.SetInt(RESOLUTION_KEY, resolutionIndex);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.LogWarning("Invalid resolution index: " + resolutionIndex);
        }
    }

    public void SetVolume(float volume)
    {
        // Simpan volume di PlayerPrefs dan terapkan
        PlayerPrefs.SetFloat(VOLUME_KEY, volume);
        PlayerPrefs.Save();
        ApplyVolume(volume);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt(FULLSCREEN_KEY, isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    void Update()
    {
        // Deteksi gerakan mouse
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            lastMouseMoveTime = Time.time;
            if (!Cursor.visible)
            {
                Cursor.visible = true; // Perlihatkan kursor jika sebelumnya tersembunyi
            }
        }

        // Sembunyikan kursor jika tidak ada gerakan selama waktu tertentu
        if (Time.time - lastMouseMoveTime > cursorHideDelay)
        {
            Cursor.visible = false;
        }
    }

    public void PlayGame(string sceneName)
    {
        LoadingScreen.Instance.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void setFullScreen(bool fullScreen)
    {
        // Atur mode layar penuh
        Screen.fullScreen = fullScreen;
    }

    public void SetQuality(int qualityIndex)
    {
        if (qualityIndex >= 0 && qualityIndex < QualitySettings.names.Length)
        {
            QualitySettings.SetQualityLevel(qualityIndex);
            PlayerPrefs.SetInt(GRAPHICS_QUALITY_KEY, qualityIndex);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.LogWarning("Invalid quality index: " + qualityIndex);
        }
    }

    // fungsi ini untuk mengatur focus ke Resolution Dropdown
    public void OpenSettings()
    {
        eventSystem.SetSelectedGameObject(resolutionDropdownObject);
    }
}