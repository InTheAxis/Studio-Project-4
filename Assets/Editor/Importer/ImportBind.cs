using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImportBind
{

    public ImportBind()
    {
        materials = new List<Material>();
        textureFilePath = "";
        bindTimer = 0.80f;
    }

    public float bindTimer;
    public List<Material> materials;
    public string textureFilePath;
    
}
