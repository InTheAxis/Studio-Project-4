using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStateActiveManager : MonoBehaviour
{
    // List of UI States
    private static Dictionary<string, UIStateStack> uiStateStacks = new Dictionary<string, UIStateStack>();
    private static UIStateActiveManager currentInstance;
    public static UIStateActiveManager currentActiveManager { get { return currentInstance; } }

    // Set which UI Stack this manager is associated with
    [SerializeField]
    private string nameOfStack;
    // Set the root state, if stack is uninitialized
    [SerializeField]
    private string rootState;
    private UIStateStack ownedStack;

    private string currentState;
    private string nextState;
    private UIStateBehaviourBase stateGameObject;

    private void Awake()
    {
        currentInstance = this;
        if (!uiStateStacks.ContainsKey(nameOfStack))
        {
            uiStateStacks.Add(nameOfStack, new UIStateStack());
            ownedStack = uiStateStacks[nameOfStack];
            ownedStack.Push(rootState);
        }
        else
            ownedStack = uiStateStacks[nameOfStack];

        currentState = ownedStack.getTop();
        nextState = "";
        stateGameObject = findStateBehaviour(currentState);
        stateGameObject.onEnter();
        stateGameObject.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (nextState.Length > 0)
        {
            stateGameObject.onExit();
            stateGameObject.gameObject.SetActive(false);
            stateGameObject = findStateBehaviour(nextState);
            stateGameObject.onEnter();
            stateGameObject.gameObject.SetActive(true);
            nextState = "";
        }

        stateGameObject.update();
    }


    public void setNextState(string state)
    {
        nextState = ownedStack.Push(state);
    }

    public void setToPrevState()
    {
        nextState = ownedStack.Pop();
    }

    // Pop until state specified or root, whichever is encountered first
    public void jumpToState(string state)
    {
        nextState = ownedStack.jumpTo(state);
    }

    public void jumpToRoot()
    {
        nextState = ownedStack.jumpToRoot();
    }

    public void jumpIfHasState(string state)
    {
        nextState = ownedStack.jumpIfHasState(state);
    }

    // True if state exists in stack
    public bool hasStateInStack(string state, bool ignoreTop = false)
    {
        return ownedStack.hasState(state, ignoreTop);
    }


    private UIStateBehaviourBase findStateBehaviour(string gameobjectName)
    {
        return GameObject.Find("Canvas").transform.Find(gameobjectName).gameObject.GetComponent<UIStateBehaviourBase>();
    }
}
