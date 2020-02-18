using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIStateBehaviourBase : MonoBehaviour
{
    public virtual void onEnter() { }
    public virtual void update() { }
    public virtual void onExit() { }

    protected void backToPrevState()
    {
        UIStateActiveManager.currentActiveManager.setToPrevState();
    }
}
