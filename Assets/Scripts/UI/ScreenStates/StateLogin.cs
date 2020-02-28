using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class StateLogin : State
{
    [Header("References")]
    [SerializeField]
    [Tooltip("The status message that shows while logging in")]
    private TextMeshProUGUI tmStatus = null;

    [Header("Inputs")]
    [SerializeField]
    [Tooltip("Username Input Field")]
    private TMP_InputField tmUsername = null;
    [SerializeField]
    [Tooltip("Password Input Field")]
    private TMP_InputField tmPassword = null;


    

    public override string Name { get { return "Login"; } }

    private void Start()
    {
        tmStatus.text = "Connecting to Photon Servers...";
        NetworkClient.instance.masterServerConnectedCallback = onConnectedToPhotonServers;
        NetworkClient.instance.connectToMasterServer();
    }

    private void Update()
    {
        
    }

    public void Login()
    {
        Debug.Log("Username: " + tmUsername.text);
        Debug.Log("Password: " + tmPassword.text);

        /* Playfab Login */
        //tmStatus.color = busyColor;
        tmStatus.text = "Connecting...";

        PlayfabAuthenticator playfabAuthenticator = (PlayfabAuthenticator)DoNotDestroySingleton<PlayfabAuthenticator>.instance;
        playfabAuthenticator.Login(tmUsername.text, tmPassword.text,
            playerName =>
            {
                //setName(playerName);
                //setScene(ScreenStates.MAINMENU);
                StateController.showNext("Mainmenu");
            }, (errorMsg, errorType) =>
            {
                tmStatus.text = "Connection Failed";
            });
    }

    public void Register()
    {
        Debug.Log("Username: " + tmUsername.text);
        Debug.Log("Password: " + tmPassword.text);
    }

    public void useDebugAccount()
    {
        tmUsername.text = "Elson";
        tmPassword.text = "Elson123";
        Login();
    }

    public void useDebugAccount2()
    {
        tmUsername.text = "Jeff";
        tmPassword.text = "Jeff123";
        Login();
    }


    public void quickHost()
    {
        tmStatus.text = "Connecting to Photon Servers";
        NetworkClient.instance.Host("");
    }

    public void quickJoin()
    {
        tmStatus.text = "Connecting to Photon Servers";
        tmUsername.text = getRandomGuestName();
        NetworkClient.instance.Join("");
    }

    private string getRandomGuestName()
    {
        return "Guest" + Random.Range(0, 10000);
    }

    public override void onShow()
    {
        tmUsername.text = "";
        tmStatus.text = "";
        tmPassword.text = "";
        base.onShow();
    }



    /* NETWORKING */

    private void onConnectedToPhotonServers()
    {
        tmStatus.text = "";
    }
}
