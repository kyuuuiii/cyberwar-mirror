using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerSpawner : NetworkBehaviour
{
    public static PlayerSpawner Instance;
    public GameObject playerSinglePrefab;
    public GameObject playerMultiPrefab;

    public float minSpawnX = -5f;
    public float maxSpawnX = 5f;
    public float minSpawnZ = -5f;
    public float maxSpawnZ = 5f;

    private bool hasSpawned = false;

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
        Invoke(nameof(SpawnPlayer), 0.1f);
    }

    void SpawnPlayer()
    {
        if (hasSpawned) return;

        if (IsNetworkedGame())
        {
            if (IsServer)
            {
                SpawnPlayerForAllClients();
                hasSpawned = true;
            }
        }
        else
        {
            SpawnSinglePlayer();
            hasSpawned = true;
        }
    }

    void SpawnSinglePlayer()
    {
        if (playerSinglePrefab == null)
        {
            Debug.LogError("PlayerSinglePrefab not assigned!");
            return;
        }

        Vector3 spawnPosition = GetSpawnPosition();
        GameObject player = Instantiate(playerSinglePrefab, spawnPosition, Quaternion.identity);
        Debug.Log("SINGLEPLAYER: Player spawned at " + spawnPosition);

        TryAssignCamera(player.transform);
    }

    void SpawnPlayerForAllClients()
    {
        if (playerMultiPrefab == null)
        {
            Debug.LogError("PlayerMultiPrefab not assigned!");
            return;
        }

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            SpawnPlayerForClient(client.ClientId);
        }

        Debug.Log($"MULTIPLAYER: Spawned players for {NetworkManager.Singleton.ConnectedClients.Count} clients");
    }

    public void SpawnPlayerForClient(ulong clientId)
    {
        Vector3 spawnPosition = GetSpawnPosition();
        GameObject playerObj = Instantiate(playerMultiPrefab, spawnPosition, Quaternion.identity);

        NetworkObject networkObject = playerObj.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            networkObject.SpawnAsPlayerObject(clientId);
            Debug.Log($"Player spawned for client {clientId} at {spawnPosition}");

            if (clientId == NetworkManager.Singleton.LocalClientId)
            {
                TryAssignCamera(playerObj.transform);
            }
        }
    }

    Vector3 GetSpawnPosition()
    {
        return new Vector3(
            Random.Range(minSpawnX, maxSpawnX),
            0,
            Random.Range(minSpawnZ, maxSpawnZ)
        );
    }

    public void OnClientConnected(ulong clientId)
    {
        if (IsServer && hasSpawned)
        {
            Debug.Log($"New client connected: {clientId}, spawning player...");
            SpawnPlayerForClient(clientId);
        }
    }

    void TryAssignCamera(Transform playerTransform)
    {
        CameraFollow cameraFollow = FindObjectOfType<CameraFollow>();
        if (cameraFollow != null)
        {
            cameraFollow.SetTarget(playerTransform);
        }
    }

    private bool IsNetworkedGame()
    {
        return NetworkManager.Singleton != null &&
              (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsServer);
    }
}