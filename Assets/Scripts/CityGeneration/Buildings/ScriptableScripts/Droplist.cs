using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class GoDropListItem
{
    //public Mesh mesh;
    public GameObject go;

    [Range(0, 100)]
    public int weight;

    public float chance;
}

[System.Serializable]
public class GoDropList
{
    public List<GoDropListItem> list = new List<GoDropListItem>();

    private void Validate()
    {
        int totalWeight = 0;
        foreach (GoDropListItem meshData in list)
        {
            totalWeight += meshData.weight;
        }
        foreach (GoDropListItem meshData in list)
        {
            if (totalWeight == 0)
                meshData.chance = 0;
            else
                meshData.chance = (float)meshData.weight / totalWeight;
        }
    }

    public GameObject SelectGO()
    {
        Validate();
        float val = Random.value;
        GoDropListItem selected = null;
        float incrementedVal = 0;
        // sort by chance
        list.Sort((p1, p2) => p1.chance.CompareTo(p2.chance));
        foreach (GoDropListItem item in list)
        {
            selected = item;
            incrementedVal += item.chance;
            if (incrementedVal > val)
                break;
        }
        return selected.go;
    }
}