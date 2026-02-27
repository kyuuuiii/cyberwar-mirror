using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float moveSpeed = 10;

    void Start()
    {
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(10);
                DestroyBullet();
            }
        }
        else if (other.gameObject.CompareTag("Player"))
        {
            Player player = other.gameObject.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(20);
                DestroyBullet();
            }
        }
        else if (other.gameObject.CompareTag("CloseEnemy"))
        {
            CloseEnemy closeEnemy = other.gameObject.GetComponent<CloseEnemy>();
            if (closeEnemy != null)
            {
                closeEnemy.TakeDamage(10);
                DestroyBullet();
            }
        }
        else if (other.gameObject.layer == 6)
        {
            DestroyBullet();
        }
    }

    private void DestroyBullet()
    {
        Destroy(gameObject);
    }
}