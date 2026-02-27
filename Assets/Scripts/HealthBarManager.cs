using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarManager : MonoBehaviour
{
    public static HealthBarManager Instance;

    private List<GameObject> allHealthBars = new List<GameObject>();
    private Canvas healthBarCanvas;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            CreateHealthBarCanvas();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void CreateHealthBarCanvas()
    {
        GameObject canvasGO = new GameObject("HealthBarsCanvas");
        healthBarCanvas = canvasGO.AddComponent<Canvas>();
        healthBarCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        canvasGO.AddComponent<GraphicRaycaster>();
        DontDestroyOnLoad(canvasGO);
    }

    public GameObject CreateHealthBar(GameObject healthBarPrefab, Transform target, float maxHp, float currentHp)
    {
        if (healthBarPrefab == null || target == null)
        {
            Debug.LogError("HealthBarPrefab or target is null!");
            return null;
        }

        GameObject healthBarInstance = Instantiate(healthBarPrefab, healthBarCanvas.transform);
        allHealthBars.Add(healthBarInstance);

        HealthBarFollow followComponent = healthBarInstance.GetComponent<HealthBarFollow>();
        if (followComponent == null)
        {
            followComponent = healthBarInstance.AddComponent<HealthBarFollow>();
        }

        followComponent.Initialize(target, maxHp, currentHp);

        return healthBarInstance;
    }

    public void RemoveHealthBar(GameObject healthBar)
    {
        if (healthBar != null && allHealthBars.Contains(healthBar))
        {
            allHealthBars.Remove(healthBar);
            Destroy(healthBar);
        }
    }

    public void SetAllHealthBarsVisible(bool visible)
    {
        foreach (GameObject healthBar in allHealthBars)
        {
            if (healthBar != null)
            {
                healthBar.SetActive(visible);
            }
        }
    }

    public void ClearAllHealthBars()
    {
        foreach (GameObject healthBar in allHealthBars)
        {
            if (healthBar != null)
            {
                Destroy(healthBar);
            }
        }
        allHealthBars.Clear();
    }
}