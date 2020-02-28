using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Attachment", menuName = "Attachment")]
public class BuildingAttachment : ScriptableObject
{
    public List<GoDropListItem> meshList = new List<GoDropListItem>();

    private void OnValidate()
    {
        int totalWeight = 0;
        foreach (GoDropListItem meshData in meshList)
        {
            totalWeight += meshData.weight;
        }
        foreach (GoDropListItem meshData in meshList)
        {
            if (totalWeight == 0)
                meshData.chance = 0;
            else
                meshData.chance = (float)meshData.weight / totalWeight;
        }
    }
}