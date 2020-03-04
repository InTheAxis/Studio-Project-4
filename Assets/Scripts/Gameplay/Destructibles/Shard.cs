using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Shard : MonoBehaviour
{
    private Rigidbody[] rigidbodies = null;

    [SerializeField]
    private float chanceToDespawn = 0.30f;
    [SerializeField]
    private float timeBeforeDespawn = 20.0f;
    [SerializeField]
    private float smallShardScaleThreshold = 1.0f;
    [SerializeField]
    private float shardSettleVelocityThreshold = 0.1f;
    private bool shouldDespawn = false;




    private List<Rigidbody> smallShards = new List<Rigidbody>();
    private List<Rigidbody> sleepShards = new List<Rigidbody>();

    private PhotonView thisView;

    private void Start()
    {
        thisView = PhotonView.Get(this);
        if (!PhotonNetwork.IsMasterClient)
            return;


        if (Random.Range(0.0f, 1.0f) <= chanceToDespawn)
        {
            //Debug.Log("Destroyed");

            if (GetComponent<NetworkDestroyDelay>() == null)
            {
                NetworkDestroyDelay comp = this.gameObject.AddComponent<NetworkDestroyDelay>();
                comp.delay = timeBeforeDespawn;
            }

        }


        if(GetComponents<PhotonView>().Length > 1)
        {
            Debug.LogError("Multiple Photon Views: " + gameObject.name);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.sqrMagnitude < 4.0f) return;

        if(HitEffects.instance == null)
            Debug.LogError("HitEffects does not exist!");

        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Monster"))
        {
            GameObject clone = Instantiate(HitEffects.instance.monsterCollide);
            clone.transform.position = collision.contacts[0].point;
            clone.transform.forward = collision.contacts[0].normal;
            Destroy(clone, clone.GetComponent<ParticleSystem>().main.duration);
        }
        else if(collision.collider.gameObject.layer == LayerMask.NameToLayer("Human"))
        { 
            GameObject clone = Instantiate(HitEffects.instance.survivorCollide);
            clone.transform.position = collision.contacts[0].point;
            clone.transform.forward = collision.contacts[0].normal;
            Destroy(clone, clone.GetComponent<ParticleSystem>().main.duration);
        }
    }
}
