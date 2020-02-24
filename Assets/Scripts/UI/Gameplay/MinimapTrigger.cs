using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MinimapTrigger : MonoBehaviour
{
    [SerializeField]
    private LayerMask buildingMask = 0;

    /* Event Callbacks for Minimap Trigger */
    public event Action<GameObject> showHologram;
    public event Action<GameObject> hideHologram;

    private void Awake()
    {
        //buildingMaskLayer = LayerMask.NameToLayer(maskName);
    }

    public void setBounds(float size)
    {
        GetComponent<SphereCollider>().radius = size;
    }

    /* TODO: Ignore all ground */
    private void OnTriggerEnter(Collider collider)
    {
        if (LayerMaskExt.CheckIfIgnored(buildingMask, collider.gameObject.layer) && collider.gameObject.name != "Ground")
            showHologram?.Invoke(collider.gameObject);
    }

    private void OnTriggerExit(Collider collider)
    {
        if (LayerMaskExt.CheckIfIgnored(buildingMask, collider.gameObject.layer) && collider.gameObject.name != "Ground")
            hideHologram?.Invoke(collider.gameObject);
    }
}
