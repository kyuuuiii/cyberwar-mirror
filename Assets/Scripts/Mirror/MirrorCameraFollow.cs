using Mirror;
using UnityEngine;

public class MirrorCameraFollow : MonoBehaviour
{
    public Vector3 offset = new Vector3(0, 10, -10);

    private Transform target;
    private bool targetAssigned = false;

    void Start()
    {
        FindLocalPlayer();
    }

    void Update()
    {
        if (target == null || !targetAssigned)
        {
            FindLocalPlayer();
            return;
        }

        transform.position = target.position + offset;
    }

    void FindLocalPlayer()
    {
        MirrorPlayer[] players = FindObjectsOfType<MirrorPlayer>();

        foreach (MirrorPlayer player in players)
        {
            if (player.isLocalPlayer)
            {
                target = player.transform;
                targetAssigned = true;
                Debug.Log("Camera found LOCAL player: " + player.name);
                return;
            }
        }

        if (!targetAssigned)
        {
            Invoke(nameof(FindLocalPlayer), 1f);
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        targetAssigned = true;
        Debug.Log("Camera target manually set to: " + newTarget.name);
    }
}