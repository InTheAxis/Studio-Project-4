using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class UIStateBehaviourLoadGame : UIStateBehaviourBase
{
    public override void onEnter()
    {
        UIStateActiveManager.currentActiveManager.setToPrevState();
        PhotonNetwork.LoadLevel("Gameplay");
    }
}
