using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
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

    public GameObject healthBarPrefab;
    private GameObject healthBarInstance;
    private Slider healthSlider;
    public Vector3 healthBarOffset = new Vector3(0, 2f, 0);

    void Start()
    {
        stopDistance = agent.stoppingDistance;
        target = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(Shooting());
        StartCoroutine(CheckState());
        InitializeHealthBar(); // Инициализация шкалы здоровья
    }

    void Update()
    {
        transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
        timer += Time.deltaTime;
        UpdateHealthBarPosition(); // Обновление позиции шкалы здоровья
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

    public void TakeDamage(float damage)
    {
        hp -= damage;

        if (healthSlider != null)
        {
            healthSlider.value = hp; // Обновление шкалы здоровья
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
            Destroy(healthBarInstance); // Уничтожение шкалы здоровья
        }

        Destroy(gameObject);
    }

    private void Shoot()
    {
        GameObject newBullet = Instantiate(bulletPrefab);
        newBullet.transform.position = firespot.transform.position;
        newBullet.transform.rotation = firespot.transform.rotation;
        SoundManager.Instance.PlaySound(shootSound);
    }

    public System.Collections.IEnumerator Shooting()
    {
        while (true)
        {
            yield return new WaitForSeconds(5 / attackSpeed);
            if (!isHiding && Vector3.Distance(transform.position, target.position) <= attackRange)
            {
                Shoot();
            }
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