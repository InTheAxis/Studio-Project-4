using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ProceduralBuilding))]
public class ProceduralBuildingEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ProceduralBuilding building = (ProceduralBuilding)target;

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate Random"))
        {
            building.GenerateRandom();
        }
        //if (GUILayout.Button("Generate Seeded"))
        //{
        //    building.GenerateSeeded();
        //}
        GUILayout.EndHorizontal();
    }
}