using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StateSpectate : State
{
    [SerializeField]
    private TextMeshProUGUI tmUsername = null;

    public override string Name { get { return "Spectate"; } }

    public void Spectate(string username)
    {

    }

    public void selectPrevious()
    {

    }

    public void selectNext()
    {

    }

    public void Disconnect()
    {

    }
}
