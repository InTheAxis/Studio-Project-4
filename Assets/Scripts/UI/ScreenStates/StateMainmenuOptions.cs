using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using System.Linq;

public class StateMainmenuOptions : StateGenericOptions
{
  
    public override string Name { get { return "MainmenuOptions"; } }


    public override void onShow()
    {
        base.onShow();
        StartCoroutine(StateController.fadeCanvasGroup(GetComponent<CanvasGroup>(), true, 10.0f));
    }
}
