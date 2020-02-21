using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour
{
    public static Singleton<T> instance = null;

    private void Awake()
    {
        checkSingleton();
    }


    public virtual void checkSingleton()
    {
        if (instance == null)
        {
            instance = this;
        }
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