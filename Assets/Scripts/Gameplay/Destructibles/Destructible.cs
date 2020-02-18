using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{

    [Header("General")]
    public GameObject destroyed = null;
    [SerializeField]
    private GameObject hitParticle = null;
    [SerializeField]
    private float hitParticleSpawnChance = 0.65f;

    [Header("Explosion")]
    [SerializeField]
    private float breakForce = 2.0f;
    [SerializeField]
    private Vector3 minExplosionDir = new Vector3(-1.0f, -1.0f, -1.0f);
    [SerializeField]
    private Vector3 maxExplosionDir = new Vector3(1.0f, 1.0f, 1.0f);
    [SerializeField]
    private Vector3 minExplosionForce = new Vector3(1.0f, 1.0f, 1.0f);
    [SerializeField]
    private Vector3 maxExplosionForce = new Vector3(2.0f, 2.0f, 2.0f);

    private float sqrBreakForce = 0.0f;

    private void Start()
    {
        sqrBreakForce = breakForce * breakForce;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude >= sqrBreakForce)
            Destruct(collision);

    }

    private void OnTriggerEnter(Collider other)
    {

    }


    private void Destruct(Collision collision)
    {

        /* Instantiate shattered clone */
        GameObject target = destroyed;
        GameObject clone = Instantiate(target, transform.position, transform.rotation);
        clone.transform.localRotation = transform.localRotation;
        clone.transform.localScale = transform.localScale;

        /* Instantiate particle hit */
        if(hitParticle != null && Random.Range(0.0f, 1.0f) <= hitParticleSpawnChance)
        {
            GameObject particle = Instantiate(hitParticle);
            particle.transform.position = collision.GetContact(0).point;
            particle.transform.forward = collision.GetContact(0).normal;
            Destroy(particle, particle.GetComponent<ParticleSystem>().main.duration);
        }

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
