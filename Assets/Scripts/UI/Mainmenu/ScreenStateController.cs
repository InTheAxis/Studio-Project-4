using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ScreenStateController : MonoBehaviour
{

    private enum ScreenStates
    {
        CONNECTINGTOPHOTON,
        LOGIN,
        MAINMENU,
        SERVERSELECT,
        CONNECTINGTOSERVER,
        MATCHLOBBY,
        LOADING,
        OPTIONS,
        CREDITS,
        COUNT,
    }

    [Header("Screens")]
    [SerializeField]
    [Tooltip("Stores a collection of different menu screens")]
    private GameObject[] screens = null;
    [SerializeField]
    private TextMeshProUGUI tmUsername = null;

    [Header("Models")]
    [SerializeField]
    private GameObject mainmenuModel = null;
    [SerializeField]
    private GameObject lobbyModels = null;

    [Header("Interactions")]
    [SerializeField]
    [Tooltip("The size of the button when hovered")]
    private float hoverGrowSize = 1.2f;
    [SerializeField]
    private Color busyColor = Color.yellow;
    [SerializeField]
    private Color failColor = Color.red;
    [SerializeField]
    private Color successColor = Color.green;

    [Header("Transitions")]
    [SerializeField]
    private ButtonMaskEffect buttonMask = null;
    [SerializeField]
    private float buttonMaskStartY = 78.0f;
    [SerializeField]
    private float buttonHeightGap = 120.0f;
    [SerializeField]
    private float canvasFadeSpeed = 2.0f;

    [Header("Login")]
    [SerializeField]
    private CanvasGroup cgLogin = null;
    [SerializeField]
    private TMP_InputField tmLoginUsername = null;
    [SerializeField]
    private TMP_InputField tmLoginPassword = null;
    [SerializeField]
    private TextMeshProUGUI tmLoginStatus = null;

    [Header("Registration")]
    [SerializeField]
    private CanvasGroup cgRegister = null;
    [SerializeField]
    private TMP_InputField tmRegisterEmail = null;
    [SerializeField]
    private TMP_InputField tmRegisterUsername = null;
    [SerializeField]
    private TMP_InputField tmRegisterPassword = null;
    [SerializeField]
    private TMP_InputField tmRegisterConfirm = null;
    [SerializeField]
    private TextMeshProUGUI tmRegisterStatus = null;

    [Header("Server Selection")]
    [SerializeField]
    private CanvasGroup cgHost = null;
    [SerializeField]
    private CanvasGroup cgJoin = null;
    [SerializeField]
    private GameObject registerInput = null;
    [SerializeField]
    private GameObject loginInput = null;
    [SerializeField]
    private GameObject hostPasswordInput = null;
    [SerializeField]
    private GameObject joinPasswordInput = null;

    private List<GameObject> hovered;
    private List<GameObject> prevHovered;
    private List<RaycastResult> raycastResults;

    /* Interactions */
    private Vector3 hoverGrowScale = Vector3.zero;

    /* Screen State */
    [SerializeField]
    private ScreenStates currentScreen = ScreenStates.CONNECTINGTOPHOTON;

    private Stack<ScreenStates> history = null;

    /* TODO: Player Usernames */
    private Dictionary<string, GameObject> playerModels = null;


    private void Start()
    {
        // Attach callbacks
        NetworkClient.instance.masterServerConnectedCallback = connectedToPhotonServers;
        NetworkClient.instance.roomJoinedCallback = joinedLobby;
        NetworkClient.instance.randomRoomJoinFailedCallback = failedToJoinRandomLobby;
        NetworkClient.instance.roomJoinFailedCallback = failedToJoinLobby;
        NetworkClient.instance.roomCreateFailedCallback = failedToCreateLobby;
        NetworkClient.instance.playerJoinedCallback = playerJoined;
        NetworkClient.instance.playerLeftCallback = playerLeft;

        history = new Stack<ScreenStates>();
        hovered = new List<GameObject>();
        prevHovered = new List<GameObject>();
        hoverGrowScale = new Vector3(hoverGrowSize, hoverGrowSize, hoverGrowSize);

        for (int i = 0; i < screens.Length; ++i)
        {
            if (i == (int)currentScreen)
                screens[i].SetActive(true);
            else
                screens[i].SetActive(false);
        }

        mainmenuModel.SetActive(true);
        lobbyModels.SetActive(false);

        NetworkClient.instance.connectToMasterServer();
    }
    private void OnDestroy()
    {
        NetworkClient.instance.masterServerConnectedCallback = null;
        NetworkClient.instance.roomJoinedCallback = null;
        NetworkClient.instance.randomRoomJoinFailedCallback = null;
        NetworkClient.instance.roomJoinFailedCallback = null;
        NetworkClient.instance.roomCreateFailedCallback = null;
        NetworkClient.instance.playerJoinedCallback = null;
        NetworkClient.instance.playerLeftCallback = null;
    }

    private void Update()
    {
        // TODO: Update start button activeness depending on player readiness
    }


    private void getHoveredUIElements()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);
    }

    private void setScene(ScreenStates targetState)
    {
        screens[(int)targetState].SetActive(true);
        screens[(int)currentScreen].SetActive(false);
        history.Push(currentScreen);
        currentScreen = targetState;
    }

    /* Network callbacks */
    #region Network Callbacks
    private void connectedToPhotonServers()
    {
        // This function is called each time we connect to photon servers, including when reconnecting to photon servers after leaving a room.
        // Therefore, we need this check
        if (currentScreen == ScreenStates.CONNECTINGTOPHOTON)
            setScene(ScreenStates.LOGIN);
    }

    private void joinedLobby(List<string> playersInLobby)
    {
        // Note: This function is called while either joining or hosting. Essentially the callback for when we enter the room.

        // playersInLobby is inclusive of the user themselves
        // TODO: Set player names
        // Note: Planning for the host to always be the hunter. The host is always the first string in the list

        if (currentScreen == ScreenStates.CONNECTINGTOSERVER)
        {
            // TODO: Set button to be either start or ready, depending on is host or not
            // if (PhotonNetwork.isMasterClient)

            setScene(ScreenStates.MATCHLOBBY);
            mainmenuModel.SetActive(false);
            lobbyModels.SetActive(true);
        }
        else
        {
            // Backed out from connecting to server before receiving "joined room" callback. Need send disconnect request
            NetworkClient.instance.DisconnectFromRoom();
        }
    }
    private void failedToJoinRandomLobby()
    {
        // TODO: State switched from joining to hosting. Need to change whatever variables that depend on this state
    }
    private void failedToJoinLobby()
    {
        // TODO: Show error message to user
        // Reasons for error could be:
        // - Lobby does not exist
        // - Lobby is currently in-game
    }
    private void failedToCreateLobby()
    {
        // TODO: Show error message to user
        // Reasons for error could be:
        // - Lobby already exists
    }
    private void playerJoined(string playerName)
    {
        // TODO: Add player name to list
    }
    private void playerLeft(string playerName)
    {
        // TODO: Remove player name from list
    }
    #endregion


    public void onButtonClick(string name)
    {
        if (name == "Back")
        {
            screens[(int)currentScreen].SetActive(false);
            currentScreen = history.Pop();
            screens[(int)currentScreen].SetActive(true);
            buttonMask.Begin(buttonMaskStartY);

            /* TODO: Might be wrong (Elson) */
            if (currentScreen != ScreenStates.MATCHLOBBY && currentScreen != ScreenStates.OPTIONS)
            {
                mainmenuModel.SetActive(true);
                lobbyModels.SetActive(false);
            }
        }
        else if (name == "Login")
        {
            /* Playfab Login */
            tmLoginStatus.color = busyColor;
            tmLoginStatus.text = "Connecting...";

            PlayfabAuthenticator playfabAuthenticator = (PlayfabAuthenticator)DoNotDestroySingleton<PlayfabAuthenticator>.instance;
            playfabAuthenticator.Login(tmLoginUsername.text, tmLoginPassword.text,
                playerName =>
                {
                    PlayerSettings.playerName = playerName;
                    setScene(ScreenStates.MAINMENU);
                    tmUsername.text = tmLoginUsername.text;
                }, (errorMsg, errorType) =>
                {
                    tmLoginStatus.color = failColor;
                    tmLoginStatus.text = "Connection Failed";
                });
        }
        else if (name == "Register")
        {

            /* Invalid Email */
            if(!tmRegisterEmail.text.Contains("@"))
            {
                tmRegisterStatus.color = failColor;
                tmRegisterStatus.text = "Invalid Email";
            }
            /* Empty Fields */
            else if(tmRegisterEmail.text.Equals("") || tmRegisterUsername.text.Equals("") || tmRegisterPassword.text.Equals("") || tmRegisterConfirm.text.Equals("")) /*|| tmRegisterUsername.text == "" || tmRegisterPassword.text = "" || tmRegisterConfirm.text == "")*/
            {
                tmRegisterStatus.color = failColor;
                tmRegisterStatus.text = "Invalid Fields";
            }
            /* Password mismatch */
            else if (!tmRegisterPassword.text.Equals(tmRegisterConfirm.text))
            {
                tmRegisterStatus.color = failColor;
                tmRegisterStatus.text = "Password Mismatch";
            }
            else
            {
                tmLoginStatus.color = busyColor;
                tmRegisterStatus.text = "Registering";

                /* Playfab Register */
                PlayfabAuthenticator playfabAuthenticator = (PlayfabAuthenticator)DoNotDestroySingleton<PlayfabAuthenticator>.instance;
                playfabAuthenticator.Register(tmRegisterUsername.text, tmRegisterPassword.text, tmRegisterEmail.text,
                    playerName =>
                    {
                        PlayerSettings.playerName = playerName;
                        setScene(ScreenStates.MAINMENU);
                        tmUsername.text = tmRegisterUsername.text;
                    }, (errorMsg, errorType) =>
                    {
                        tmRegisterStatus.color = failColor;
                        tmRegisterStatus.text = "Failed to Register";
                    });
            }

        }
        else if (name == "Logout")
        {
            /* Playfab Logout */
            PlayerSettings.playerName = null;

            setScene(ScreenStates.LOGIN);
            buttonMask.Begin(buttonMaskStartY);
            loginInput.SetActive(false);
            registerInput.SetActive(false);
        }
        else if (name == "MainmenuLogin")
        {
            loginInput.SetActive(!loginInput.activeSelf);
            registerInput.SetActive(false);
            if(loginInput.activeSelf)
                StartCoroutine(fadeCanvasGroup(cgLogin, true));
        }
        else if (name == "MainmenuRegister")
        {
            loginInput.SetActive(false);
            registerInput.SetActive(!registerInput.activeSelf);
            if(registerInput.activeSelf)
                StartCoroutine(fadeCanvasGroup(cgRegister, true));
        }
        else if (name == "MainmenuPlay")
        {
            setScene(ScreenStates.SERVERSELECT);
            buttonMask.Begin(buttonMaskStartY + buttonHeightGap * 3);

            joinPasswordInput.SetActive(false);
            hostPasswordInput.SetActive(false);
        }
        else if (name == "MainmenuOptions")
        {
            buttonMask.Begin(buttonMaskStartY + buttonHeightGap * 2);
        }
        else if (name == "MainmenuCredits")
        {
            buttonMask.Begin(buttonMaskStartY + buttonHeightGap * 1);
        }
        else if (name == "Exit")
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
        }
        else if (name == "ServerSelectHost")
        {
            hostPasswordInput.SetActive(!hostPasswordInput.activeSelf);
            joinPasswordInput.SetActive(false);
            if (hostPasswordInput.activeSelf)
                StartCoroutine(fadeCanvasGroup(cgHost, true));
        }
        else if (name == "ServerJoin")
        {
            joinPasswordInput.SetActive(!joinPasswordInput.activeSelf);
            hostPasswordInput.SetActive(false);
            if (joinPasswordInput.activeSelf)
                StartCoroutine(fadeCanvasGroup(cgJoin, true));

        }
        else if (name == "ServerJoinRandom")
        {
            setScene(ScreenStates.CONNECTINGTOSERVER);
            NetworkClient.instance.Join("");
        }
        else if(name == "ServerHostCreate")
        {
            setScene(ScreenStates.CONNECTINGTOSERVER);
            NetworkClient.instance.Host(hostPasswordInput.GetComponent<TMP_InputField>().text);

            //setScene(ScreenStates.MATCHLOBBY);
            //mainmenuModel.SetActive(false);
            //lobbyModels.SetActive(true);
            /* TODO: Set player's name tag -> Get reference using Lobby Models -> Player -> Name */
        }
        else if(name == "ServerJoinRoom")
        {
            setScene(ScreenStates.CONNECTINGTOSERVER);
            NetworkClient.instance.Join(joinPasswordInput.GetComponent<TMP_InputField>().text);
            //setScene(ScreenStates.MATCHLOBBY);
            //mainmenuModel.SetActive(false);
            //lobbyModels.SetActive(true);
        }
        else if(name == "LobbyStart")
        {
            // TODO: This line loads the scene. Add this to loading class
            NetworkClient.instance.goInGame();
        }
        else if(name == "LobbyReady")
        {
            NetworkClient.instance.toggleReady(PlayerSettings.playerName);

            /* Pseudo Start Game */
            //mainmenuModel.SetActive(false);
            //lobbyModels.SetActive(false);
            //setScene(ScreenStates.LOADING);
            //screens[(int)ScreenStates.LOADING].GetComponent<Loading>().Load("Destructibles");
        }
        else if(name == "LobbyCharacter")
        {
            
        }
        else if(name == "LobbySettings")
        {

        }
        else if(name == "LobbyLeave")
        {
            NetworkClient.instance.DisconnectFromRoom();

            screens[(int)currentScreen].SetActive(false);
            // Pop twice because previous screen is "Connecting to Server"
            currentScreen = history.Pop();
            currentScreen = history.Pop();
            screens[(int)currentScreen].SetActive(true);
            buttonMask.Begin(buttonMaskStartY);

            /* TODO: Might be wrong (Elson) */
            if (currentScreen != ScreenStates.MATCHLOBBY && currentScreen != ScreenStates.OPTIONS)
            {
                mainmenuModel.SetActive(true);
                lobbyModels.SetActive(false);
            }

        }
    }

    private IEnumerator fadeCanvasGroup(CanvasGroup canvas, bool fadeIn)
    {
        float targetAlpha = fadeIn ? 1.0f : 0.0f;
        canvas.alpha = 1.0f - targetAlpha;
        while (Mathf.Abs(targetAlpha - canvas.alpha) > 0.05f)
        {
            canvas.alpha = Mathf.Lerp(canvas.alpha, targetAlpha, Time.deltaTime * canvasFadeSpeed);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        canvas.alpha = targetAlpha;
        
    }

    public void onHoverEnterButton(GameObject go)
    {
        go.transform.localScale = hoverGrowScale;
    }

    public void onHoverExitButton(GameObject go)
    {
        go.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }
}
