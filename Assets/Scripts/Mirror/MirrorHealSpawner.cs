using Mirror;
using UnityEngine;

public class MirrorHealSpawner : NetworkBehaviour
{
    public static MirrorHealSpawner Instance;

    public float minSpawnX = -85f;
    public float maxSpawnX = 60f;
    public float minSpawnZ = -22f;
    public float maxSpawnZ = 120f;
    public int maxHeals = 1;
    public GameObject healPrefab;
    public float spawnInterval = 5f;

    [SyncVar]
    private int currentHeals = 0;

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

        InvokeRepeating(nameof(SpawnHeal), 1f, spawnInterval);
    }

    [Server]
    public void SpawnHeal()
    {
        if (currentHeals >= maxHeals) return;

        Vector3 spawnPosition = new Vector3(
            Random.Range(minSpawnX, maxSpawnX),
            -0.7f,
            Random.Range(minSpawnZ, maxSpawnZ)
        );

        GameObject healObject = Instantiate(healPrefab, spawnPosition, Quaternion.identity);
        NetworkServer.Spawn(healObject);
        currentHeals++;
    }

    [Server]
    public void HealPickedUp()
    {
        currentHeals--;
    }
}