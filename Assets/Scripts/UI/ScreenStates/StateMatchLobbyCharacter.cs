using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMatchLobbyCharacter : State
{
    public override string Name { get { return "LobbyCharacter"; } }

    public void selectCharacter(GameObject go)
    {
        Debug.Log("Selected " + go.name);
        switch (go.name)
        {
            case "Monster":
                NetworkClient.setPlayerProperty("charModel", 0);
                break;
            case "Male":
                NetworkClient.setPlayerProperty("charModel", 1);
                break;
            case "Female":
                NetworkClient.setPlayerProperty("charModel", 2);
                break;
        }
    }

    public void Cancel()
    {
        StateController.Hide(Name);
    }
}
