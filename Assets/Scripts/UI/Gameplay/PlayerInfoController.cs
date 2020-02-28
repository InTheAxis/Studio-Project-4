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

        for (int i = 0; i < transform.childCount; ++i)
            transform.GetChild(i).gameObject.SetActive(false);
        foreach (var p in CharTPController.PlayerControllerRefs)
            addPlayerInfo(p);

        CharTPController.OnPlayerAdd += addPlayerInfo;
        CharTPController.OnPlayerRemoved += removePlayerInfo;
    }
    private void OnDestroy()
    {
        CharTPController.OnPlayerAdd -= addPlayerInfo;
        CharTPController.OnPlayerRemoved -= removePlayerInfo;
        foreach (var p in CharTPController.PlayerControllerRefs)
            p.controller.GetComponent<CharHealth>().OnHealthChange -= updateHealth;
    }

    private void Update()
    {
    }

    private PlayerInfo setupPlayerInfo(CharTPController player)
    {
        return setupPlayerInfo(player.photonView.Owner.NickName, player.GetComponent<CharHealth>());
    }
    private PlayerInfo setupPlayerInfo(string playerName, CharHealth playerHealth)
    {
        return setupPlayerInfo(playerName, playerHealth.hp, playerHealth.invulnerable);
    }
    private PlayerInfo setupPlayerInfo(string playerName, int health, bool invulnerable)
    {
        return setupPlayerInfo(playerName, playerInfo.Count, health, invulnerable);
    }
    private PlayerInfo setupPlayerInfo(string playerName, int index, int health, bool invulnerable)
    {
        Transform t = transform.GetChild(index);
        t.gameObject.SetActive(true);
        PlayerInfo newPlayer = new PlayerInfo(t, playerName);
        newPlayer.setHealth(health, invulnerable);
        return newPlayer;
    }

    private void addPlayerInfo(CharTPController.PlayerControllerData newPlayer)
    {
        addPlayerInfo(newPlayer.controller);
    }
    private void addPlayerInfo(CharTPController newPlayer)
    {
        playerInfo.Add(setupPlayerInfo(newPlayer));

        newPlayer.GetComponent<CharHealth>().OnHealthChange += updateHealth;
    }
    private void removePlayerInfo(CharTPController.PlayerControllerData player)
    {
        removePlayerInfo(player.name);
    }
    private void removePlayerInfo(string name)
    {
        bool foundPlayer = false;
        for (int i = 0; i < transform.childCount; ++i)
        {
            if (i >= playerInfo.Count) // No player occupies this Player HUD slot. Set as inactive
                transform.GetChild(i).gameObject.SetActive(false);
            else if (foundPlayer) // Replace old PlayerInfo with a new one targeting the Player HUD child above its current one
                playerInfo[i] = setupPlayerInfo(playerInfo[i].name, i, playerInfo[i].health, playerInfo[i].invulnerable);
            else if (playerInfo[i].name == name) // Remove and replace the PlayerInfo that takes over its place
            {
                foundPlayer = true;
                playerInfo.RemoveAt(i);
                if (i < playerInfo.Count)
                    playerInfo[i] = setupPlayerInfo(playerInfo[i].name, i, playerInfo[i].health, playerInfo[i].invulnerable);
                else
                    transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
    private void updateHealth(CharTPController player, int newAmt, bool invulnerable)
    {
        getTargetPlayerInfo(player.photonView.Owner.NickName).setHealth(newAmt, invulnerable);
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
