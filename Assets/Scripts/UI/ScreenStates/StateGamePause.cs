using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEngine.UI;
using Photon.Pun;
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
        Debug.Log("Disconnect.");

        if (PhotonNetwork.IsMasterClient)
            GameManager.instance.sendForceGameEnd(false);
        StateMainmenu.isReturningFromGame = true;
        StateGameover.isGameover = false;
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("NewMainMenu");
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
