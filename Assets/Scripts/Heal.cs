using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Heal : NetworkBehaviour
{
    public float rotationSpeedY = 0.5f;

    void Update()
    {
        transform.Rotate(0, rotationSpeedY, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Попало что-то");
        if (other.gameObject.layer == 6 && other.gameObject.tag == "Player")
        {
            if (IsNetworkedGame())
            {
                ProcessHealPickupNetwork(other.gameObject);
            }
            else
            {
                ProcessHealPickup(other.gameObject);
            }
        }
    }

    private void ProcessHealPickupNetwork(GameObject player)
    {
        NetworkObject playerNetworkObject = player.GetComponent<NetworkObject>();
        if (playerNetworkObject != null && playerNetworkObject.IsOwner)
        {
            Player playerComponent = player.GetComponent<Player>();
            if (playerComponent != null)
            {
                playerComponent.TakeHeal(20);
            }

            if (IsServer)
            {
                DestroyHeal();
            }
            else
            {
                RequestDestroyServerRpc();
            }
        }
    }

    private void ProcessHealPickup(GameObject player)
    {
        if (HealSpawner.Instance != null)
            HealSpawner.Instance.SpawnHeal();

        Player playerComponent = player.GetComponent<Player>();
        if (playerComponent != null)
        {
            playerComponent.TakeHeal(20);
        }
        Destroy(gameObject.transform.parent.gameObject);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestDestroyServerRpc()
    {
        DestroyHeal();
    }

    private void DestroyHeal()
    {
        if (HealSpawner.Instance != null)
            HealSpawner.Instance.HealPickedUp();

        if (IsNetworkedGame())
        {
            NetworkObject networkObject = GetComponent<NetworkObject>();
            if (networkObject != null)
            {
                networkObject.Despawn(true);
            }
        }
        else
        {
            Destroy(gameObject.transform.parent != null ? gameObject.transform.parent.gameObject : gameObject);
        }
    }

    private bool IsNetworkedGame()
    {
        return NetworkManager.Singleton != null &&
              (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsServer);
    }
}