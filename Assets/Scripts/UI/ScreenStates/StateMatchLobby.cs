using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StateMatchLobby : State
{
    [Header("References")]
    [SerializeField]
    [Tooltip("The canvas that holds objects in world space.")]
    private GameObject worldCanvas = null;
    [SerializeField]
    [Tooltip("The gameobject that holds all player 'slots' as children.")]
    private GameObject playerHolder = null;
    [SerializeField]
    [Tooltip("The button used to indicate whether a player is a ready (shows Start for Host)")]
    private TextMeshProUGUI tmReady = null;
    

    /* Keep track of players and their IDS */
    private static List<LobbyPlayer> players;
    private static Dictionary<string, int> playerIDs;
    private Button btnReady = null;
    public override string Name { get { return "MatchLobby"; } }

    private void Awake()
    {
        StateController.Register(this);
        NetworkClient.instance.playerJoinedCallback = onPlayerJoin;
        NetworkClient.instance.playerLeftCallback = onPlayerLeave;

        players = new List<LobbyPlayer>();
        playerIDs = new Dictionary<string, int>();
        for (int i = 0; i < playerHolder.transform.childCount; ++i)
        {
            Transform t = playerHolder.transform.GetChild(i);
            players.Add(new LobbyPlayer(t.gameObject));
        }

        worldCanvas.SetActive(false);
    }

    private void Update()
    {
        foreach (var p in PhotonNetwork.PlayerList)
        {
            string name = p.NickName;
            if (playerIDs.ContainsKey(name))
                players[playerIDs[name]].setReady(NetworkClient.instance.isReady(name));
        }

        if (PhotonNetwork.IsMasterClient && btnReady != null)
        {
            if (NetworkClient.instance.areAllReady())
            {
                btnReady.enabled = true;
                //tmReady.GetComponent<TextMeshProUGUI>().color = successColor;
            }
            else
            {
                btnReady.enabled = false;
                //tmReady.GetComponent<TextMeshProUGUI>().color = failColor;
            }

            //disable character select
            //charSelectButton.SetActive(false);
        }
        //else
        //    charSelectButton.SetActive(true);
    }

    public void Ready()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            NetworkClient.instance.setRoomVisibility(false);
            //mainmenuModel.SetActive(false);
            //lobbyModels.SetActive(false);
            //setScene(ScreenStates.LOADING);
            //screens[(int)ScreenStates.LOADING].GetComponent<Loading>().LoadPhoton("Gameplay");
            //NetworkClient.instance.goInGame();
            StateController.showNext("GameLoading");
            FindObjectOfType<StateGameLoading>().loadPhoton("Gameplay");
        }
        else
        {
            NetworkClient.instance.toggleReady(PlayerSettings.playerName);
        }
    }

    public void Leave()
    {
        NetworkClient.instance.DisconnectFromRoom();
        StateController.showPrevious();
    }

    public override void onShow()
    {
        worldCanvas.SetActive(true);
        base.onShow();
    }

    public override void onHide()
    {
        worldCanvas.SetActive(false);
        base.onHide();
    }

    /* NETWORKING */

    #region Network Callbacks

    public void onJoinRoom(List<string> playersInLobby)
    {
        Debug.Log("WORKS");
        playerIDs.Clear();
        btnReady = tmReady.GetComponent<Button>();

        for (int i = 0; i < 5; ++i)
        {
            if (i < playersInLobby.Count)
            {
                players[i].setActive(playersInLobby[i]);
                playerIDs.Add(playersInLobby[i], i);
            }
            else
            {
                players[i].setInactive();
            }
        }

        if (playersInLobby.Count <= 0) return;
        tmReady.text = PhotonNetwork.IsMasterClient ? "Start" : "Ready";

        if (PhotonNetwork.IsMasterClient)
        {
            tmReady.GetComponent<Button>().enabled = false;
            //tmReady.GetComponent<TextMeshProUGUI>().color = failColor;
        }

    }

    private void onPlayerJoin(string name)
    {
        Debug.Log(name + "with ID: " + playerIDs.Count + " joined!");
        players[playerIDs.Count].setActive(name);
        playerIDs.Add(name, playerIDs.Count);
    }

    private void onPlayerLeave(string name)
    {
        Debug.Log(name + "with ID: " + playerIDs.Count + " left!");
        playerIDs.Remove(name);
        players[playerIDs.Count].setInactive();

    }

    #endregion


}
