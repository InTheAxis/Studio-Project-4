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
            Vector3 scale = transform.localScale;
            scale.Scale(newObj.transform.localScale);
            newObj.transform.localScale = scale;
        }
        Destroy(this.gameObject);
    }
}
