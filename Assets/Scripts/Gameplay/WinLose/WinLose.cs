using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WinLose : MonoBehaviourPun
{
    public static WinLose instance = null;

    public delegate void WinLossCallback(bool isHunterWin);
    public WinLossCallback winLossCallback;

    private bool gameEnded = false;

    private float backToLobbyTimer = 0.0f;
    private bool hasSentLoadRequest = false;

    [SerializeField]
    private float delayToLobbyTime = 5.0f;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Debug.LogWarning("WinLose instance already instanced! This should not happen");
    }

    private void OnEnable()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        CharTPController.OnPlayerAdd += onNewPlayer;
        foreach (var p in CharTPController.PlayerControllerRefs)
            p.controller.GetComponent<CharHealth>().OnDead += onPlayerDied;
    }
    private void OnDisable()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        CharTPController.OnPlayerAdd -= onNewPlayer;
        foreach (var p in CharTPController.PlayerControllerRefs)
            p.controller.GetComponent<CharHealth>().OnDead -= onPlayerDied;
    }
    private void onNewPlayer(CharTPController.PlayerControllerData newPlayer)
    {
        newPlayer.controller.GetComponent<CharHealth>().OnDead += onPlayerDied;
    }
    private void onPlayerDied()
    {
        bool isHunterAlive = false;
        bool isSurvivorAlive = false;

        foreach (var p in CharTPController.PlayerControllerRefs)
            if (!p.controller.GetComponent<CharHealth>().dead)
            {
                if ((int)NetworkClient.getPlayerProperty(p.controller.photonView.Controller, "charModel") == 0) // Hunter
                    isHunterAlive = true;
                else
                    isSurvivorAlive = true;

                if (isHunterAlive && isSurvivorAlive)
                    break;
            }

        if (!isHunterAlive)
            gameEnd(false);
        else if (!isSurvivorAlive)
            gameEnd(true);
    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (!gameEnded)
        {
            if (Input.GetKeyDown(KeyCode.K))
                gameEnd(true);
            else if (Input.GetKeyDown(KeyCode.J))
                gameEnd(false);
        }
        else
        {
            backToLobbyTimer += Time.deltaTime;
            if (backToLobbyTimer >= delayToLobbyTime && !hasSentLoadRequest)
            {
                hasSentLoadRequest = true;
                PhotonNetwork.LoadLevel("NewMainmenu");
            }
        }
    }

    // Should only ever be called by the master client
    private void gameEnd(bool isHunterWin)
    {
        //gameEnded = true;
        //ScreenStateController.ReturningFromInGame = true;
        Debug.Log("Sending game end with " + (isHunterWin ? "Hunter" : "Survivors") + " winning");
        photonView.RPC("receiveGameEnd", RpcTarget.Others, isHunterWin);

        //winLossCallback(isHunterWin);
        receiveGameEnd(isHunterWin);
    }

    // Should only ever be called on remote clients
    [PunRPC]
    private void receiveGameEnd(bool isHunterWin)
    {
        gameEnded = true;
        ScreenStateController.ReturningFromInGame = true;

        winLossCallback(isHunterWin);

        // Note: Gameover is also set in GameManager
        StateGameover.isGameover = true;
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel("NewMainMenu");

        Debug.Log("Received game end with " + (isHunterWin ? "Hunter" : "Survivors") + " winning");
    }
}
