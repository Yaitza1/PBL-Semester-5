using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EnemySpawner : MonoBehaviour
{
    // Kelas untuk mendefinisikan properti setiap wave
    [System.Serializable]
    public class Wave
    {
        public int numberOfEnemies;
        public float delayBeforeWave;
    }

    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Wave[] waves;
    [SerializeField] private float minSpawnRadius = 15f;
    [SerializeField] private float maxSpawnRadius = 25f;
    [SerializeField] private float minDistanceFromPlayer = 15f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Text waveDisplayText;
    [SerializeField] private float waveDisplayDuration = 5f;

    public UnityEvent onWaveComplete;
    public UnityEvent onAllWavesComplete;

    [Header("Audio")]
    public AudioClip zombieSound;
    public AudioSource zombieAudioSource;

    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private Transform playerTransform;
    private int enemyCounter = 1;
    private int currentWave = 0;
    private Coroutine waveDisplayCoroutine;

    private void Start()
    {
        Debug.Log("EnemySpawner: Start method called");
        
        // Mencari player di scene
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        if (playerTransform == null)
        {
            Debug.LogError("EnemySpawner: Player not found! Make sure your player has the 'Player' tag.");
            return;
        }

        // Memeriksa apakah waveDisplayText telah di-assign
        if (waveDisplayText == null)
        {
            Debug.LogError("EnemySpawner: Wave display Text component is not assigned!");
        }
        else
        {
            waveDisplayText.gameObject.SetActive(false); // Sembunyikan teks di awal
        }

        Debug.Log($"EnemySpawner: onWaveComplete listeners: {onWaveComplete.GetPersistentEventCount()}");
        Debug.Log($"EnemySpawner: onAllWavesComplete listeners: {onAllWavesComplete.GetPersistentEventCount()}");

        StartCoroutine(SpawnWaves());
    }

    private IEnumerator SpawnWaves()
    {
        Debug.Log("EnemySpawner: Starting SpawnWaves coroutine");
        for (int i = 0; i < waves.Length; i++)
        {
            currentWave = i;
            ShowWaveDisplay(); // Tampilkan informasi wave baru
            
            Debug.Log($"EnemySpawner: Preparing to spawn Wave {i + 1}");
            yield return new WaitForSeconds(waves[i].delayBeforeWave);
            Debug.Log($"EnemySpawner: Delay before Wave {i + 1} complete, starting spawn");
            yield return StartCoroutine(SpawnWave(i));

            // Play zombie sound
            if (zombieSound != null && zombieAudioSource != null)
            {
                zombieAudioSource.PlayOneShot(zombieSound);
            }
            
            Debug.Log($"EnemySpawner: Wave {i + 1} spawned, waiting for zombies to be defeated");
            while (spawnedEnemies.Count > 0)
            {
                spawnedEnemies.RemoveAll(enemy => enemy == null);
                Debug.Log($"EnemySpawner: Remaining Zombie in Wave {i + 1} = {spawnedEnemies.Count}");
                yield return new WaitForSeconds(1f);
            }

            Debug.Log($"EnemySpawner: Wave {currentWave + 1} completed!");
            onWaveComplete?.Invoke();

            if (i == waves.Length - 1)
            {
                Debug.Log("EnemySpawner: All waves completed!");
                onAllWavesComplete?.Invoke();
            }
        }
    }

    private IEnumerator SpawnWave(int waveIndex)
    {
        Debug.Log($"EnemySpawner: Spawning Wave {waveIndex + 1} with {waves[waveIndex].numberOfEnemies} Zombie");
        for (int i = 0; i < waves[waveIndex].numberOfEnemies; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(0.5f);
        }
        Debug.Log($"EnemySpawner: Finished spawning all Zombie for Wave {waveIndex + 1}");
    }

    private void SpawnEnemy()
    {
        Vector3 spawnPosition = GetValidSpawnPosition();
        if (spawnPosition != Vector3.zero)
        {
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            enemy.name = $"Zombie {enemyCounter} (Wave {currentWave + 1})";
            enemyCounter++;
            spawnedEnemies.Add(enemy);
            Debug.Log($"EnemySpawner: Spawned {enemy.name} at position {spawnPosition}");
        }
        else
        {
            Debug.LogWarning($"EnemySpawner: Could not find a valid spawn position for Enemy {enemyCounter} (Wave {currentWave + 1})");
            enemyCounter++;
        }
    }

    private Vector3 GetValidSpawnPosition()
    {
        Vector3 spawnPosition;
        int maxAttempts = 30;

        for (int i = 0; i < maxAttempts; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle.normalized * Random.Range(minSpawnRadius, maxSpawnRadius);
            spawnPosition = transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);

            if (Vector3.Distance(spawnPosition, playerTransform.position) < minDistanceFromPlayer)
            {
                continue;
            }

            if (Physics.Raycast(spawnPosition + Vector3.up * 10f, Vector3.down, out RaycastHit hit, 20f, groundLayer))
            {
                return hit.point;
            }
        }

        Debug.LogWarning("EnemySpawner: Failed to find valid spawn position after max attempts");
        return Vector3.zero;
    }

    // Metode untuk menampilkan informasi wave
    private void ShowWaveDisplay()
    {
        if (waveDisplayCoroutine != null)
        {
            StopCoroutine(waveDisplayCoroutine);
        }
        waveDisplayCoroutine = StartCoroutine(ShowWaveDisplayCoroutine());
    }

    // Coroutine untuk menampilkan dan menyembunyikan informasi wave
    private IEnumerator ShowWaveDisplayCoroutine()
    {
        if (waveDisplayText != null)
        {
            waveDisplayText.text = $"Wave: {currentWave + 1} / {waves.Length}";
            waveDisplayText.gameObject.SetActive(true);
            yield return new WaitForSeconds(waveDisplayDuration);
            waveDisplayText.gameObject.SetActive(false);
        }
    }

    public void RestartWaves()
    {
        Debug.Log("EnemySpawner: Restarting waves");
        StopAllCoroutines();
        DespawnAllEnemies();
        currentWave = 0;
        enemyCounter = 1;
        StartCoroutine(SpawnWaves());
    }

    private void DespawnAllEnemies()
    {
        Debug.Log($"EnemySpawner: Despawning all Zombies. Current count: {spawnedEnemies.Count}");
        foreach (GameObject enemy in spawnedEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
                Debug.Log($"EnemySpawner: Despawned {enemy.name}");
            }
        }
        spawnedEnemies.Clear();
        Debug.Log("EnemySpawner: All Zombies despawned");
    }

    // Optional: Add this method to manually trigger wave completion (for testing)
    public void ForceCompleteCurrentWave()
    {
        Debug.Log($"EnemySpawner: Forcing completion of Wave {currentWave + 1}");
        DespawnAllEnemies();
    }
}