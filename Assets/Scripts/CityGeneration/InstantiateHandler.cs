using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class InstantiateHandler : MonoBehaviour
{
    public bool serverInstantiate = true;
    public static bool sServerInstantiate = true;

    private void OnValidate()
    {
        sServerInstantiate = serverInstantiate;
        Debug.Log("Server: " + sServerInstantiate);
    }

    private void Awake()
    {
        sServerInstantiate = serverInstantiate;
    }

    public static GameObject mInstantiate(GameObject go)
    {
        GameObject instantiated = null;
        if (sServerInstantiate)
        {
            instantiated = PhotonNetwork.InstantiateSceneObject(go.name, Vector3.zero, Quaternion.identity);
        }
        else
        {
            instantiated = Instantiate(go);
        }
        return instantiated;
    }

    public static GameObject mInstantiate(GameObject go, Vector3 pos)
    {
        GameObject instantiated = null;
        if (sServerInstantiate)
        {
            instantiated = PhotonNetwork.InstantiateSceneObject(go.name, pos, Quaternion.identity);
        }
        else
        {
            instantiated = Instantiate(go, pos, Quaternion.identity);
        }
        return instantiated;
    }

    public static GameObject mInstantiate(GameObject go, Vector3 pos, Quaternion rot)
    {
        GameObject instantiated = null;
        if (sServerInstantiate)
        {
            instantiated = PhotonNetwork.InstantiateSceneObject(go.name, pos, rot);
        }
        else
        {
            instantiated = Instantiate(go, pos, rot);
        }
        return instantiated;
    }

    public static GameObject mInstantiate(GameObject go, Transform parent)
    {
        GameObject instantiated = mInstantiate(go);
        instantiated.transform.parent = parent;
        return instantiated;
    }

    public static GameObject mInstantiate(GameObject go, Transform parent, string layer)
    {
        GameObject instantiated = mInstantiate(go);
        instantiated.transform.parent = parent;
        instantiated.layer = LayerMask.NameToLayer(layer);
        return instantiated;
    }

    public static GameObject mInstantiate(GameObject go, Vector3 localPos, Transform parent)
    {
        GameObject instantiated = mInstantiate(go);
        instantiated.transform.parent = parent;
        instantiated.transform.position = localPos;
        return instantiated;
    }

    public static GameObject mInstantiate(GameObject go, Vector3 pos, Quaternion rot, Transform parent)
    {
        GameObject instantiated = mInstantiate(go, pos, rot);
        instantiated.transform.parent = parent;
        return instantiated;
    }

    public static GameObject mInstantiate(GameObject go, Vector3 pos, Quaternion rot, Transform parent, string layer)
    {
        GameObject instantiated = mInstantiate(go, pos, rot);
        instantiated.transform.parent = parent;
        instantiated.layer = LayerMask.NameToLayer(layer);
        return instantiated;
    }
}