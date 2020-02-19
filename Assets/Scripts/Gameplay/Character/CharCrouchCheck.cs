using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharCrouchCheck : MonoBehaviour
{
    public Collider charTop;
    public LayerMask ignore;
    public bool crouching { private set; get; }

    private bool canUnCrouch;
    private bool keyPressed;

    private void Start()
    {
        canUnCrouch = true;
        crouching = false;
    }

    public void Crouch(bool b)
    {
        keyPressed = b;
    }

    private void Update()
    {
        if (canUnCrouch)
        { 
            crouching = keyPressed;
        }
        else        
            crouching = true;
        
        charTop.enabled = !crouching;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (LayerMaskExt.CheckIfNotIgnored(ignore, other.gameObject.layer))
            canUnCrouch = false;
    }
    private void OnTriggerExit(Collider other)
    {
        canUnCrouch = true;
    }
}
