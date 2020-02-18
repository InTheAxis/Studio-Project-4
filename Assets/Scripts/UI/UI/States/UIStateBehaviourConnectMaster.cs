using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStateBehaviourConnectMaster : UIStateBehaviourBase
{
    public override void onEnter()
    {
        NetworkClient.instance.masterServerConnectedCallback = connectedToMaster;
        NetworkClient.instance.connectToMasterServer();
    }
    public override void onExit()
    {
        NetworkClient.instance.masterServerConnectedCallback = null;
    }

    private void connectedToMaster()
    {
        UIStateActiveManager.currentActiveManager.setNextState("StateInputName");
    }
}
