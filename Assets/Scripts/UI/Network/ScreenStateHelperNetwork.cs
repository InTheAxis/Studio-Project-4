using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ScreenStateHelperNetwork : MonoBehaviourPun
{
    public static ScreenStateHelperNetwork instance = null;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Debug.LogError("ScreenStateHelperNetwork instantiated twice! This should not happen");
    }

    public void sendLoadingGameMsg()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("Attempt to send loading game msg as non-master client!");
            return;
        }

        Debug.Log("Sending loading game msg");
        photonView.RPC("receiveLoadingGameMsg", RpcTarget.Others);
    }

    [PunRPC]
    private void receiveLoadingGameMsg()
    {
        Debug.Log("Received loading game msg");
        if (StateController.getCurrentState().Name == "MatchLobby")
            StateController.showNext("GameLoading");
        else
            Debug.LogWarning("Received loading game msg while state is " + StateController.getCurrentState().Name);
    }
}
