using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Photon.Pun;

public class ProceduralBuilding : MonoBehaviour
{
    //public values
    // public int seed = 0;
    [SerializeField]
    public bool unbreakableAttachments = false;

    [SerializeField]
    private float buildingRadius = 10;

    [SerializeField]
    private GameObject attachGo;

    // references
    public GameObject rootRef;

    public bool GenerateOnStart = false;
    public GameObject attachmentBaseRef;

    // owned by gameobject
    private GameObject attachmentRoot;

    public GameObject attachmentPositions;

    [SerializeField]
    private bool gizmosEnabled = false;

    [SerializeField]
    private float breakForce = 10;

    // private variables
    public float GetRadius()
    {
        return buildingRadius;
    }

    private void OnDrawGizmos()
    {
        if (!gizmosEnabled)
            return;
        Gizmos.DrawWireSphere(Vector3.zero, buildingRadius);
    }

    private void OnEnable()
    {
        //#if true
        //        instantiateFunction = Instantiate;
        //        instantiateAsChildFunction = Instantiate;
        //#else
        //        instantiateFunction = PrefabUtility.InstantiatePrefab;
        //        instantiateAsChildFunction = PrefabUtility.InstantiatePrefab;
        //#endif
    }

    private void Start()
    {
        gizmosEnabled = false;
        if (GenerateOnStart)
            Generate();
    }

    // Remove all current attachments
    private void Clear()
    {
        if (attachmentRoot)
        {
            DestroyImmediate(attachmentRoot);
            attachmentRoot = null;
        }
    }

    //public void GenerateSeeded()
    //{
    //    Random.InitState(seed); // should be move to an overall world generator instead of per building
    //    Generate();
    //}

    public void GenerateRandom()
    {
        // Random.InitState(Random.Range(0, int.MaxValue));
        Generate();
    }

    private void Generate()
    {
        Rigidbody rigid = GetComponent<Rigidbody>();
        // seed = Random.seed;
        Clear();
        attachmentRoot = InstantiateHandler.mInstantiate(rootRef, transform);

        foreach (Transform slot in attachmentPositions.transform)
        {
            if (!slot.gameObject.activeSelf)
                continue;
            AttachmentSlot attachmentSlotScript = slot.GetComponent<AttachmentSlot>();
            if (Random.value <= attachmentSlotScript.chance)
            {
                GameObject meshRef = attachmentSlotScript.SelectMesh();
                //if (!meshRef.GetComponent<PhotonView>())
                //    meshRef.AddComponent<PhotonView>();
                if (!meshRef)
                {
                    Debug.LogError("mesh is null on: " + gameObject.name);
                    continue;
                }
                GameObject attachment = InstantiateHandler.mInstantiate(meshRef, slot.transform.position, Quaternion.identity, attachmentRoot.transform);
                attachment.transform.rotation = slot.rotation;
                attachment.transform.localScale = slot.localScale;
                if (attachGo && unbreakableAttachments)
                {
                    FixedJoint joint = attachment.GetComponent<FixedJoint>();
                    if (joint)
                    {
                        joint.connectedBody = attachGo.GetComponent<Rigidbody>();

                        joint.breakForce = breakForce;
                        joint.breakTorque = breakForce;
                    }
                    else
                    {
                        Debug.LogError("no joint attached on" + attachment.name);
                    }
                }

                if (attachment.GetComponent<ProceduralBuilding>())
                    attachment.GetComponent<ProceduralBuilding>().Generate();
            }
        }
    }
}