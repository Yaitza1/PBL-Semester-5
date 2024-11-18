using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BossSpawner : MonoBehaviour
{
    public GameObject bossPrefab; // Prefab boss
    public Transform bossSpawnPoint; // Lokasi spawn boss
    private EnemySpawner enemySpawner;
    private bool isBossSpawnerActive = false; // Flag untuk mengatur aktivasi skrip
    private bool isBossSpawned = false; // Flag untuk memastikan boss hanya spawn sekali

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
            GameObject spawnedBoss = Instantiate(bossPrefab, bossSpawnPoint.position, Quaternion.identity);
            spawnedBoss.tag = "Boss"; // Tambahkan tag untuk boss
            Debug.Log("Boss Spawned!");
        }
        else
        {
            Debug.LogError("Boss prefab or spawn point is not assigned.");
        }
    }
}