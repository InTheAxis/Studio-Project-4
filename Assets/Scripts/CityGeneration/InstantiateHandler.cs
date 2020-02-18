using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class InstantiateHandler : MonoBehaviour
{
    public bool serverInstantiate;
    public static bool sServerInstantiate;

    private void OnValidate()
    {
        sServerInstantiate = serverInstantiate;
    }

    public static GameObject mInstantiate(GameObject go)
    {
        GameObject instantiated = null;
        if (sServerInstantiate)
        {
            instantiated = PhotonNetwork.Instantiate(go.name, Vector3.zero, Quaternion.identity);
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
            instantiated = PhotonNetwork.Instantiate(go.name, pos, Quaternion.identity);
        }
        else
        {
            instantiated = Instantiate(go);
        }
        return instantiated;
    }

    public static GameObject mInstantiate(GameObject go, Transform parent)
    {
        GameObject instantiated = mInstantiate(go);
        instantiated.transform.parent = parent;
        return instantiated;
    }

    public static GameObject mInstantiate(GameObject go, Vector3 localPos, Transform parent)
    {
        GameObject instantiated = mInstantiate(go);
        instantiated.transform.parent = parent;
        instantiated.transform.position = localPos;
        return instantiated;
    }

    public static GameObject mInstantiate(GameObject go, Vector3 localPos, Quaternion localRot, Transform parent)
    {
        GameObject instantiated = mInstantiate(go);
        instantiated.transform.parent = parent;
        instantiated.transform.position = localPos;
        instantiated.transform.rotation = localRot;
        return instantiated;
    }
}