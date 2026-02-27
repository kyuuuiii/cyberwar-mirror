using System.Collections;
using TMPro;
using UnityEngine;

public class Ultradead : MonoBehaviour
{
    [Header("Text Settings")]
    public TMP_Text textComponent;
    [SerializeField] private float lineDelay = 0.5f;
    [SerializeField] private float charDelay = 0.05f;
    [SerializeField] private bool useTypewriterEffect = true;
    public AudioClip deathSound;

    string[] cyberneticDistressLines = new string[] {
        "WARNING: EXTREME DAMAGE SUSTAINED",
        "RUNNING DIAGNOSTIC",
        "ERROR: ARM MODULE #1 NOT RESPONDING",
        "ERROR: ARM MODULE #2 NOT RESPONDING",
        "WARNING: COMBAT SYSTEMS INOPERABLE",
        "ATTEMPTING RECONSTRUCTION",
        "ERROR: SELF-REPAIR NEXUS NOT RESPONDING",
        "INSUFFICENT BLOOD.",
        "INSUFFICENT BLOOD.",
        "INITIATING ESCAPE PROTOCOL",
        "ATTEMPTING CONNECTION WITH LIMBIC MODULES",
        "ERROR: LEG MODULE #1 NOT RESPONDING",
        "ERROR: LEG MODULE #2 NOT RESPONDING",
        "WARNING: UNABLE TO SUSTAIN MOTOR FUNCTIONS",
        "ERROR: VISUAL CORTEX MALFUNCTION",
        "ERROR: LIMBIC FUNCTION NOT RESPONDING",
        "INSUFFICENT BLOOD.",
        "INSUFFICENT BLOOD.",
        "WARNING: UNABLE TO SUSTAIN INTERNAL ORGANS",
        "! PULSE FAILURE !",
        "! PULSE FAILURE !",
        "! PULSE FAILURE !",
        "-!- SHUTDOWN IMMITENT -!-",
        "ERROR: NO VOCAL INTERFACE DETECTED. UNABLE TO COMPLETE TASK",
        "! PULSE FAILURE !",
        "! PULSE FAILURE !",
        "INSUFFICENT BLOOD.",
        "INSUFFICENT BLOOD.",
        "WARNING: UNABLE TO SUSTAIN BASIC FUNCTIONS",
        "-!- SHUTDOWN IMMITENT -!-",
        "-!- SHUTDOWN IMMITENT -!-",
        "I DON'T WANT TO DIE.",
        "I DON'T WANT TO DIE.",
        "I DON'T WANT TO DIE."
    };

    private string fullText = "";
    private int currentLine = 0;

    private void Start()
    {
        if (textComponent == null)
            textComponent = GetComponent<TMP_Text>();
        
        if (HealthBarManager.Instance != null)
        {
            HealthBarManager.Instance.SetAllHealthBarsVisible(false);
        }
    }

    private void OnEnable()
    {
        SoundManager.Instance.PlayDeathSound(deathSound);
        StartCoroutine(DisplayText());
    }

    private IEnumerator DisplayText()
    {
        textComponent.text = "";

        while (currentLine < cyberneticDistressLines.Length)
        {
            string lineToAdd = cyberneticDistressLines[currentLine] + "\n";

            if (useTypewriterEffect)
            {
                for (int i = 0; i < lineToAdd.Length; i++)
                {
                    fullText += lineToAdd[i];
                    textComponent.text = fullText;
                    
                    yield return new WaitForSecondsRealtime(charDelay); // Используем Realtime
                }
            }
            else
            {
                fullText += lineToAdd;
                textComponent.text = fullText;
            }

            currentLine++;
            if(currentLine == 5)
            {
                lineDelay = 0.000000000001f;
                charDelay = 0;
            }
            yield return new WaitForSecondsRealtime(lineDelay); // Используем Realtime
        }
    }

    public void ResetAndPlay()
    {
        StopAllCoroutines();
        fullText = "";
        currentLine = 0;
        textComponent.text = "";
        StartCoroutine(DisplayText());
    }
    void OnDisable()
    {
        // Показываем все HP бары при закрытии меню смерти
        if (HealthBarManager.Instance != null)
        {
            HealthBarManager.Instance.SetAllHealthBarsVisible(true);
        }
    }
}