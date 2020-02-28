using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

public class MonsterDataBootstrap : MonoBehaviour
{
    private EntityManager em;
    private Entity monsterData;

    private void Start()
    {
        em = World.DefaultGameObjectInjectionWorld.EntityManager;
        EntityArchetype ea = em.CreateArchetype(
          //for transform
          typeof(Translation),
          typeof(MonsterData)
          );
        monsterData = em.CreateEntity(ea);
    }

    private void Update()
    {
        MonsterData e = new MonsterData();
        e.monsterPos = transform.position;
        em.SetComponentData(monsterData, e);
    }
}