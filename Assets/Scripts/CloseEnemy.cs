using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class CloseEnemy : MonoBehaviour
{
    public float hp = 100f;
    public float maxHp = 100;
    public float attackSpeed = 5;
    public float stopDistance = 0.4f;
    public Transform target;
    public NavMeshAgent agent;
    public float timer = 0;
    public float scoreAmount;
    private Animator animator;
    private Player playerScript;

    public GameObject healthBarPrefab;
    private GameObject healthBarInstance;
    private Slider healthSlider;
    public Vector3 healthBarOffset = new Vector3(0, 2f, 0);

    public AudioClip slashSound;

    void Start()
    {
        stopDistance = agent.stoppingDistance;
        target = GameObject.FindGameObjectWithTag("Player").transform;
        playerScript = target.GetComponent<Player>();
        StartCoroutine(CheckState());
        InitializeHealthBar();
    }

    void InitializeHealthBar()
    {
        if (healthBarPrefab != null)
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("Canvas not found in scene!");
                return;
            }

            healthBarInstance = Instantiate(healthBarPrefab, canvas.transform);
            healthSlider = healthBarInstance.GetComponentInChildren<Slider>();
            healthSlider.maxValue = maxHp;
            healthSlider.value = hp;

            UpdateHealthBarPosition();
        }
    }

    void Update()
    {
        transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
        transform.eulerAngles += new Vector3(0, 180, 0);
        timer += Time.deltaTime;

        UpdateHealthBarPosition();
    }

    void UpdateHealthBarPosition()
    {
        if (healthBarInstance != null && Camera.main != null)
        {
            Vector3 worldPosition = transform.position + healthBarOffset;
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

            if (screenPosition.z > 0)
            {
                healthBarInstance.transform.position = screenPosition;
                healthBarInstance.SetActive(true);
            }
            else
            {
                healthBarInstance.SetActive(false);
            }
        }
    }

    System.Collections.IEnumerator CheckState()
    {
        while (true)
        {
            if (!agent.hasPath || timer > 1)
            {
                float distance = Vector3.Distance(transform.position, target.position);
                agent.SetDestination(target.position);
                if (distance <= stopDistance)
                {
                    Attack();
                    yield return new WaitForSeconds(5 / attackSpeed);
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;

        if (healthSlider != null)
        {
            healthSlider.value = hp;
        }

        if (hp <= 0)
        {
            HandleDeath();
        }
    }

    private void HandleDeath()
    {
        EnemySpawner.Instance.EnemyDestroyed();
        ScoreSystem.Instance.killedAmount += 1;
        ScoreSystem.Instance.lastKilledTime = Time.time;
        ScoreSystem.Instance.GainScore(scoreAmount);

        if (healthBarInstance != null)
        {
            Destroy(healthBarInstance);
        }

        Destroy(gameObject);
    }

    private void Attack()
    {
        if (playerScript != null)
        {
            playerScript.TakeDamage(20);
            SoundManager.Instance.PlaySound(slashSound);
        }
    }

    void OnDestroy()
    {
        if (healthBarInstance != null)
        {
            Destroy(healthBarInstance);
        }
    }

    void OnDisable()
    {
        if (healthBarInstance != null)
        {
            healthBarInstance.SetActive(false);
        }
    }

    void OnEnable()
    {
        if (healthBarInstance != null)
        {
            healthBarInstance.SetActive(true);
        }
    }
}