using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public EnemySpawner enemySpawner;
    public string NextSceneName;

    void Start()
    {
        if (enemySpawner != null)
        {
            enemySpawner.onWaveComplete.AddListener(OnWaveCompleted);
            enemySpawner.onAllWavesComplete.AddListener(OnAllWavesCompleted);
        }
        else
        {
            Debug.Log("No EnemySpawner found. This Scene must be a Cutscene");
            StartCoroutine(LoadNextScene());
        }

        // Start with fade out effect
        StartCoroutine(SceneTransition.Instance.FadeOut());
    }

    public void OnWaveCompleted()
    {
        Debug.Log("Good Job! Keep Surviving and Reach The End");
    }

    public void OnAllWavesCompleted()
    {
        Debug.Log("Congrats For surviving This Level");
        StartCoroutine(LoadNextScene());
    }

    private IEnumerator LoadNextScene()
    {
        Debug.Log("Loading next scene...");
        yield return StartCoroutine(SceneTransition.Instance.FadeIn());

        if (!string.IsNullOrEmpty(NextSceneName))
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(NextSceneName);

            // Wait until the asynchronous scene fully loads
            while (!asyncLoad.isDone)
            {
                float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
                Debug.Log($"Loading progress: {progress * 100}%");
                yield return null;
            }
        }
        else
        {
            Debug.LogError("Next scene name is not set in the GameManager!");
        }

        // Fade out after new scene is loaded
        yield return StartCoroutine(SceneTransition.Instance.FadeOut());
    }
}

