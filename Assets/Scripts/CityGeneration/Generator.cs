using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Generator : MonoBehaviour
{
    protected float scale;

    [SerializeField]
    protected bool gizmosEnabled;

    public void SetScale(float f)
    {
        scale = f;
    }

    abstract public void Generate();

    abstract public void Clear();
}