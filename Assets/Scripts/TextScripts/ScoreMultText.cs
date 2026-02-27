using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class ScoreMultText : MonoBehaviour
{
    public TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = $"X{string.Format("{0:F1}", ScoreSystem.Instance.scoreMult)}";
    }
}
