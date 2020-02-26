using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudScript : MonoBehaviour
{
    //public Shader shader;
    public Material material;

    //public WorleyNoiseRender worleyNoiseRender;
    public Transform container;

    //
    public SpriteRenderer sprite;

    //private bool invert = true;

    private WorleyNoise worleyA = new WorleyNoise();
    private WorleyNoise worleyB = new WorleyNoise();

    private Texture3D shapeTexture;
    private Texture3D detailTexture;

    [Range(0, 1)]
    public float layer = 0;

    //
    //private Texture3D texture;

    //
    [Min(0)]
    public int numSteps;

    //[Range(0, 5)]
    public int resolutionA;

    public float generateScaleA = 1;

    public int numGridA = 5;

    [Min(0)]
    public float densityThresholdA;

    public float cloudScaleA;

    [Min(0)]
    public float densityMultiplierA;

    public Vector3 offsetA;
    //----------------------------

    //[Range(0, 5)]
    public int resolutionB;

    public float generateScaleB = 1;

    public int numGridB = 5;

    [Min(0)]
    public float densityThresholdB;

    public float cloudScaleB;

    [Min(0)]
    public float densityMultiplierB;

    public Vector3 offsetB;

    private void Start()
    {
        Generate();
    }

    public void Generate()
    {
        worleyA.Generate(numGridA, generateScaleA);
        shapeTexture = worleyA.GenerateTexture3D(resolutionA, layer, true);
        worleyB.Generate(numGridB, generateScaleB);
        detailTexture = worleyB.GenerateTexture3D(resolutionB, layer, true);
        SetValues();
    }

    private void Update()
    {
        SetValues();
    }

    private void OnValidate()
    {
        SetValues();
        if (sprite)
            sprite.sprite = Sprite.Create(worleyA.GenerateTexture(resolutionA, layer, true), new Rect(0, 0, resolutionA, resolutionA), Vector2.one * 0.5f);
    }

    private void SetValues()
    {
        // Debug.Log("Frame");
        //if (material == null)
        //    material = new Material(shader);
        // Set container bounds:
        material.SetVector("BoundsMin", container.position - container.lossyScale / 2);
        material.SetVector("BoundsMax", container.position + container.lossyScale / 2);
        //
        material.SetVector("CloudOffset", offsetA);
        material.SetVector("CloudOffsetB", offsetB);
        material.SetFloat("CloudScale", cloudScaleA);
        material.SetFloat("CloudScaleB", cloudScaleB);
        material.SetFloat("DensityThreshold", densityThresholdA);
        material.SetFloat("DensityThresholdB", densityThresholdB);
        material.SetFloat("DensityMultiplier", densityMultiplierA);
        material.SetFloat("DensityMultiplierB", densityMultiplierB);
        material.SetInt("NumSteps", numSteps);
        // noise
        material.SetTexture("ShapeNoise", shapeTexture);
        material.SetTexture("DetailNoise", detailTexture);
    }

    //[ExecuteAlways, ImageEffectAllowedInSceneView]
    //private void OnRenderImage(RenderTexture source, RenderTexture destination)
    //{
    //    Debug.Log("Frame");
    //    if (material == null)
    //        material = new Material(shader);
    //    // Set container bounds:
    //    material.SetVector("BoundsMin", container.position - container.localScale / 2);
    //    material.SetVector("BoundsMax", container.position + container.localScale / 2);
    //    //
    //    material.SetVector("CloudOffset", offsetA);
    //    material.SetVector("CloudOffset", offsetB);
    //    material.SetFloat("CloudScale", cloudScaleA);
    //    material.SetFloat("CloudScaleB", cloudScaleB);
    //    material.SetFloat("DensityThreshold", densityThresholdA);
    //    material.SetFloat("DensityThresholdB", densityThresholdB);
    //    material.SetFloat("DensityMultiplier", densityMultiplierA);
    //    material.SetFloat("DensityMultiplierB", densityMultiplierB);
    //    material.SetInt("NumSteps", numSteps);
    //    // noise
    //    material.SetTexture("ShapeNoise", shapeTexture);
    //    material.SetTexture("DetailNoise", detailTexture);

    //    Graphics.Blit(source, destination, material);
    //}
}