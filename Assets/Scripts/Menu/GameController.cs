using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoSingletonHungry<GameController>
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        }
    }

    void Awake()
    {
        Screen.SetResolution(2560, 1440, true);
    }
    
    public void Restart()
    {
        BlackScreenTransition.Instance.Transition(0.5f,EnterScene);
    }

    private void EnterScene()
    {
        SceneManager.LoadScene("GameScene");
    }
}
