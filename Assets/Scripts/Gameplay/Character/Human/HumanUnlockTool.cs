using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanUnlockTool : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private List<MonoBehaviour> scripts;
    [Header("Type of Script At Index")]
    [SerializeField]
    private List<TYPE> types;


    public enum TYPE
    { 
        NONE = -1,
        PULL,
        PUSH,
        STOP,
        SLOW,
    };
    private Dictionary<TYPE, MonoBehaviour> tools;


    private void Start()
    {
        tools = new Dictionary<TYPE, MonoBehaviour>();
        int size = Mathf.Min(types.Count, scripts.Count);
        for (int i = 0; i < size; ++i)        
            tools.Add(types[i], scripts[i]);

        DisableAll();
    }
    public void Unlock(TYPE _type)
    {
        DisableAll();
        tools[_type].enabled = true;
        Debug.LogFormat("Unlocked Tool of {0}", _type.ToString());
    }

    public void DisableAll()
    {
        foreach (var kv in tools)
        {
            kv.Value.enabled = false;
        }
    }
}
