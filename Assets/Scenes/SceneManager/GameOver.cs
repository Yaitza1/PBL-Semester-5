using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameOver : MonoBehaviour
{
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject firstSelectedButton;
    public static bool isGameOver;
    private static GameOver instance;
    
    private void Awake()
    {
        if (gameOverUI == null)
        {
            Debug.Log("GameOverUI tidak ditemukan!");
            return;
        }

        // Pastikan gameOverUI tidak aktif saat game dimulai
        gameOverUI.SetActive(false);
        isGameOver = false;
    }

    void Start()
    {
        // Sembunyikan game over screen saat memulai
        HideGameOver();
    }

    public void ShowGameOver()
    {
        try
        {
            if (gameOverUI != null)
            {
                gameOverUI.SetActive(true);
                Time.timeScale = 0f;
                isGameOver = true;

                // Set selected button untuk navigasi keyboard/controller
                if (firstSelectedButton != null)
                {
                    EventSystem.current.SetSelectedGameObject(firstSelectedButton);
                }

                // Tampilkan kursor saat game over
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }
        catch (System.Exception e)
        {
            Debug.Log($"Error saat menampilkan Game Over: {e.Message}");
        }
    }

    private void HideGameOver()
    {
        try
        {
            if (gameOverUI != null)
            {
                gameOverUI.SetActive(false);
                Time.timeScale = 1f;
                isGameOver = false;

                // Sembunyikan kursor
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
        catch (System.Exception e)
        {
            Debug.Log($"Error saat menyembunyikan Game Over: {e.Message}");
        }
    }

    public void Restart()
    {
        try
        {
            Debug.Log("Restart Button Clicked!");
            Time.timeScale = 1f;
            isGameOver = false;
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
        }
        catch (System.Exception e)
        {
            Debug.Log($"Error saat Restart: {e.Message}");
        }
    }

    public void MainMenu()
    {
        try
        {
            Time.timeScale = 1f;
            isGameOver = false;
            SceneManager.LoadScene(0);
        }
        catch (System.Exception e)
        {
            Debug.Log($"Error saat ke MainMenu: {e.Message}");
        }
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }
}