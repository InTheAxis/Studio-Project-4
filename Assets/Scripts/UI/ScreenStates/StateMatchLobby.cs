using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
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
    [SerializeField]
    [Tooltip("The button for accessing Map")]
    private GameObject mapMenuButton = null;
    [SerializeField]
    [Tooltip("The sliding indicator that shows what sub-menu is opened at the moment.")]
    private RectTransform indicator = null;
    

    [SerializeField]
    private StateMatchLobbyCharacter stateLobbyChar;

    /* Keep track of players and their IDS */
    private static List<LobbyPlayer> players;
    private static Dictionary<string, int> playerIDs;
    private Button btnReady = null;
    private IEnumerator iSlideIndicator = null;

    public override string Name { get { return "MatchLobby"; } }

    private void Awake()
    {
        StateController.Register(this);



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
                tmReady.GetComponent<TextMeshProUGUI>().color = ThemeColors.positive;
            }
            else
            {
                btnReady.enabled = false;
                tmReady.GetComponent<TextMeshProUGUI>().color = ThemeColors.neutral;
            }
        }

    }

    public void Ready()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Player toBeMasterClient;
            string chkValidMsg = checkValidCharSelection(out toBeMasterClient);
            if (chkValidMsg.Length != 0)
            {
                Debug.Log(chkValidMsg);
                return;
            }

            ScreenStateHelperNetwork.instance.startGame(toBeMasterClient);
        }
        else
        {
            NetworkClient.instance.toggleReady(PlayerSettings.playerName);
        }
    }
    private string checkValidCharSelection(out Player playerToBeMaster)
    {
        playerToBeMaster = null;
        Player monsterPlayer = null;
        foreach (var p in PhotonNetwork.PlayerList)
            if ((int)NetworkClient.getPlayerProperty(p, "charModel") == 0)
                if (monsterPlayer == null)
                    monsterPlayer = p;
                else
                    return "Only 1 Player may be the Monster";

        if (monsterPlayer == null)
            return "There must be 1 Player who is the Monster";
        else
        {
            playerToBeMaster = monsterPlayer;
            return "";
        }
    }

    public void Leave()
    {
        if (NetworkClient.instance != null)
            NetworkClient.instance.DisconnectFromRoom();
        StateController.showPrevious();
    }

    public void showTutorial()
    {
        StateController.Show("Tutorial");
    }

    /* Slides the indicator up and down based on the nav button pressed */
    private void selectNavButton(GameObject go)
    {
        indicator.gameObject.SetActive(true);
        if (iSlideIndicator != null)
            StopCoroutine(iSlideIndicator);
        iSlideIndicator = slideIndicator(go.GetComponent<RectTransform>().anchoredPosition.y);
        StartCoroutine(iSlideIndicator);
    }

    /* Animate the sliding of the indicator in the Y axis */
    private IEnumerator slideIndicator(float targetY)
    {
        Vector2 anchoredPos = indicator.anchoredPosition;
        while (Mathf.Abs(targetY - anchoredPos.y) > 0.005f)
        {
            anchoredPos.y = Mathf.Lerp(anchoredPos.y, targetY, Time.deltaTime * 12.0f);
            indicator.anchoredPosition = anchoredPos;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        indicator.anchoredPosition = anchoredPos;
    }

    public void selectSubMenu(GameObject go)
    {
        selectNavButton(go);
        StateController.Show(go.name);
    }

   

    public override void onShow()
    {
        if (NetworkClient.instance != null)
        {
            NetworkClient.instance.playerJoinedCallback = onPlayerJoin;
            NetworkClient.instance.playerLeftCallback = onPlayerLeave;
        }

        mapMenuButton.SetActive(PhotonNetwork.IsMasterClient);
        worldCanvas.SetActive(true);
        StateController.Hide("LobbyCharacter");
        StateController.Hide("LobbyMap");

        base.onShow();

        StartCoroutine(registerCallbackCour());
    }
    private IEnumerator registerCallbackCour()
    {
        while (ScreenStateHelperNetwork.instance == null)
            yield return null;

        ScreenStateHelperNetwork.instance.modelChangeCallback += receivedCharModelChange;
    }

    public override void onHide()
    {
        worldCanvas.SetActive(false);

        if (ScreenStateHelperNetwork.instance != null)
            ScreenStateHelperNetwork.instance.modelChangeCallback -= receivedCharModelChange;
        base.onHide();
    }

    /* NETWORKING */

    #region Network Callbacks

    public void onJoinRoom(List<string> playersInLobby)
    {
        playerIDs.Clear();
        btnReady = tmReady.GetComponent<Button>();

        string playerName = PhotonNetwork.LocalPlayer.NickName;
        // Set this client as Model 0
        players[0].setActive(playerName);
        playerIDs.Add(playerName, 0);
        stateLobbyChar.setModel(0, (int)NetworkClient.getPlayerProperty("charModel"));

        // Set all other players starting from Model 1
        var networkPlayerList = PhotonNetwork.PlayerList;
        int offset = 1;
        for (int i = 0; i < 5; ++i)
        {
            if (i < playersInLobby.Count)
            {
                if (playersInLobby[i] == playerName)
                    offset = 0;
                else
                {
                    // Offset model index
                    players[i + offset].setActive(playersInLobby[i]);
                    stateLobbyChar.setModel(i + offset, (int)NetworkClient.getPlayerProperty(networkPlayerList[i], "charModel"));
                    playerIDs.Add(playersInLobby[i], i + offset);
                }
            }
            else
            {
                players[i].setInactive();
            }

        }

        if (playersInLobby.Count <= 0)
            return;
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
        Player newPlayer = PhotonNetwork.PlayerList[PhotonNetwork.PlayerList.Length - 1];

        players[playerIDs.Count].setActive(name);
        stateLobbyChar.setModel(playerIDs.Count, (int)NetworkClient.getPlayerProperty(newPlayer, "charModel"));
        playerIDs.Add(name, playerIDs.Count);
    }

    private void onPlayerLeave(string name)
    {
        Debug.Log(name + "with ID: " + playerIDs.Count + " left!");
        playerIDs.Remove(name);
        players[playerIDs.Count].setInactive();

    }

    #endregion

    // Received character model change from remote client
    public void receivedCharModelChange(string playerName, int modelIndex)
    {
        stateLobbyChar.setModel(playerIDs[playerName], modelIndex);
    }

}
