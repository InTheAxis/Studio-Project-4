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
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject newObj = PhotonNetwork.InstantiateSceneObject(prefab.name, transform.position, transform.rotation);
            newObj.transform.localScale = transform.localScale;
        }
        Destroy(this.gameObject);
    }
}
