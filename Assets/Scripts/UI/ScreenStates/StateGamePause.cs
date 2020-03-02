using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class StateGamePause : State
{

    public override string Name { get { return "GamePause"; } }

    private void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            StateController.showNext("Gameplay");
    }

    public void Resume()
    {
        StateController.showNext("Gameplay");
    }

    public void showOptions()
    {
        StateController.showNext("GameOptions");
    }

    public void Disconnect()
    {
        Debug.Log("This should be changed to Disconnect.");
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public override void onShow()
    {
        Cursor.lockState = CursorLockMode.None;

        if (GameManager.playerObj != null)
        {
            GameManager.playerObj.GetComponent<CharTPController>().disableKeyInput = true;
            GameManager.playerObj.GetComponent<CharTPController>().disableMouseInput = true;
            GameManager.playerObj.GetComponent<CharTPController>().disableMovement = true;
        }
        base.onShow();
    }


}
