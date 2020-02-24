using UnityEngine;

[CreateAssetMenu]
public class ParticleEntityDataObj : ScriptableObject
{
    [Header("Rendering")]
    public Mesh mesh;
    public Material material;

    [Header("Rotation")]
    public Vector3 startRot;
    public Vector3 rotRandom;
    [Header("Rotation Over Time")]
    public bool rotOverTime;
    public Vector3 endRot;

    [Header("Scale")]
    public float startScale;
    public float scaleRandom;
    [Header("Scale Over Time")]
    public bool scaleOverTime;
    public float endScale;

    [Header("Velocity")]
    public float startSpeed;
    public Vector3 velRandom;
    [Header("Velocity Over Time")]
    public bool velOverTime;
    public Vector3 endVel;

    [Header("Gravity")]
    public Vector3 startGravity;
    public Vector3 gravityRandom;
    public bool gravTowardsSource;
    [Header("Gravity Over Time")]
    public bool gravityOverTime;
    public Vector3 endGravity;

    [Header("Lifetime")]
    public float lifeTime;
    public float lifeTimeRandom;
};