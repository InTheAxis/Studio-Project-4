using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ConvertToPhotonSceneObject : MonoBehaviour
{
    [SerializeField]
    private GameObject prefab;

    private void Awake()
    {
        PhotonNetwork.InstantiateSceneObject(prefab.name, transform.position, transform.rotation);
        Destroy(this.gameObject);
    }
}
