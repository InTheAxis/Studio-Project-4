using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharMinimapCamera : MonoBehaviour
{
    public static CharMinimapCamera Instance;

    [Header("General")]
    [SerializeField]
    private GameObject minimap = null;
    [SerializeField]
    private float minimapY = 50.0f;

    [SerializeField]
    private CharTPController charControl = null;

    public Action<bool> eventShowMinimap;
    private Minimap3D minimap3D = null;

    private void Awake()
    {
        Instance = this;
    }

    private void LateUpdate()
    {
        if (charControl != null)
        {
            Vector3 position = charControl.transform.position;
            position.y = minimapY;
            transform.position = position;

            /* Rotates Minimap Camera */
            Vector3 rotation = charControl.transform.rotation.eulerAngles;
            rotation.x = 90.0f;
            //rotation.y = transform.rotation.eulerAngles.y;
            transform.rotation = Quaternion.Euler(rotation);

            /* Rotates Minimap */
            Vector3 hologramRot = minimap3D.transform.localRotation.eulerAngles;
            hologramRot.y = charControl.transform.rotation.eulerAngles.y;
            minimap3D.transform.localRotation = Quaternion.Euler(hologramRot);
        }
    }

    public void SetCharController(CharTPController cc)
    {
        charControl = cc;
        minimap = charControl.transform.Find("Minimap").gameObject;
        minimap3D = minimap.transform.Find("Minimap 3D").GetComponent<Minimap3D>();

    }

 
    public void toggleMap(bool status)
    {
        minimap.SetActive(status);
        minimap3D.Show();
        eventShowMinimap?.Invoke(status);
    }


    //private void OnTriggerEnter(Collider collider)
    //{
    //    if (collider.gameObject.layer == buildingMaskLayer)
    //    {
    //        Debug.Log("Show building: " + collider.gameObject.name);
    //    }
    //}

    //private void OnTriggerExit(Collider collider)
    //{
    //    if (collider.gameObject.layer == buildingMaskLayer)
    //    {
    //        Debug.Log("Hide building: " + collider.gameObject.name);
    //    }
    //}
}
