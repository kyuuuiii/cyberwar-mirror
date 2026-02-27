using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    public static ScoreSystem Instance;
    public float score = 0;
    public float scoreMultDecrease = 0;
    public float scoreMult = 1;
    public float scoreMultMin = 1;
    public float killedAmount = 0;
    public float lastKilledTime;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        } 
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        StartCoroutine(ComboBar());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GainScore(float scoreAmount)
    {
        score += scoreAmount * scoreMult;
    }

    public IEnumerator ComboBar()
    {
        while (true)
        {
            killedAmount = 0;
            yield return new WaitForSeconds(0.1f);
            scoreMult += killedAmount / (scoreMult + 1 - scoreMultMin) * 0.5f;
            if (Time.time - lastKilledTime >= 2.5f)
            {
                scoreMultDecrease += 0.01f;
            }
            else
            {
                scoreMultDecrease = 0;
            }

            scoreMult -= scoreMultDecrease * 0.1f;
            if (scoreMult < scoreMultMin)
            {
                scoreMult = scoreMultMin;
            }
            //Debug.Log(scoreMult);
        }
    }
}
