﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BttnBackToMainMenu() {
        SceneManager.LoadScene("MainMenu");
    }
    public void BttnBackToPlaying() {
        SceneManager.LoadScene("SceneSwitcher");
    }
}
