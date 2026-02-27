using UnityEngine;
using UnityEngine.UI;

public class HealthBarFollow : MonoBehaviour
{
    private Transform target;
    private Slider healthSlider;
    private Vector3 offset = new Vector3(0, 2f, 0);

    public void Initialize(Transform targetTransform, float maxHp, float currentHp)
    {
        target = targetTransform;
        healthSlider = GetComponentInChildren<Slider>();

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHp;
            healthSlider.value = currentHp;
        }
        else
        {
            Debug.LogError("HealthSlider not found in HealthBarFollow!");
        }
    }

    void Update()
    {
        if (target != null && Camera.main != null)
        {
            Vector3 worldPosition = target.position + offset;
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

            if (screenPosition.z > 0)
            {
                transform.position = screenPosition;
                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
        else if (target == null)
        {
            // ≈сли цель уничтожена, уничтожаем и HP бар
            Destroy(gameObject);
        }
    }

    public void UpdateHealth(float currentHp)
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHp;
        }
    }
}