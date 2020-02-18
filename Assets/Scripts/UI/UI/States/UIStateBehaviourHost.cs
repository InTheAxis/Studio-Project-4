using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStateBehaviourHost : UIStateBehaviourBase
{
    [SerializeField]
    InputField passwordInput;
    [SerializeField]
    Button confirmButton;
    [SerializeField]
    Button backButton;

    private void Awake()
    {
        confirmButton.onClick.AddListener(confirmButtonClicked);
        backButton.onClick.AddListener(backToPrevState);
    }

    private void confirmButtonClicked()
    {
        NetworkClient.instance.Host(passwordInput.text);
        UIStateBehaviourLobby.isHost = true;
        UIStateActiveManager.currentActiveManager.setNextState("StateLobby");
    }
}
