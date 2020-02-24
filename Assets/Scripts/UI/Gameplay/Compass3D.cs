using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Compass3D : MonoBehaviour
{
    [SerializeField]
    private Transform dirFrame = null;

    [SerializeField]
    private Transform dottedFrame = null;

    private Transform playerTransform = null;

    private void Start()
    {
        
    }

    private void OnEnable()
    {

    }

    private void Update()
    {
        if (GameManager.playerObj != null)
        {
            Vector3 rotation = dirFrame.localRotation.eulerAngles;
            rotation.y = GameManager.playerObj.transform.rotation.eulerAngles.y;
            dirFrame.localRotation = Quaternion.Euler(rotation);
        }
    }

}
