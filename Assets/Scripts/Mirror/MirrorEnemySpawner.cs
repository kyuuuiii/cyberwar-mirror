using Mirror;
using UnityEngine;

public class MirrorEnemySpawner : NetworkBehaviour
{
    public static MirrorEnemySpawner Instance;
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
    [SyncVar] private int currentEnemies = 0;

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

    public override void OnStartServer()
    {
        base.OnStartServer();
        mainCamera = Camera.main;
        InvokeRepeating(nameof(SpawnEnemy), spawnInterval, spawnInterval);
    }

    [Server]
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
        GameObject enemy = Instantiate(enemyToSpawn, spawnPosition, Quaternion.identity);
        NetworkServer.Spawn(enemy);
        currentEnemies++;
    }

    bool IsPositionInCameraView(Vector3 position)
    {
        if (mainCamera == null) return false;
        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(position);
        return viewportPoint.x >= 0 && viewportPoint.x <= 1 && viewportPoint.y >= 0 && viewportPoint.y <= 1 && viewportPoint.z > 0;
    }

    [Server]
    public void EnemyDestroyed()
    {
        currentEnemies--;
    }
}