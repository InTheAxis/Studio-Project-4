using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class StateDeath : State
{
    [SerializeField]
    private Volume postProcessing = null;
    [SerializeField]
    private TextMeshProUGUI tmCountdown = null;

    public override string Name { get { return "Death"; } }

    private ColorAdjustments adjustment = null;

    private void Start()
    {

    }

    private void Update()
    {
        
    }

    public void Spectate()
    {

    }

    public void Disconnect()
    {

    }

    public override void onShow()
    {
        Cursor.lockState = CursorLockMode.None;
        postProcessing.profile.TryGet(out adjustment);
        adjustment.saturation.value = -100.0f;
        base.onShow();
    }

    public override void onHide()
    {
        adjustment.saturation.value = 0.0f;
        base.onHide();
    }
}
