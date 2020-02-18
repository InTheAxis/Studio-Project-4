using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FBXCustomImporter : AssetPostprocessor
{
    public static bool customImport = false;
    public static bool autoScale = false;

    private void OnPostprocessModel(GameObject g)
    {

        ModelImporter importer = assetImporter as ModelImporter;

        if(autoScale)
            importer.globalScale = 100.0f;


        if (!customImport) return;

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
