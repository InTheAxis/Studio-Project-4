using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMatchLobby : State
{ 

    /* Keep track of players and their IDS */
    private static List<LobbyPlayer> players = null;
    private static Dictionary<string, int> playerIDs = null;

    public override string Name { get { return "MatchLobby"; } }

    private void Start()
    {
        NetworkClient.instance.roomJoinedCallback += onJoinRoom;
    }

    private void Update()
    {
        
    }

    /* NETWORKING */

    private void onJoinRoom(List<string> playersInLobby)
    {
        playerIDs.Clear();
        for (int i = 0; i < 5; ++i)
        {
            if (i < playersInLobby.Count)
            {
                players[i].setActive(playersInLobby[i]);
                playerIDs.Add(playersInLobby[i], i);
                Debug.Log(playersInLobby[i]);
            }
            else
            {
                players[i].setInactive();
            }
        }

        if (playersInLobby.Count <= 0) return;
        //tmReady.text = PhotonNetwork.IsMasterClient ? "Start" : "Ready";

        if (PhotonNetwork.IsMasterClient)
        {
            //tmReady.transform.parent.GetComponent<Button>().enabled = false;
            //tmReady.GetComponent<TextMeshProUGUI>().color = failColor;
        }

    }
}
