using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FBXCustomImporter : AssetPostprocessor
{
    public static bool convertMayaScale = false;

    private void OnPostprocessModel(GameObject g)
    {
        if (!convertMayaScale) return;

        ModelImporter importer = assetImporter as ModelImporter;

        importer.globalScale = 100.0f;

        string fullPath = assetImporter.assetPath;
        int posFirstSeparator = fullPath.IndexOf("/");

        string relativeDir = fullPath.Substring(posFirstSeparator + 1, fullPath.Length - posFirstSeparator - 1);
        int posSecondSeparator = relativeDir.IndexOf("/");

        string categoryWithName = relativeDir.Substring(posSecondSeparator + 1, relativeDir.Length - posSecondSeparator - 1);
        int posExtension = categoryWithName.LastIndexOf(".");
        categoryWithName = categoryWithName.Substring(0, posExtension);

        importer.ExtractTextures("Assets/Textures/" + categoryWithName);
    }

   
}
