using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateController : MonoBehaviour
{

    private Stack<State> history = null;
    private Dictionary<int, State> screens = null;

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    private void setScene(State targetState)
    {
        //screens[(int)targetState].SetActive(true);
        //screens[(int)currentScreen].SetActive(false);
        //history.Push(currentScreen);
        //currentScreen = targetState;
    }
}
