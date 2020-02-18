using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharJumpCheck : MonoBehaviour
{
    //public Collider charBot;
    public bool airborne { private set; get; }

    public void Jumped()
    {
        airborne = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (other != charBot)
            airborne = false;        
    }

    private void OnTriggerStay(Collider other)
    {
        //if (other != charBot)
            airborne = false;
    }
}
