using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HpText : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Player player;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = $"hp: {player.hp:F0}";

    }
}
