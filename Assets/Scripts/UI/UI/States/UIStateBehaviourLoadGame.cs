using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class UIStateBehaviourLoadGame : UIStateBehaviourBase
{
    [SerializeField]
    private string inGameSceneName;

    public override void onEnter()
    {
        NetworkClient.instance.setRoomVisibility(false);

        UIStateActiveManager.currentActiveManager.setToPrevState();

        // PhotonNetwork.AutomaticallySyncScene, set in NetworkClient.cs, causes non-master clients to load scenes automatically
        // Therefore, do not load the level again if non-master client
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel(inGameSceneName);
    }
}
