using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ScreenStateHelperNetwork : MonoBehaviourPun
{
    public static ScreenStateHelperNetwork instance = null;

    #region StartGame

    public void startGame(Player masterPlayer)
    {
        Debug.Log("startGame");
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("Called startGame as non-master client!");
            return;
        }

        if (masterPlayer.IsLocal)
            startGameRpc();
        else
        {
            PhotonNetwork.SetMasterClient(masterPlayer);
            photonView.RPC("startGameRpc", masterPlayer);
        }
    }

    [PunRPC]
    private void startGameRpc()
    {
        StartCoroutine(startGameRpcCour());
    }
    private IEnumerator startGameRpcCour()
    {
        while (!PhotonNetwork.IsMasterClient)
            yield return null;

        NetworkClient.instance.setRoomVisibility(false);

        ScreenStateHelperNetwork.instance.sendLoadingGameMsg();
        StateController.showNext("GameLoading");
        FindObjectOfType<StateGameLoading>().loadPhoton("Gameplay");
    }

    #endregion

    #region Lobby Character

    public delegate void ModelChangeCallback(string playerName, int modelIndex);
    public ModelChangeCallback modelChangeCallback;

    public void sendModelChange(int modelIndex)
    {
        photonView.RPC("receiveModelChangeRpc", RpcTarget.Others, modelIndex);
    }
    [PunRPC]
    private void receiveModelChangeRpc(int modelIndex, PhotonMessageInfo messageInfo)
    {
        modelChangeCallback?.Invoke(messageInfo.Sender.NickName, modelIndex);
    }

    #endregion

    #region Loading

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

    #endregion
}
