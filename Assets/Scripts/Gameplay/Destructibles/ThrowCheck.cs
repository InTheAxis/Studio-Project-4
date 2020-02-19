using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowCheck : MonoBehaviour
{

    private List<Collider> destructibles;

    private void Start()
    {
        destructibles = new List<Collider>();
    }

    private void Update()
    {
        Debug.Log(destructibles.Count);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Throwable"))
            destructibles.Add(collider);
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Throwable") && destructibles.Contains(collider))
            destructibles.Remove(collider);
    }



}
