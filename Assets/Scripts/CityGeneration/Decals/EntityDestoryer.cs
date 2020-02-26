using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class EntityDestoryer : MonoBehaviour
{
    private BoidSpawner bSpawner;
    private BoidManager boid;
    private PropGenerator prop;

    private void Start()
    {
        boid = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<BoidManager>();
        prop = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<PropGenerator>();
        bSpawner = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<BoidSpawner>();
    }

    private void OnDestroy()
    {
        boid.CleanUp();
        prop.CleanUp();
        bSpawner.Reset();
    }
}