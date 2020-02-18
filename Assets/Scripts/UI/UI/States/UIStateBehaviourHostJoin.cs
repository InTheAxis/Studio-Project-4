using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStateBehaviourHostJoin : UIStateBehaviourBase
{
    [SerializeField]
    Button hostButton;
    [SerializeField]
    Button joinButton;
    [SerializeField]
    Button randomButton;

    private void Awake()
    {
        hostButton.onClick.AddListener(hostButtonClicked);
        joinButton.onClick.AddListener(joinButtonClicked);
        randomButton.onClick.AddListener(randomButtonClicked);
    }

    private void hostButtonClicked()
    {
        UIStateActiveManager.currentActiveManager.setNextState("StateHost");
    }
    private void joinButtonClicked()
    {
        UIStateActiveManager.currentActiveManager.setNextState("StateJoin");
    }
    private void randomButtonClicked()
    {
        NetworkClient.instance.Join("");
        UIStateActiveManager.currentActiveManager.setNextState("StateLobby");
    }
}
