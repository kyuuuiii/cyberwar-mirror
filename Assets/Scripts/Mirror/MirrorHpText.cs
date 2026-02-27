using TMPro;
using UnityEngine;

public class MirrorHpText : MonoBehaviour
{
    public TextMeshProUGUI hpText;
    private MirrorPlayer localPlayer;
    private float nextUpdateTime = 0f;
    private float updateInterval = 0.1f;

    void Start()
    {
        hpText = GetComponent<TextMeshProUGUI>();

        Invoke(nameof(FindLocalPlayerOnce), 0.5f);
    }

    void FindLocalPlayerOnce()
    {
        MirrorPlayer[] players = FindObjectsOfType<MirrorPlayer>();
        foreach (var player in players)
        {
            if (player.isLocalPlayer)
            {
                localPlayer = player;
                break;
            }
        }
    }

    void Update()
    {
        if (Time.time >= nextUpdateTime)
        {
            nextUpdateTime = Time.time + updateInterval;

            if (localPlayer != null)
            {
                hpText.text = $"HP: {localPlayer.GetHp():F0}";
            }
        }
    }
}