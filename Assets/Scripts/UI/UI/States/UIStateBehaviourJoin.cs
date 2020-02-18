using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStateBehaviourJoin : UIStateBehaviourBase
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
        NetworkClient.instance.Join(passwordInput.text);
        UIStateBehaviourLobby.isHost = false;
        UIStateActiveManager.currentActiveManager.setNextState("StateLobby");
    }
}
