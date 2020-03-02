using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class HumanUnlockTool : MonoBehaviourPun
{
    [Header("References")]
    [SerializeField]
    private List<MonoBehaviour> scripts;
    [Header("Type of Script At Index")]
    [SerializeField]
    private List<TYPE> types;

    public delegate void AbilityGainCallback(TYPE type);

    public AbilityGainCallback abilityGainCallback;

    public enum TYPE
    { 
        NONE = -1,
        PULL,
        PUSH,
        STOP,
        SLOW,
        RANDOM,
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
        if (_type == TYPE.RANDOM)
            _type = (TYPE)Random.Range((int)TYPE.PULL, (int)TYPE.RANDOM);
        DisableAll();
        tools[_type].enabled = true;
        Debug.LogFormat("Unlocked Tool of {0}", _type.ToString());

        photonView.RPC("abilityUnlockedRpc", RpcTarget.Others, (int)_type);
        abilityGainCallback?.Invoke(_type);
    }

    [PunRPC]
    private void abilityUnlockedRpc(int type)
    {
        TYPE abilityType = (TYPE)type;
        abilityGainCallback?.Invoke(abilityType);
    }

    public void DisableAll()
    {
        foreach (var kv in tools)
        {
            kv.Value.enabled = false;
        }
    }
}
