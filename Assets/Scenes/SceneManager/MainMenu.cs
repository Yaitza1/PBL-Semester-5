using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private float lastMouseMoveTime;
    private float cursorHideDelay = 2.0f;
    
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        lastMouseMoveTime = Time.time;
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

    public void PlayGame()
    {
        SceneManager.LoadSceneAsync("Level 1");
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
