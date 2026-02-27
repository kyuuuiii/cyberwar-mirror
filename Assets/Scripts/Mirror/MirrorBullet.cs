using Mirror;
using UnityEngine;

public class MirrorBullet : NetworkBehaviour
{
    public float moveSpeed = 10;

    [SyncVar]
    public uint ownerNetId;

    private NetworkIdentity _ownerIdentity;

    void Start()
    {
        if (isServer)
        {

            if (ownerNetId != 0)
            {
                NetworkServer.spawned.TryGetValue(ownerNetId, out _ownerIdentity);
            }

            Invoke(nameof(ServerDestroyBullet), 3f);
        }
    }

    void Update()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {

        if (_ownerIdentity != null && other.gameObject == _ownerIdentity.gameObject)
            return;

        if (other.CompareTag("Enemy"))
        {
            MirrorEnemy enemy = other.GetComponent<MirrorEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(10);
                ServerDestroyBullet();
            }
        }
        else if (other.CompareTag("Player"))
        {
            MirrorPlayer player = other.GetComponent<MirrorPlayer>();
            if (player != null)
            {
                player.TakeDamage(20);
                ServerDestroyBullet();
            }
        }
        else if (other.gameObject.layer == 6)
        {
            ServerDestroyBullet();
        }
    }

    [Server]
    private void ServerDestroyBullet()
    {
        NetworkServer.Destroy(gameObject);
    }
}
