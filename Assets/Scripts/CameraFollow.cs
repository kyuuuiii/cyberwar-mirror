using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    void Start()
    {
        // Автоматически ищем игрока при старте
        FindPlayer();
    }

    void Update()
    {
        // Если target не назначен, пытаемся найти игрока
        if (target == null)
        {
            FindPlayer();
            return;
        }

        // Следим за игроком
        transform.position = target.position + offset;
    }

    void FindPlayer()
    {
        // Ищем объект с тегом "Player"
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
            Debug.Log("Camera found player: " + player.name);
        }
        else
        {
            // Если не нашли с первого раза, пробуем еще через секунду
            Invoke(nameof(FindPlayer), 1f);
        }
    }

    // Метод для ручного назначения цели (можно вызывать из PlayerSpawner)
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        Debug.Log("Camera target manually set to: " + newTarget.name);
    }
}