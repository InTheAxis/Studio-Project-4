using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorleyNoiseRender : MonoBehaviour
{
    public SpriteRenderer sprite;
    public int resolution;
    public bool invert = false;
    public float scale = 1;
    public Vector3 offset;

    private WorleyNoise worley = new WorleyNoise();

    public Texture3D shapeTexture;

    [Range(0, 1)]
    public float layer = 0;

    public int numGrid = 5;

    private void Start()
    {
        Generate();
    }

    public void Generate()
    {
        worley.Generate(numGrid, scale);
        shapeTexture = worley.GenerateTexture3D(resolution, layer, invert);
        if (sprite)
            sprite.sprite = Sprite.Create(worley.GenerateTexture(resolution, layer, invert), new Rect(0, 0, resolution, resolution), Vector2.one * 0.5f);
    }

    private void OnValidate()
    {
        worley.SetOffset(offset);
        shapeTexture = worley.GenerateTexture3D(resolution, layer, invert);
        if (sprite)
            sprite.sprite = Sprite.Create(worley.GenerateTexture(resolution, layer, invert), new Rect(0, 0, resolution, resolution), Vector2.one * 0.5f);
    }

    private void OnDrawGizmos()
    {
        //foreach (Vector3 point in worley.GetPoints())
        //{
        //    Gizmos.DrawSphere(point - Vector3.one / 2, 0.1f);
        //}
    }
}