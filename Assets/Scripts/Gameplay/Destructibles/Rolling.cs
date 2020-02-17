using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rolling : MonoBehaviour
{
    [SerializeField]
    private Vector3 initialVelocity = Vector3.zero;
    [SerializeField]
    private float delay = 2.0f;

    private void Start()
    {
        StartCoroutine(setVelocity());
    }

    private void Update()
    {

    }

    private IEnumerator setVelocity()
    {
        yield return new WaitForSeconds(delay);
        GetComponent<Rigidbody>().velocity = initialVelocity;
        yield return null;
    }
}
