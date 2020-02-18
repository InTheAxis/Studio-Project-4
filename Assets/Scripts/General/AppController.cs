using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

using UnityEngine.SceneManagement;
using System;
using System.Security.Cryptography;
using System.Text;

public class AppController : DoNotDestroySingleton<AppController>
{

    [SerializeField]
    private bool animateTransition = false;
    [SerializeField]
    private string initialScene = "Mainmenu";

    private void Start()
    {
        if (animateTransition)
            GetComponent<SceneTransition>().Transition(initialScene, "FadeToBlack", "FadeFromBlack", 1.0f);
        else
            SceneManager.LoadScene(initialScene, LoadSceneMode.Single);
    }

    private void Update()
    {

    }

    private void OnApplicationQuit()
    {

    }




}
