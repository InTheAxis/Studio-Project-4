using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WorleyNoiseRender))]
public class WorleyEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        WorleyNoiseRender script = (WorleyNoiseRender)target;

        //GUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate"))
        {
            script.Generate();
        }
        // GUILayout.EndHorizontal();
    }
}

[CustomEditor(typeof(CloudScript))]
public class CloudScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        CloudScript script = (CloudScript)target;

        //GUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate"))
        {
            script.Generate();
        }
        // GUILayout.EndHorizontal();
    }
}