using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMatchLobbyCharacter : State
{
    public override string Name { get { return "LobbyCharacter"; } }

    public void selectCharacter(GameObject go)
    {
        Debug.Log("Selected " + go.name);
    }

    public void Cancel()
    {
        StateController.Hide(Name);
    }

    public override void onShow()
    {
        base.onShow();
        StartCoroutine(StateController.fadeCanvasGroup(GetComponent<CanvasGroup>(), true, 10.0f));
    }
}
