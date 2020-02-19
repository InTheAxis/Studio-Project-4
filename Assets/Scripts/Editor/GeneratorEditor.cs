using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BuildingGenerator))]
public class BuildingGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        BuildingGenerator script = (BuildingGenerator)target;

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate"))
        {
            script.Generate();
        }
        if (GUILayout.Button("Clear"))
        {
            script.Generate();
        }
        GUILayout.EndHorizontal();
    }
}

[CustomEditor(typeof(RoadGenerator))]
public class RoadGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        RoadGenerator script = (RoadGenerator)target;

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate"))
        {
            script.Generate();
        }
        if (GUILayout.Button("Clear"))
        {
            script.Generate();
        }
        GUILayout.EndHorizontal();
    }
}