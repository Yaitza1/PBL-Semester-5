using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BossSpawner : MonoBehaviour
{
    public GameObject bossPrefab; // Prefab boss
    public Transform bossSpawnPoint; // Lokasi spawn boss

    [Header("UI References")]
    [SerializeField] private Text waveDisplayText;
    [SerializeField] private Text zombieCountText;
    [SerializeField] private Image zombieImage;
    [SerializeField] private float waveDisplayDuration = 5f;

    private EnemySpawner enemySpawner;
    private bool isBossSpawnerActive = false; // Flag untuk mengatur aktivasi skrip
    private bool isBossSpawned = false; // Flag untuk memastikan boss hanya spawn sekali
    private Coroutine waveDisplayCoroutine;

    public void EnableSpawner()
    {
        isBossSpawnerActive = true;
        Debug.Log("BossSpawner Script is now active.");
    }

    private void Start()
    {
        // Pastikan flag diinisialisasi dengan false
        isBossSpawnerActive = false;

        // Cari komponen EnemySpawner di scene
        enemySpawner = FindObjectOfType<EnemySpawner>();
        if (enemySpawner == null)
        {
            Debug.LogError("EnemySpawner not found in the scene.");
        }

        // Validasi UI References
        ValidateUIReferences();
    }

    private void ValidateUIReferences()
    {
        if (waveDisplayText == null)
        {
            Debug.LogError("BossSpawner: Wave display Text component is not assigned!");
        }
        else
        {
            waveDisplayText.gameObject.SetActive(false);
        }

        if (zombieCountText == null)
        {
            Debug.LogError("BossSpawner: Zombie count Text component is not assigned!");
        }
        else
        {
            zombieCountText.gameObject.SetActive(false);
        }

        if (zombieImage == null)
        {
            Debug.LogError("BossSpawner: Zombie Image component is not assigned!");
        }
        else
        {
            zombieImage.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        // Pastikan hanya berjalan jika BossSpawner diaktifkan
        if (isBossSpawnerActive)
        {
            if (enemySpawner != null && enemySpawner.transform.childCount == 0 && !isBossSpawned)
            {
                StartCoroutine(CheckForWavesAndSpawnBoss());
            }
        }
    }

    private IEnumerator CheckForWavesAndSpawnBoss()
    {
        // Pastikan coroutine hanya berjalan sekali
        isBossSpawned = true;

        // Tunggu 5 detik untuk memastikan tidak ada wave baru
        yield return new WaitForSeconds(5f);

        // Cek ulang: jika EnemySpawner masih tidak memiliki musuh, spawn boss
        if (enemySpawner.transform.childCount == 0 && SceneManager.GetActiveScene().name == "Level 3")
        {
            SpawnBoss();
        }
        else
        {
            isBossSpawned = false; // Reset flag jika ada wave baru yang dimulai
        }
    }

    private void SpawnBoss()
    {
        if (bossPrefab != null && bossSpawnPoint != null)
        {
            // Tampilkan wave display
            ShowBossWaveDisplay();

            GameObject spawnedBoss = Instantiate(bossPrefab, bossSpawnPoint.position, Quaternion.identity);
            spawnedBoss.name = "boss level 3";
            spawnedBoss.tag = "Boss"; // Tambahkan tag untuk boss

            // Tampilkan zombie count dan image
            SetZombieUIVisibility(true);
            UpdateZombieCountDisplay(1);

            Debug.Log("Boss Spawned!");
        }
        else
        {
            Debug.LogError("Boss prefab or spawn point is not assigned.");
        }
    }

    private void ShowBossWaveDisplay()
    {
        if (waveDisplayCoroutine != null)
        {
            StopCoroutine(waveDisplayCoroutine);
        }
        waveDisplayCoroutine = StartCoroutine(ShowBossWaveDisplayCoroutine());
    }

    private IEnumerator ShowBossWaveDisplayCoroutine()
    {
        if (waveDisplayText != null)
        {
            waveDisplayText.text = "BOSS WAVE";
            waveDisplayText.gameObject.SetActive(true);
            yield return new WaitForSeconds(waveDisplayDuration);
            waveDisplayText.gameObject.SetActive(false);
        }
    }

    private void SetZombieUIVisibility(bool visible)
    {
        if (zombieImage != null)
        {
            zombieImage.gameObject.SetActive(visible);
        }
        if (zombieCountText != null)
        {
            zombieCountText.gameObject.SetActive(visible);
        }
    }

    private void UpdateZombieCountDisplay(int count)
    {
        if (zombieCountText != null)
        {
            zombieCountText.text = $"X {count}";
        }
    }
}