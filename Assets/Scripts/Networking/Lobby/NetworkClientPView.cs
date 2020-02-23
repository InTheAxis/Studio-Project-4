using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkClientPView : MonoBehaviourPun
{
    public static NetworkClientPView instance = null;

    private ReadyList readyList;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(this.gameObject);
            return;
        }

        readyList = GetComponent<ReadyList>();
    }
    public static void destroySingleton()
    {
        if (instance)
        {
            Destroy(instance.gameObject);
            instance = null;
        }
    }

    public List<string> getReadyPlayers()
    {
        return readyList.PlayerList;
    }
    public void toggleReady(string playerName)
    {
        readyList.toggleReady(playerName);
    }
    public void disconnect(string playerName)
    {

    }
    public bool isReady(string playerName)
    {
        return readyList.isReady(playerName);
    }
    public bool areAllReady(int totalPlayers)
    {
        return readyList.areAllReady(totalPlayers);
    }
}
