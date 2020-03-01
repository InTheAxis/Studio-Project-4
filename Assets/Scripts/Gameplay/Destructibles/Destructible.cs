using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

[RequireComponent(typeof(PhotonView))]
public class Destructible : MonoBehaviourPun
{
    [Header("General")]
    public List<GameObject> destroyed = null;

    [Header("Visuals")]
    public GameObject hitParticle = null;

    public GameObject dustParticles = null;

    [SerializeField]
    private float hitParticleSpawnChance = 0.65f;

    [Header("Explosion")]
    [SerializeField]
    [Tooltip("Force to Break - Negative for Infinity")]
    private float breakForce = 2.0f;

    //[SerializeField]
    //[Tooltip("The factor that determines how far the shard is relative to its parent when spawned, to prevent instant collisions ")]
    //private float safetyTolerance = 0.50f;

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
        if (breakForce < 0.0f)
            sqrBreakForce = -1.0f;
        else
            sqrBreakForce = breakForce * breakForce;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        // Negative break force is infinity. Never break.
        if (sqrBreakForce < 0.0f)
            return;

        if (!isDestroyed)
        {
            if (collision.relativeVelocity.magnitude >= sqrBreakForce)
            {
                //isDestroyed = true;
                Destruct(collision);
            }
            if (collision.gameObject.layer == LayerMask.NameToLayer("Monster"))
            {
                //TODO for elson :) , explode force based on velocity, maybe based on mass too
                //isDestroyed = true;
                Destruct(collision);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
    }

    #region Destruct

    public void Destruct(Collision collision)
    {
        Destruct(collision?.GetContact(0).point ?? transform.position, collision?.GetContact(0).normal ?? -Vector3.up);
    }

    public void Destruct(Vector3 collisionPoint, Vector3 collisionNormal)
    {
        if (isDestroyed)
            return;

        GetComponent<DestructibleAudioController>().DestructAudio();
        if (PhotonNetwork.IsMasterClient)
            rpcDestruct(collisionPoint, collisionNormal);
        else
        {
            // Destruct request is by client. Only send RPC once
            photonView.RPC("rpcDestruct", RpcTarget.MasterClient, collisionPoint, collisionNormal);
            isDestroyed = true;
        }
    }

    [PunRPC]
    private void rpcDestruct(Vector3 collisionPoint, Vector3 collisionNormal)
    {
        if (isDestroyed)
            return;
        isDestroyed = true;

        StartCoroutine(destructCourFunc(collisionPoint, collisionNormal));
        // Retrieve control in order to execute the destruct coroutine
        if (!(photonView.Owner == null || photonView.IsMine))
            photonView.RPC("releaseControlToMasterRequest", photonView.Owner);
    }

    [PunRPC]
    private void releaseControlToMasterRequest()
    {
        NetworkOwnership.instance.releaseOwnership(photonView, null, null);
    }

    // Should only be called on the master client
    private IEnumerator destructCourFunc(Vector3 collisionPoint, Vector3 collisionNormal)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("Destruct Coroutine called on non-master client! This should not be possible!");
            yield break;
        }

        // Start destruct only when we've got control
        while (!(photonView.Owner == null || photonView.IsMine))
            yield return new WaitForFixedUpdate();

        /* Instantiate shattered clone */
        //GameObject target = destroyed;
        //GameObject clone = Instantiate(target, transform.position, transform.rotation);

        if (destroyed.Count > 0)
        {
            foreach (GameObject target in destroyed)
            {
                if (target == null) continue;

                GameObject clone = PhotonNetwork.InstantiateSceneObject(target.name, transform.position + target.transform.position * 1.50f, transform.rotation);
                clone.transform.localRotation = transform.localRotation;
                clone.transform.localScale = transform.localScale * 0.95f;

                Rigidbody cloneRigidbody = clone.GetComponent<Rigidbody>();

                //Vector3 velocity;
                //velocity.x = Random.Range(minExplosionDir.x, maxExplosionDir.x);
                //velocity.y = Random.Range(minExplosionDir.y, maxExplosionDir.y);
                //velocity.z = Random.Range(minExplosionDir.z, maxExplosionDir.z);
                //velocity.Normalize();
                //velocity.x *= Random.Range(minExplosionForce.x, maxExplosionForce.x);
                //velocity.y *= Random.Range(minExplosionForce.y, maxExplosionForce.y);
                //velocity.z *= Random.Range(minExplosionForce.z, maxExplosionForce.z);
                cloneRigidbody.velocity = Vector3.zero;
            }
        }

        /* Instantiate particle hit */
        bool shouldSpawnParticles = Random.Range(0.0f, 1.0f) <= hitParticleSpawnChance;
        photonView.RPC("breakApart", RpcTarget.Others, shouldSpawnParticles, collisionPoint, collisionNormal);
        breakApart(shouldSpawnParticles, collisionPoint, collisionNormal);

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

    #endregion Destruct
}