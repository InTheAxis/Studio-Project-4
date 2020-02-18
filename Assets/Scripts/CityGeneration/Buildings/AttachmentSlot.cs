using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachmentSlot : MonoBehaviour
{
    public BuildingAttachment buildingAttachment;

    [Range(0, 1)]
    public float chance = 1.0f;

    public GameObject SelectMesh()
    {
        float val = Random.value;
        MeshData selected = null;
        float incrementedVal = 0;
        // sort by chance
        buildingAttachment.meshList.Sort((p1, p2) => p1.chance.CompareTo(p2.chance));
        foreach (MeshData meshData in buildingAttachment.meshList)
        {
            selected = meshData;
            incrementedVal += meshData.chance;
            if (incrementedVal > val)
                break;
        }
        return selected.mesh;
    }
}