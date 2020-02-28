using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StateMainmenuPlay : State
{
    [Header("References")]
    [SerializeField]
    [Tooltip("The status message for hosting a room.")]
    private TextMeshProUGUI tmHostStatus = null;
    [SerializeField]
    [Tooltip("The status message for joining a room.")]
    private TextMeshProUGUI tmJoinStatus = null;

    [Header("Inputs")]
    [SerializeField]
    [Tooltip("Host Password's Input Field")]
    private TMP_InputField tmHostPass = null;
    [SerializeField]
    [Tooltip("Join Password's Input Field")]
    private TMP_InputField tmJoinPass = null;

    private bool isConnecting = false;
    public override string Name { get { return "MainmenuPlay"; } }

    private void Start()
    {
        NetworkClient.instance.roomCreateFailedCallback = onCreateLobbyFailed;
        NetworkClient.instance.roomJoinFailedCallback = onJoinRoomFailed;
        NetworkClient.instance.randomRoomJoinFailedCallback = onJoinRandomRoomFailed;
        NetworkClient.instance.roomJoinedCallback += onJoinRoomSuccess;

    }

    private void Update()
    {
        
    }

    public void hostRoom()
    {
        isConnecting = true;
        tmHostStatus.text = "Creating a room...";
        NetworkClient.instance.Host(tmHostPass.text);
    }

    public void joinRoom()
    {
        isConnecting = true;
        tmJoinStatus.text = "Joining room...";
        NetworkClient.instance.Join(tmJoinPass.text);
    }

    public void joinRandom()
    {
        isConnecting = true;
        tmJoinStatus.text = "Joining random...";
        NetworkClient.instance.Join("");
    }


    public override void onShow()
    {
        isConnecting = false;
        base.onShow();
    }


    /* NETWORKING */

    private void onJoinRoomSuccess(List<string> playersInRoom)
    {
        if(isConnecting)
            StateController.showNext("MatchLobby");
    }

    private void onCreateLobbyFailed()
    {
        tmHostStatus.text = "Lobby already exists";
    }

    private void onJoinRandomRoomFailed()
    {
        
    }

    private void onJoinRoomFailed()
    {
        tmJoinStatus.text = "Lobby does not exist / is in progress.";
    }


}
