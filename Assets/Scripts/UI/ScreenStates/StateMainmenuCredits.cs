using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMainmenuCredits : State
{
    public override string Name { get { return "MainmenuCredits"; } }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    public override void onShow()
    {
        base.onShow();
        StartCoroutine(StateController.fadeCanvasGroup(GetComponent<CanvasGroup>(), true, 10.0f, 0.50f));
    }
}
