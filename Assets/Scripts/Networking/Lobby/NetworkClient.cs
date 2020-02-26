using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class NetworkClient : MonoBehaviourPunCallbacks
{
    public static NetworkClient instance = null;

    [SerializeField]
    private string ingameSceneName;

    public System.Action masterServerConnectedCallback;
    public System.Action<List<string>> roomJoinedCallback;
    public System.Action randomRoomJoinFailedCallback;
    public System.Action roomJoinFailedCallback;
    public System.Action roomCreateFailedCallback;
    public System.Action<string> playerJoinedCallback;
    public System.Action<string> playerLeftCallback;

    private void Awake()
    { // Singleton
        if (instance == null)
            instance = this;
        else
        {
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);

        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void connectToMasterServer()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        masterServerConnectedCallback?.Invoke();
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("PUN: OnDisconnected() was called by PUN with reason {0}", cause);
    }

    public void Host(string password)
    {
        // An empty password hosts a random room
        Debug.Log("Creating room with password " + password);
        //setPlayerProperty("isHunter", true);
        setPlayerProperty("charModel", 0);

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 3;
        roomOptions.EmptyRoomTtl = 0;
        roomOptions.PlayerTtl = 0;
        if (password.Length > 0)
            roomOptions.IsVisible = false;
        PhotonNetwork.CreateRoom(password, roomOptions);
    }
    public void Join(string password)
    {
        Debug.Log("Joining room with password " + password);
        //setPlayerProperty("isHunter", false);
        setPlayerProperty("charModel", 0);

        if (password.Length > 0)
            PhotonNetwork.JoinRoom(password);
        else
            PhotonNetwork.JoinRandomRoom();
    }
    public void DisconnectFromRoom()
    {
        NetworkClientPView.instance.disconnect(PlayerSettings.playerName);
        PhotonNetwork.LeaveRoom();
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room!");
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.InstantiateSceneObject("NetworkLobbyManager", Vector3.zero, Quaternion.identity);

        List<string> players = new List<string>();
        
        foreach (var p in PhotonNetwork.PlayerList)
            players.Add(p.NickName);
        roomJoinedCallback?.Invoke(players);
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Join Random Failed: " + returnCode + " / " + message);
        UIStateBehaviourLobby.isHost = true;
        randomRoomJoinFailedCallback?.Invoke();
        Host("");
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Join Room Failed: " + returnCode + " / " + message);
        roomJoinFailedCallback?.Invoke();
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Create Room Failed: " + returnCode + " / " + message);
        roomCreateFailedCallback?.Invoke();
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        playerJoinedCallback?.Invoke(newPlayer.NickName);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        playerLeftCallback?.Invoke(otherPlayer.NickName);
    }
    
    public void setRoomVisibility(bool visible)
    {
        PhotonNetwork.CurrentRoom.IsOpen = visible;
    }

    public void goInGame()
    {
        //PhotonView photonView = PhotonView.Get(NetworkClientPView.instance);
        //photonView.RPC("goInGameRequestReceived", RpcTarget.All);
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel(ingameSceneName);
        else
            Debug.LogError("Attempted to go in game as a non-master client! This should not be possible");
    }

    public static void setPlayerProperty(string key, object v)
    {
        Hashtable hashTable = PhotonNetwork.LocalPlayer.CustomProperties;
        hashTable[key] = v;
        PhotonNetwork.LocalPlayer.SetCustomProperties(hashTable);
    }
    public static object getPlayerProperty(string key)
    {
        return PhotonNetwork.LocalPlayer.CustomProperties[key];
    }


    #region Ready Functions
    public List<string> getReadyPlayers()
    {
        return NetworkClientPView.instance.getReadyPlayers();
    }
    public bool isReady(string playerName)
    {
        return NetworkClientPView.instance.isReady(playerName);
    }
    public bool areAllReady()
    {
        return NetworkClientPView.instance.areAllReady(PhotonNetwork.CurrentRoom.PlayerCount - 1);
    }
    public void toggleReady(string playerName)
    {
        NetworkClientPView.instance.toggleReady(playerName);
    }
    #endregion
}
