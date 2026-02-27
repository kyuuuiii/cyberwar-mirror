using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public int sceneNum = 3;

    void Start()
    {
        
    }
    void Update()
    {
        
    }
    public void SceneSwitch(int sceneNum)
    {
        SceneManager.LoadScene(sceneNum);
    }
}
