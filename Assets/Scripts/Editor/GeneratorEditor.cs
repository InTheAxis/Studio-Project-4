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

        Generator script = (Generator)target;

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate"))
        {
            script.Generate();
        }
        if (GUILayout.Button("Clear"))
        {
            script.Clear();
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

        Generator script = (Generator)target;

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate"))
        {
            script.Generate();
        }
        if (GUILayout.Button("Clear"))
        {
            script.Clear();
        }
        GUILayout.EndHorizontal();
    }
}

[CustomEditor(typeof(TowerGenerator))]
public class TowerGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Generator script = (Generator)target;

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate"))
        {
            script.Generate();
        }
        if (GUILayout.Button("Clear"))
        {
            script.Clear();
        }
        GUILayout.EndHorizontal();
    }
}