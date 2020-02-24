using UnityEngine;

[CreateAssetMenu]
public class ParticleEntityDataObj : ScriptableObject
{
    public Mesh mesh;
    public Material material;

    [Header("Position")]
    public Vector3 emitSource;
    public Vector3 emitRandom;

    [Header("Rotation")]
    public Vector3 rotRandom;
    public Vector3 startRot;
    public bool rotOverTime;
    public Vector3 endRot;

    [Header("Scale")]
    public float scaleRandom;
    public float startScale;
    public bool scaleOverTime;
    public float endScale;

    [Header("Velocity")]
    public Vector3 velRandom;
    public float startSpeed;
    public bool velOverTime;
    public Vector3 endVel;

    [Header("Gravity")]
    public Vector3 gravityRandom;
    public Vector3 startGravity;
    public bool gravityOverTime;
    public Vector3 endGravity;

    [Header("Lifetime")]
    public float lifeTimeRandom;
    public float lifeTime;
};