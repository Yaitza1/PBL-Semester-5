using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public EnemySpawner enemySpawner;
    public string NextSceneName; // Name of the next scene to load

    void Start()
    {
        if (enemySpawner != null)
        {
            enemySpawner.onWaveComplete.AddListener(OnWaveCompleted);
            enemySpawner.onAllWavesComplete.AddListener(OnAllWavesCompleted);
        }
    }

    public void OnWaveCompleted()
    {
        Debug.Log("Good Job! Keep Surviving and Reach The End");
        // Add other logic here, such as displaying UI or giving rewards
    }

    public void OnAllWavesCompleted()
    {
        Debug.Log("Congrats For surviving This Level");
        StartCoroutine(LoadNextScene());
    }

    private IEnumerator LoadNextScene()
    {
        Debug.Log("Loading next scene...");
        yield return new WaitForSeconds(2f); // Optional delay before loading the next scene

        if (!string.IsNullOrEmpty(NextSceneName))
        {
            SceneManager.LoadScene(NextSceneName);
        }
        else
        {
            Debug.LogError("Next scene name is not set in the GameManager!");
        }
    }
}