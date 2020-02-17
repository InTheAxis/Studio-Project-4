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
    private string initialScene = "Mainmenu";

    private void Start()
    {
        GetComponent<SceneTransition>().Transition(initialScene, "FadeToBlack", "FadeFromBlack", 1.0f);
    }

    private void Update()
    {

    }

    private void OnApplicationQuit()
    {

    }




}
