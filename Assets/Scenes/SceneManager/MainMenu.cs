using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Dropdown qualityDropdown;
    public Slider volumeSlider; // Slider untuk volume
    public AudioSource[] gameAudioSources; // Array untuk semua audio dalam game

    private float lastMouseMoveTime;
    private float cursorHideDelay = 2.0f;
    private const string GRAPHICS_QUALITY_KEY = "GraphicsQuality";
    private const string VOLUME_KEY = "GameVolume"; // Key untuk volume di PlayerPrefs
    private const int DEFAULT_QUALITY = 5; // Ultra quality as default
    private const float DEFAULT_VOLUME = 0.5f; // Volume default

    void Start()
    {
        InitializeQualitySettings();
        SetupQualityDropdown();
        InitializeVolumeSettings(); // Inisialisasi volume

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

    private void InitializeVolumeSettings()
    {
        // Atur slider volume ke nilai tersimpan atau default
        float savedVolume = PlayerPrefs.GetFloat(VOLUME_KEY, DEFAULT_VOLUME);
        volumeSlider.value = savedVolume;
        ApplyVolume(savedVolume);

        // Tambahkan listener untuk slider
        volumeSlider.onValueChanged.AddListener(SetVolume);
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

    public void SetVolume(float volume)
    {
        // Simpan volume di PlayerPrefs dan terapkan
        PlayerPrefs.SetFloat(VOLUME_KEY, volume);
        PlayerPrefs.Save();
        ApplyVolume(volume);
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

        // Sembunyikan kursor jika tidak ada gerakan dalam waktu tertentu
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
}