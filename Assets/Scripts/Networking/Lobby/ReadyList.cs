using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ReadyList : MonoBehaviourPun
{
    private List<string> playerList = new List<string>();
    public List<string> PlayerList { get => playerList; }

    private void Start()
    {
        photonView.RPC("helloRequest", RpcTarget.MasterClient);
    }

    public void toggleReady(string playerName)
    {
        receiveToggle(playerName);
        photonView.RPC("receiveToggle", RpcTarget.Others, playerName);
    }
    public void disconnect(string playerName)
    {
        if (isReady(playerName))
            toggleReady(playerName);
    }

    public bool isReady(string playerName)
    {
        return playerList.Contains(playerName);
    }
    public bool areAllReady(int totalInRoom)
    {
        return playerList.Count == totalInRoom;
    }

    [PunRPC]
    private void helloRequest(PhotonMessageInfo messageInfo)
    {
        photonView.RPC("receiveReadyList", messageInfo.Sender, playerList.Count, playerList.ToArray());
    }
    [PunRPC]
    private void receiveReadyList(int count, string[] readyList)
    {
        for (int i = 0; i < count; ++i)
        {
            string name = readyList[i];
            if (!playerList.Contains(name))
                playerList.Add(name);
            else
                // This case happens when this player was ready on the master client, 
                // but we receive a toggle request by that player before we got a reply
                // from the master client. Therefore, this player shouldn't be ready.
                playerList.Remove(name);
        }
    }
    [PunRPC]
    private void receiveToggle(string playerName)
    {
        Debug.Log("Received ready toggle for " + playerName);

        if (playerList.Contains(playerName))
            playerList.Remove(playerName);
        else
            playerList.Add(playerName);
    }
}
