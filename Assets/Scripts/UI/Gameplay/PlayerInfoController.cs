using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerInfoController : MonoBehaviour
{

    private List<PlayerInfo> playerInfo = null;

    private void Start()
    {
        playerInfo = new List<PlayerInfo>();
        //for(int i = 0; i < transform.childCount; ++i)
        //{
        //    Transform t = transform.GetChild(i);
        //    playerInfo.Add(new PlayerInfo(t));
        //}

        //getTargetPlayerInfo("Jeff").setHealth(2);
        //getTargetPlayerInfo("Mark").setHealth(3);
        //getTargetPlayerInfo("Dummy").setHealth(1);

        for (int i = 0; i < transform.childCount; ++i)
            transform.GetChild(i).gameObject.SetActive(false);
        foreach (var p in CharTPController.PlayerControllerRefs)
            addPlayerInfo(p);
        CharTPController.OnPlayerAdd += addPlayerInfo;
        CharTPController.OnPlayerRemoved += removePlayerInfo;
    }

    private void Update()
    {
    }

    private PlayerInfo setupPlayerInfo(CharTPController player)
    {
        return setupPlayerInfo(player.photonView.Owner.NickName);
    }
    private PlayerInfo setupPlayerInfo(string playerName)
    {
        return setupPlayerInfo(playerName, playerInfo.Count);
    }
    private PlayerInfo setupPlayerInfo(string playerName, int index)
    {
        Transform t = transform.GetChild(index);
        t.gameObject.SetActive(true);
        PlayerInfo newPlayer = new PlayerInfo(t, playerName);
        newPlayer.setHealth(3);
        return newPlayer;
    }

    private void addPlayerInfo(CharTPController newPlayer)
    {
        playerInfo.Add(setupPlayerInfo(newPlayer));

        newPlayer.GetComponent<CharHealth>().OnHealthChange += updateHealth;
    }
    private void removePlayerInfo(CharTPController player)
    {
        bool foundPlayer = false;
        for (int i = 0; i < transform.childCount; ++i)
        {
            if (i >= playerInfo.Count) // No player occupies this Player HUD slot. Set as inactive
                transform.GetChild(i).gameObject.SetActive(false);
            else if (foundPlayer) // Replace old PlayerInfo with a new one targeting the Player HUD child above its current one
                playerInfo[i] = setupPlayerInfo(playerInfo[i].name, i);
            else if (playerInfo[i].name == player.photonView.Controller.NickName) // Remove and replace the PlayerInfo that takes over its place
            {
                foundPlayer = true;
                playerInfo.RemoveAt(i);
                if (i < playerInfo.Count)
                    playerInfo[i] = setupPlayerInfo(playerInfo[i].name, i);
                else
                    transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
    private void updateHealth(CharTPController player, int newAmt)
    {
        getTargetPlayerInfo(player.photonView.Owner.NickName).setHealth(newAmt);
    }

    private PlayerInfo getTargetPlayerInfo(string name)
    {
        for(int i = 0; i < playerInfo.Count; ++i)
        {
            if (playerInfo[i].name == name)
                return playerInfo[i];
        }
        return null;
    }
    
}
