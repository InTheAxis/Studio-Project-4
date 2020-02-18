using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ProceduralBuilding : MonoBehaviour
{
    //public values
    // public int seed = 0;

    // references
    public GameObject rootRef;

    public GameObject attachmentBaseRef;

    // owned by gameobject
    private GameObject attachmentRoot;

    public GameObject attachmentPositions;
    // private variables

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
        // seed = Random.seed;
        Clear();
        attachmentRoot = CityGenerator.instantiateAsChild(rootRef, transform) as GameObject;
        foreach (Transform slot in attachmentPositions.transform)
        {
            AttachmentSlot attachmentSlotScript = slot.GetComponent<AttachmentSlot>();
            if (Random.value <= attachmentSlotScript.chance)
            {
                GameObject meshRef = attachmentSlotScript.SelectMesh();
                GameObject attachment = CityGenerator.instantiateAsChild(meshRef, attachmentRoot.transform) as GameObject;
                attachment.transform.position = slot.position;
                attachment.transform.rotation = slot.rotation;
                attachment.transform.localScale = slot.localScale;
            }
        }
    }
}