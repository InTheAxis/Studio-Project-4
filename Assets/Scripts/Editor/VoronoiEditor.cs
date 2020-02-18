using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoadGenerator))]
public class VoronoiEditor : Editor
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
        GUILayout.EndHorizontal();
    }
}