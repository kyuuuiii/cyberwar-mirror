using UnityEngine;

public class HealSpawner : MonoBehaviour
{
    public static HealSpawner Instance;

    public float minSpawnX = -85f;
    public float maxSpawnX = 60f;
    public float minSpawnZ = -22f;
    public float maxSpawnZ = 120f;
    public int maxHeals = 1;
    public GameObject healPrefab;
    public float spawnInterval = 5f;

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

    void Start()
    {
        if (spawnInterval > 0)
        {
            InvokeRepeating(nameof(SpawnHeal), spawnInterval, spawnInterval);
        }
    }

    public void SpawnHeal()
    {
        if (currentHeals >= maxHeals) return;

        Vector3 spawnPosition = new Vector3(
            Random.Range(minSpawnX, maxSpawnX),
            -0.7f,
            Random.Range(minSpawnZ, maxSpawnZ)
        );

        Instantiate(healPrefab, spawnPosition, Quaternion.identity);
        currentHeals++;
    }

    public void HealPickedUp()
    {
        currentHeals--;
    }
}