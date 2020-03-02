using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateGameOptions : StateGenericOptions
{
    public override string Name { get { return "GameOptions"; } }

    private void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            StateController.showNext("Gameplay");
    }

    public void backToPause()
    {
        StateController.showPrevious();
    }



}
