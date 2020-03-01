using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New City", menuName = "City")]
public class CityScriptable : ScriptableObject
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

    public GameObject SelectMesh()
    {
        float val = Random.value;
        GoDropListItem selected = null;
        float incrementedVal = 0;
        // sort by chance
        meshList.Sort((p1, p2) => p1.chance.CompareTo(p2.chance));
        foreach (GoDropListItem meshData in meshList)
        {
            selected = meshData;
            incrementedVal += meshData.chance;
            if (incrementedVal > val)
                break;
        }
        return selected.go;
    }

    /// <summary>
    /// find building that fits the range most closely
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public GameObject SelectMesh(float range)
    {
        // float val = Random.value;
        GoDropListItem selected = null;
        float closestDiff = float.MaxValue;
        // sort by chance
        // meshList.Sort((p1, p2) => p1.chance.CompareTo(p2.chance));
        foreach (GoDropListItem meshData in meshList)
        {
            if (meshData.go == null)
                continue;
            float currentRange = meshData.go.GetComponent<ProceduralBuilding>().GetRadius();
            if (currentRange > range)
                continue;
            float currentDiff = range - currentRange;
            if (currentDiff < closestDiff)
                selected = meshData;
        }
        // select closest
        if (selected == null)
        {
            Debug.Log("cannot select building with range");
            return null;
        }
        return selected.go;
    }
}