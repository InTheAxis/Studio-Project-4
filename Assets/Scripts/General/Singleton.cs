using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Singleton<T> : MonoBehaviour
{
    public static T instance = default(T);

    private void Awake()
    {
        checkSingleton();
    }

    public virtual void checkSingleton()
    {
        if (instance == null)
            instance = GetComponent<T>();
        else
        {
            Destroy(gameObject);
            return;
        }
    }
}
public class DoNotDestroySingleton<T> : Singleton<T>
{
    public override void checkSingleton()
    {
        base.checkSingleton();

        DontDestroyOnLoad(gameObject);
    }
}

public class SingletonPun<T> : MonoBehaviourPun
{
    public static T instance = default(T);

    private void Awake()
    {
        checkSingleton();
    }

    public virtual void checkSingleton()
    {
        if (instance == null)
            instance = GetComponent<T>();
        else
        {
            Destroy(gameObject);
            return;
        }
    }
}