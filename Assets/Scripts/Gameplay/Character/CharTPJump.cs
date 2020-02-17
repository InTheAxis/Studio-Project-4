using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharTPJump : MonoBehaviour
{
    public Collider charBot;
    public bool jumping { private set; get; }

    private bool canJump;
    private bool keyPressed;

    private void Start()
    {
        canJump = true;
        jumping = false;
    }

    public void SetInput(bool b)
    {
        keyPressed = b;
        if (canJump && keyPressed)
            jumping = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other != charBot)
            canJump = true; 
    }
    private void OnTriggerExit(Collider other)
    {
        canJump = false;
        jumping = false;
    }
}
