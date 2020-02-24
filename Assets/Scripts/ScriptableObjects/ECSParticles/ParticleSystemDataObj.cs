using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ParticleSystemDataObj : ScriptableObject
{
    public enum EMIT_DIR_TYPE
    {
        DEFAULT = 0,
        RADIAL,
    }

    [Header("System Settings")]

    [Tooltip("Start emitting on entity creation?")]
    public bool enabledOnAwake;
    [Tooltip("Max number of particles that can exist")]
    public int maxNumParticles;
    [Tooltip("The amount of entities per job")]
    public int numJobBatch;

    [Header("Emission Settings")]
    [Tooltip("How much to randomise emission pos")]
    public Vector3 emitPosRandom;
    [Tooltip("Shape of emission")]
    public EMIT_DIR_TYPE emitDirType;

    [Tooltip("Number of particles to spawn per tick")]
    public int numPerUpdate;
    [Tooltip("How many seconds before next tick")]
    public float rate;

    [Tooltip("Loop infinitely")]
    public bool loop;
    [Tooltip("How long one loop cycle takes")]
    public float duration;
    [Tooltip("How long before emission starts")]
    public float delay;
};