using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class UIStateBehaviourNameInput : UIStateBehaviourBase
{
    [SerializeField]
    InputField nameInput;
    [SerializeField]
    Button confirmButton;

    private void Awake()
    {
        confirmButton.onClick.AddListener(confirmButtonClicked);
    }

    private void confirmButtonClicked()
    {
#if DEBUG == false
        if (nameInput.text.Length == 0)
        {
            UIStateBehaviourMessage.messageString = "Please input a username";
            UIStateActiveManager.currentActiveManager.setNextState("StateMessage");
        }
        else
#endif
        {
            PlayerSettings.playerName = nameInput.text;
            PhotonNetwork.NickName = nameInput.text;
            UIStateActiveManager.currentActiveManager.setNextState("StateHostJoin");
        }
    }
}
