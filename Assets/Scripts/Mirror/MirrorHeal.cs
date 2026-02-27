using Mirror;
using UnityEngine;

public class MirrorHeal : NetworkBehaviour
{
    public float rotationSpeedY = 0.5f;

    void Update()
    {
        transform.Rotate(0, rotationSpeedY, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6 && other.gameObject.tag == "Player")
        {
            CmdPickupHeal(other.gameObject);
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdPickupHeal(GameObject playerObject)
    {
     
        MirrorPlayer playerComponent = playerObject.GetComponent<MirrorPlayer>();
        if (playerComponent != null)
        {
            playerComponent.TakeHeal(20);
        }

        MirrorHealSpawner.Instance.HealPickedUp();

        NetworkServer.Destroy(gameObject);
    }
}