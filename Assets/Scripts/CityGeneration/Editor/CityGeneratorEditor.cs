using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CityGenerator))]
public class CityGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        CityGenerator script = (CityGenerator)target;

        //GUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate"))
        {
            script.Generate();
        }
        if (GUILayout.Button("Clear"))
        {
            script.Clear();
        }
        // GUILayout.EndHorizontal();
    }
}