using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;
    public GameObject enemyPrefab;
    public GameObject closeEnemyPrefab;
    [Range(0, 1)] public float closeEnemySpawnChance = 0.3f;

    public float spawnRadius = 10f;
    public float spawnInterval = 2f;
    public int maxEnemies = 10;

    public float minSpawnX = -5f;
    public float maxSpawnX = 5f;
    public float minSpawnZ = -5f;
    public float maxSpawnZ = 5f;

    private Camera mainCamera;
    private int currentEnemies = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        mainCamera = Camera.main;
        InvokeRepeating(nameof(SpawnEnemy), spawnInterval, spawnInterval);
    }

    void SpawnEnemy()
    {
        if (currentEnemies >= maxEnemies) return;

        Vector3 spawnPosition;
        do
        {
            spawnPosition = new Vector3(
                Random.Range(minSpawnX, maxSpawnX),
                0,
                Random.Range(minSpawnZ, maxSpawnZ)
            );
        }
        while (IsPositionInCameraView(spawnPosition));

        GameObject enemyToSpawn = Random.value < closeEnemySpawnChance ? closeEnemyPrefab : enemyPrefab;
        Instantiate(enemyToSpawn, spawnPosition, Quaternion.identity);
        currentEnemies++;
    }

    bool IsPositionInCameraView(Vector3 position)
    {
        if (mainCamera == null) return false;
        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(position);
        return viewportPoint.x >= 0 && viewportPoint.x <= 1 && viewportPoint.y >= 0 && viewportPoint.y <= 1 && viewportPoint.z > 0;
    }

    public void EnemyDestroyed()
    {
        currentEnemies--;
    }
}