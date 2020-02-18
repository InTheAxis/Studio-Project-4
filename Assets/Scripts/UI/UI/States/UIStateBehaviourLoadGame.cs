using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class UIStateBehaviourLoadGame : UIStateBehaviourBase
{
    public override void onEnter()
    {
        UIStateActiveManager.currentActiveManager.setToPrevState();

        // PhotonNetwork.AutomaticallySyncScene, set in NetworkClient.cs, causes non-master clients to load scenes automatically
        // Therefore, do not load the level again if non-master client
        if (PhotonNetwork.IsMasterClient)
            //PhotonNetwork.LoadLevel("Character");
            PhotonNetwork.LoadLevel("Interactables");
    }
}
