using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class startButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void GameStart()
    {
        SceneManager.LoadScene(1);
    }

    public void GameExit()
    {
        Application.Quit();
    }
}
