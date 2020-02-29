using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetGameplayState : MonoBehaviour
{
    private void Start()
    {
        StateController.showNext("Gameplay");
    }

    private void Update()
    {
        
    }
}
