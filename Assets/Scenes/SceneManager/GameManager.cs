using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public EnemySpawner enemySpawner;
    public string NextSceneName;
    public GameObject boss; // Referensi ke prefab boss (ditambahkan)
    private bool isBossDefeated = false; // Flag untuk status boss (ditambahkan)
    private bool canLoadNextScene = true; // Flag untuk kontrol LoadNextScene

    void Start()
    {
        if (enemySpawner != null)
        {
            enemySpawner.onWaveComplete.AddListener(OnWaveCompleted);
            enemySpawner.onAllWavesComplete.AddListener(OnAllWavesCompleted);
        }
        else
        if (enemySpawner == null)
        {
            Debug.Log("No EnemySpawner found. This Scene must be a Cutscene");
            float delay = 10f;
            switch (SceneManager.GetActiveScene().name)
            {
                case "Cutscene 0":
                case "Cutscene 1":
                case "Cutscene 2":
                    delay = 15f;
                    break;
                case "Cutscene 3":
                    delay = 25f;
                    break;
            }
            StartCoroutine(LoadNextSceneWithDelay(delay));
        }

        // Cek apakah ini Level 3
        if (SceneManager.GetActiveScene().name == "Level 3")
        {
            canLoadNextScene = false; // Matikan LoadNextScene untuk Level 3
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

        // Mengaktifkan kembali BossSpawner
        BossSpawner bossSpawner = FindObjectOfType<BossSpawner>();
        if (bossSpawner != null)
        {
            bossSpawner.EnableSpawner();
            Debug.Log("BossSpawner has been re-enabled by GameManager.");
        }

        if (SceneManager.GetActiveScene().name == "Level 3")
        {
            // Jika Level 3, pastikan LoadNextScene baru berjalan setelah boss dikalahkan
            if (boss != null && !isBossDefeated)
            {
                StartCoroutine(CheckBossDefeated());
            }
        }
        else
        {
            StartCoroutine(LoadNextScene());
        }
    }

    void OnEnable()
    {
        // Daftarkan event untuk mendeteksi kekalahan musuh
        EnemyHealth.OnEnemyDefeated += HandleEnemyDefeated;
    }

    void OnDisable()
    {
        // Hapus pendaftaran event
        EnemyHealth.OnEnemyDefeated -= HandleEnemyDefeated;
    }

    private void HandleEnemyDefeated(GameObject enemy)
    {
        // Cek apakah musuh yang dikalahkan adalah boss
        if (enemy.CompareTag("Boss"))
        {
            Debug.Log("Boss defeated!");
            isBossDefeated = true;
        }
    }

    private IEnumerator CheckBossDefeated()
    {
        // Tunggu hingga boss dikalahkan
        while (!isBossDefeated)
        {
            yield return null;
        }

        // Beri delay sebelum memuat scene berikutnya
        yield return new WaitForSeconds(3f);

        canLoadNextScene = true; // Izinkan LoadNextScene
        StartCoroutine(LoadNextScene());
    }

    private IEnumerator LoadNextSceneWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(LoadNextScene());
    }

    private IEnumerator LoadNextScene()
    {
        if (!canLoadNextScene) yield break; // Jangan lakukan apapun jika LoadNextScene belum diizinkan

        Debug.Log("Loading next scene...");
        yield return StartCoroutine(SceneTransition.Instance.FadeIn());

        if (!string.IsNullOrEmpty(NextSceneName))
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(NextSceneName);

            // Tunggu hingga scene selesai dimuat
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

        // Fade out setelah scene baru dimuat
        yield return StartCoroutine(SceneTransition.Instance.FadeOut());
    }
}