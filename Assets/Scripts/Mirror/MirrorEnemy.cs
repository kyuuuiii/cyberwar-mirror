using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class MirrorEnemy : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnHpChanged))]
    public float hp = 100f;

    public float maxHp = 100;
    public float attackSpeed = 5;
    public float checkInterval = 0.1f;
    public float stopDistance = 1;
    public GameObject bulletPrefab;
    public Transform firespot;
    public Transform dot;
    public Transform target;
    public NavMeshAgent agent;
    public float timer = 0;
    public float minusDistance = 0.2f;
    private bool isHiding = false;
    public float attackRange = 10f;
    public float scoreAmount;

    public AudioClip shootSound;

    void Start()
    {
        stopDistance = agent.stoppingDistance;
        target = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(Shooting());
        StartCoroutine(CheckState());
    }

    private void OnHpChanged(float oldHp, float newHp)
    {
        if (newHp <= 0 && isServer)
        {
            HandleDeath();
        }
    }

    void Update()
    {
        if (!isServer) return;

        transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
        timer += Time.deltaTime;
    }

    System.Collections.IEnumerator CheckState()
    {
        while (true)
        {
            if (!agent.hasPath || timer > 5)
            {
                float distance = Vector3.Distance(transform.position, target.position);
                if (distance > stopDistance - minusDistance && distance < stopDistance + minusDistance)
                {
                    agent.isStopped = true;
                }
                else if (distance > stopDistance + minusDistance)
                {
                    agent.isStopped = false;
                    agent.SetDestination(target.position);
                }
                else if (distance < stopDistance - minusDistance)
                {
                    agent.isStopped = false;
                    agent.SetDestination(dot.position);
                }
                timer = 0;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    [Server]
    public void TakeDamage(float damage)
    {
        hp -= damage;

        if (hp <= 0)
        {
            HandleDeath();
        }
    }

    [Server]
    private void HandleDeath()
    {
        if (MirrorEnemySpawner.Instance != null)
        {
            MirrorEnemySpawner.Instance.EnemyDestroyed();
        }

        NetworkServer.Destroy(gameObject);
    }

    [Server]
    private void Shoot()
    {
        GameObject newBullet = Instantiate(bulletPrefab);
        newBullet.transform.position = firespot.transform.position;
        newBullet.transform.rotation = firespot.transform.rotation;

        MirrorBullet bullet = newBullet.GetComponent<MirrorBullet>();
        if (bullet != null)
        {
            NetworkServer.Spawn(newBullet);
            RpcPlayShootSound();
        }
    }

    [ClientRpc]
    private void RpcPlayShootSound()
    {
        SoundManager.Instance.PlaySound(shootSound);
    }

    public System.Collections.IEnumerator Shooting()
    {
        while (true)
        {
            yield return new WaitForSeconds(5 / attackSpeed);
            if (!isHiding && Vector3.Distance(transform.position, target.position) <= attackRange)
            {
                if (isServer)
                {
                    Shoot();
                }
            }
        }
    }
}
