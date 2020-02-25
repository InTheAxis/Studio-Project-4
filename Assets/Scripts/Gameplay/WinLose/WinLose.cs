using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WinLose : SingletonPun<WinLose>
{
    public delegate void WinLossCallback(bool isHunterWin);
    public WinLossCallback winLossCallback;

    private bool gameEnded = false;

    private float backToLobbyTimer = 0.0f;
    private bool hasSentLoadRequest = false;

    [SerializeField]
    private float delayToLobbyTime = 5.0f;

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
                ScreenStateController.ReturningFromInGame = true;
                PhotonNetwork.LoadLevel("Mainmenu");
            }
        }
    }

    // Should only ever be called by the master client
    private void gameEnd(bool isHunterWin)
    {
        gameEnded = true;
        Debug.Log("Sending game end with " + (isHunterWin ? "Hunter" : "Survivors") + " winning");
        photonView.RPC("receiveGameEnd", RpcTarget.Others, isHunterWin);

        winLossCallback(isHunterWin);
    }

    // Should only ever be called on remote clients
    [PunRPC]
    private void receiveGameEnd(bool isHunterWin)
    {
        gameEnded = true;
        Debug.Log("Received game end with " + (isHunterWin ? "Hunter" : "Survivors") + " winning");

        winLossCallback(isHunterWin);
    }
}
