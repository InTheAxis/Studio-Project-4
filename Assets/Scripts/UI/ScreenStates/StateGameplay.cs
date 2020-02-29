using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateGameplay : State
{
    [SerializeField]
    private GameObject hud = null;

    public override string Name { get { return "Gameplay"; } }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    public override void onShow()
    {
        hud.SetActive(true);
        base.onShow();
    }
}
