using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Destructible : MonoBehaviourPun
{
    [Header("General")]
    [SerializeField]
    public List<GameObject> destroyed = null;

    [Header("Visuals")]
    [SerializeField]
    private GameObject hitParticle = null;
    [SerializeField]
    private GameObject dustParticles = null;
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

    // To prevent multiple Destruct() calls before PhotonNetwork comes around to removing this gameobject
    private bool isDestroyed = false;

    private void Start()
    {
        sqrBreakForce = breakForce * breakForce;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (!isDestroyed)
        {
            if (collision.relativeVelocity.magnitude >= sqrBreakForce)
            {
                isDestroyed = true;
                Destruct(collision);
            }
            if (collision.gameObject.layer == LayerMask.NameToLayer("Monster"))
            { 
                //TODO for elson :) , explode force based on velocity
                isDestroyed = true;
                Destruct(collision);
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {

    }


    private void Destruct(Collision collision)
    {
        /* Instantiate shattered clone */
        //GameObject target = destroyed;
        //GameObject clone = Instantiate(target, transform.position, transform.rotation);
        foreach (GameObject target in destroyed)
        {
            GameObject clone = PhotonNetwork.InstantiateSceneObject(target.name, transform.position + target.transform.position, transform.rotation);
            clone.transform.localRotation = transform.localRotation;
            clone.transform.localScale = transform.localScale;

            Rigidbody cloneRigidbody = clone.GetComponent<Rigidbody>();

            Vector3 velocity;
            velocity.x = Random.Range(minExplosionDir.x, maxExplosionDir.x);
            velocity.y = Random.Range(minExplosionDir.y, maxExplosionDir.y);
            velocity.z = Random.Range(minExplosionDir.z, maxExplosionDir.z);
            velocity.Normalize();
            velocity.x *= Random.Range(minExplosionForce.x, maxExplosionForce.x);
            velocity.y *= Random.Range(minExplosionForce.y, maxExplosionForce.y);
            velocity.z *= Random.Range(minExplosionForce.z, maxExplosionForce.z);
            cloneRigidbody.velocity = velocity;
        }

        /* Instantiate particle hit */
        bool shouldSpawnParticles = Random.Range(0.0f, 1.0f) <= hitParticleSpawnChance;
        Vector3 point = collision.GetContact(0).point;
        Vector3 normal = collision.GetContact(0).normal;
        photonView.RPC("breakApart", RpcTarget.Others, shouldSpawnParticles, point, normal);
        breakApart(shouldSpawnParticles, point, normal);
        //if (Random.Range(0.0f, 1.0f) <= hitParticleSpawnChance)
        //{
        //    if (hitParticle != null)
        //    {
        //        GameObject particle = Instantiate(hitParticle);
        //        particle.transform.position = collision.GetContact(0).point;
        //        particle.transform.forward = collision.GetContact(0).normal;
        //        particle.transform.localScale = transform.localScale;
        //        Destroy(particle, particle.GetComponent<ParticleSystem>().main.duration);
        //    }

        //    if (dustParticles != null)
        //    {
        //        GameObject particle = Instantiate(dustParticles);
        //        particle.transform.position = collision.GetContact(0).point;
        //        particle.transform.forward = collision.GetContact(0).normal;
        //        particle.transform.localScale = transform.localScale;
        //        Destroy(particle, particle.GetComponent<ParticleSystem>().main.duration);
        //    }
        //}

        //Destroy(gameObject);
        //NetworkOwnership.instance.destroy(PhotonView.Get(gameObject));
    }

    [PunRPC]
    private void breakApart(bool spawnParticles, Vector3 point, Vector3 normal)
    {
        if (spawnParticles)
        {
            if (hitParticle != null)
            {
                GameObject particle = Instantiate(hitParticle);
                particle.transform.position = point;
                particle.transform.forward = normal;
                particle.transform.localScale = transform.localScale;
                Destroy(particle, particle.GetComponent<ParticleSystem>().main.duration);
            }

            if (dustParticles != null)
            {
                GameObject particle = Instantiate(dustParticles);
                particle.transform.position = point;
                particle.transform.forward = normal;
                particle.transform.localScale = transform.localScale;
                Destroy(particle, particle.GetComponent<ParticleSystem>().main.duration);
            }
        }

        Destroy(this.gameObject);
    }

}
