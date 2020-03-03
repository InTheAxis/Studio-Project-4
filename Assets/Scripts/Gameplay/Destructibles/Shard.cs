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

    [Header("VFX")]
    [SerializeField]
    private GameObject monsterHit = null;
    [SerializeField]
    private GameObject survivorHit = null;


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
        if (collision.relativeVelocity.sqrMagnitude < 10.0f) return;

        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Monster"))
        {
            if (monsterHit == null)
            {
                Debug.LogWarning("Add monsterHit particle to this shard: " + gameObject.name);
                return;
            }

            GameObject clone = Instantiate(monsterHit);
            clone.transform.position = collision.contacts[0].point;
            clone.transform.forward = collision.contacts[0].normal;
            Destroy(clone, clone.GetComponent<ParticleSystem>().main.duration);
        }
        else if(collision.collider.gameObject.layer == LayerMask.NameToLayer("Human"))
        {
            if (survivorHit == null)
            {
                Debug.LogWarning("Add survivorHit particle to this shard: " + gameObject.name);
                return;
            }

            GameObject clone = Instantiate(survivorHit);
            clone.transform.position = collision.contacts[0].point;
            clone.transform.forward = collision.contacts[0].normal;
            Destroy(clone, clone.GetComponent<ParticleSystem>().main.duration);
        }
    }
}
