using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkClientPView : MonoBehaviour
{
    public static NetworkClientPView instance = null;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(this.gameObject);
            return;
        }
    }

    [PunRPC]
    private void goInGameRequestReceived()
    {
        Debug.Log("Received go in game");
        UIStateActiveManager.currentActiveManager.setNextState("StateLoadGame");
    }
}
