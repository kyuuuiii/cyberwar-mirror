using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsButton : MonoBehaviour
{
    public GameObject optionsPanel;
    public GameObject mainPanel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchPanels()
    {
        optionsPanel.SetActive(!optionsPanel.activeSelf);
        mainPanel.SetActive(!mainPanel.activeSelf);
    }
}
