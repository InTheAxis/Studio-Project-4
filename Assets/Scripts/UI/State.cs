using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State : MonoBehaviour
{
    public abstract string Name { get; }


    private void Awake()
    {
        StateController.Register(this);

    }

    private void OnDestroy()
    {
        StateController.Unregister(this);

    }

    public virtual void onShow()
    {
        gameObject.SetActive(true);
    }

    public virtual void onHide()
    {
        gameObject.SetActive(false);

    }
}
