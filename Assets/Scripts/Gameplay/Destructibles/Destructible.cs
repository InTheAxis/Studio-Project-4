using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{

    [SerializeField]
    private GameObject[] destroyed = null;
    [SerializeField]
    private float breakForce = 5.0f;
    [SerializeField]
    private Vector3 minExplosionDir = Vector3.zero;
    [SerializeField]
    private Vector3 maxExplosionDir = Vector3.zero;
    [SerializeField]
    private Vector3 minExplosionForce = Vector3.zero;
    [SerializeField]
    private Vector3 maxExplosionForce = Vector3.zero;

    private float sqrBreakForce = 0.0f;

    private void Start()
    {
        sqrBreakForce = breakForce * breakForce;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude >= sqrBreakForce)
            Destruct();

    }

    private void OnTriggerEnter(Collider other)
    {

    }

    private void OnMouseDown()
    {
        Destruct();
    }

    private void Destruct()
    {
        GameObject target = destroyed[Random.Range(0, destroyed.Length)];
        GameObject clone = Instantiate(target, transform.position, transform.rotation);
        clone.transform.localRotation = transform.localRotation;
        clone.transform.localScale = transform.localScale;

        Rigidbody[] rigidbodies = clone.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rigidbodies)
        {
            Vector3 velocity;
            velocity.x = Random.Range(minExplosionDir.x, maxExplosionDir.x);
            velocity.y = Random.Range(minExplosionDir.y, maxExplosionDir.y);
            velocity.z = Random.Range(minExplosionDir.z, maxExplosionDir.z);
            velocity.Normalize();
            velocity.x *= Random.Range(minExplosionForce.x, maxExplosionForce.x);
            velocity.y *= Random.Range(minExplosionForce.y, maxExplosionForce.y);
            velocity.z *= Random.Range(minExplosionForce.z, maxExplosionForce.z);
            rb.velocity = velocity;
        }

        Destroy(gameObject);
    }

}
