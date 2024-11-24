using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    public static bool isPaused;
    private static PauseMenu instance;
    
    private void Awake()
    {
        // Pastikan referensi pauseMenuUI tidak null
        if (pauseMenuUI == null)
        {
            Debug.LogError("PauseMenuUI tidak ditemukan!");
            return;
        }
    }

    void Start()
    {
        ResumeGame();
    }

    void Update()
    {
        // Hanya aktifkan kontrol pause jika UI reference valid
        if (pauseMenuUI != null)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (isPaused)
                {
                    ResumeGame();
                }
                else
                {
                    PauseGame();
                }
            }
        }
    }

    public void ResumeGame()
    {
        try
        {
            if (pauseMenuUI != null)
            {
                pauseMenuUI.SetActive(false);
                Time.timeScale = 1f;
                isPaused = false;

                // Sembunyikan kursor saat game berjalan
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error saat Resume: {e.Message}");
        }
    }

    public void PauseGame()
    {
        try
        {
            if (pauseMenuUI != null)
            {
                pauseMenuUI.SetActive(true);
                Time.timeScale = 0f;
                isPaused = true;

                // Tampilkan kursor saat game dipause
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error saat Pause: {e.Message}");
        }
    }

    public void Restart()
    {
        try
        {
            Debug.Log("Restart Button Clicked!");
            Time.timeScale = 1f;
            isPaused = false;
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error saat Restart: {e.Message}");
        }
    }

    public void MainMenu()
    {
        try
        {
            Time.timeScale = 1f;
            isPaused = false;
            SceneManager.LoadScene(0);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error saat ke MainMenu: {e.Message}");
        }
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }
}