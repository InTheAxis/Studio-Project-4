using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfoController : MonoBehaviour
{

    private List<PlayerInfo> playerInfo = null;


    private void Start()
    {
        playerInfo = new List<PlayerInfo>();
        for(int i = 0; i < transform.childCount; ++i)
        {
            Transform t = transform.GetChild(i);
            playerInfo.Add(new PlayerInfo(t));
        }

        getTargetPlayerInfo("Jeff").setHealth(2);
        getTargetPlayerInfo("Mark").setHealth(3);
        getTargetPlayerInfo("Dummy").setHealth(1);
    }

    private void Update()
    {
        
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
