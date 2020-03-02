using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateSplashscreen : State
{

    public override string Name { get { return "Splashscreen"; } }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    private IEnumerator showSplashScreen()
    {
        yield return new WaitForSeconds(2.5f);
        StateController.showNext("Login");
    }

    public override void onShow()
    {
        StartCoroutine(showSplashScreen());
        base.onShow();
    }
}
