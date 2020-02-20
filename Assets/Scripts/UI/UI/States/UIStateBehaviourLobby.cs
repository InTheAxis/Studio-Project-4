using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStateBehaviourLobby : UIStateBehaviourBase
{
    [SerializeField]
    private List<Text> playerNameTexts = new List<Text>();
    private List<string> currentPlayerNames = new List<string>();

    [SerializeField]
    private Button startButton;
    [SerializeField]
    private Button disconnectButton;

    public static bool isHost { get; set; }

    private void Awake()
    {
        startButton.onClick.AddListener(startButtonClicked);
        disconnectButton.onClick.AddListener(disconnectButtonClicked);
    }

    public override void onEnter()
    {
        NetworkClient.instance.roomJoinedCallback = joinedLobby;
        NetworkClient.instance.roomJoinFailedCallback = failedToJoinLobby;
        NetworkClient.instance.roomCreateFailedCallback = failedtoCreateLobby;
        NetworkClient.instance.playerJoinedCallback = playerJoined;
        NetworkClient.instance.playerLeftCallback = playerLeft;
        startButton.interactable = false;
        disconnectButton.interactable = true;

        foreach (var p in Photon.Pun.PhotonNetwork.PlayerList)
            currentPlayerNames.Add(p.NickName);
    }
    public override void update()
    { // Refresh the text with the list of players inside currentPlayerNames
        if (isHost)
            startButton.gameObject.SetActive(true);
        else
            startButton.gameObject.SetActive(false);

        for (int i = 0; i < playerNameTexts.Count; ++i)
            if (i >= currentPlayerNames.Count)
                if (i == 0)
                    playerNameTexts[i].text = "Connecting...";
                else
                    playerNameTexts[i].text = "Waiting...";
            else
                playerNameTexts[i].text = currentPlayerNames[i];
    }
    public override void onExit()
    {
        NetworkClient.instance.roomJoinedCallback = null;
        NetworkClient.instance.roomJoinFailedCallback = null;
        NetworkClient.instance.roomCreateFailedCallback = null;
        NetworkClient.instance.playerJoinedCallback = null;
        NetworkClient.instance.playerLeftCallback = null;
        currentPlayerNames.Clear();
    }

    private void startButtonClicked()
    {
        NetworkClient.instance.goInGame();
        startButton.interactable = false;
        disconnectButton.interactable = false;
    }
    private void disconnectButtonClicked()
    {
        NetworkClient.instance.DisconnectFromRoom();
        UIStateActiveManager.currentActiveManager.setToPrevState();
    }

    private void joinedLobby(List<string> players)
    {
        currentPlayerNames = players;
        startButton.interactable = true;
    }
    private void failedToJoinLobby()
    {
        UIStateActiveManager.currentActiveManager.setToPrevState();
        UIStateBehaviourMessage.messageString = "Failed to Join Lobby";
        UIStateActiveManager.currentActiveManager.setNextState("StateMessage");
    }
    private void failedtoCreateLobby()
    {
        UIStateActiveManager.currentActiveManager.setToPrevState();
        UIStateBehaviourMessage.messageString = "Lobby already exists or is in game";
        UIStateActiveManager.currentActiveManager.setNextState("StateMessage");
    }
    private void playerJoined(string playerName)
    {
        currentPlayerNames.Add(playerName);
    }
    private void playerLeft(string playerName)
    {
        currentPlayerNames.Remove(playerName);
    }
}
