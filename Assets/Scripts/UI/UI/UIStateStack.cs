using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStateStack
{
    private List<string> uiPageStack = new List<string>();

    public string Root { get { return uiPageStack[0]; } }

    // Clear stack, then set root state
    public string setRoot(string state)
    {
        uiPageStack.Clear();
        uiPageStack.Add(state);
        return uiPageStack[0];
    }

    public string getTop()
    {
        return uiPageStack[uiPageStack.Count - 1];
    }

    public string Push(string state)
    {
        uiPageStack.Add(state);
        return getTop();
    }

    public string Pop()
    {
        uiPageStack.RemoveAt(uiPageStack.Count - 1);
        return getTop();
    }

    // Pop until state specified or root, whichever is encountered first
    public string jumpTo(string state)
    {
        while (getTop() != state && uiPageStack.Count > 1)
            Pop();
        return getTop();
    }

    public string jumpToRoot()
    {
        while (uiPageStack.Count > 1)
            Pop();
        return getTop();
    }

    // True if state exists in stack
    public bool hasState(string state, bool ignoreTop = false)
    {
        for (int i = 0; i < uiPageStack.Count && (!ignoreTop || i < uiPageStack.Count - 1); ++i)
            if (uiPageStack[i] == state)
                return true;
        return false;
    }

    public string jumpIfHasState(string state)
    {
        if (hasState(state))
            return jumpTo(state);
        return getTop();
    }
}
