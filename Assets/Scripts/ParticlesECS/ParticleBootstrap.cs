using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Rendering;
using Unity.Mathematics;

public class ParticleBootstrap : MonoBehaviour
{
    [SerializeField]
    protected int maxNumEnts;
    [SerializeField]
    protected ParticleEntityDataObj entityObj;
    [SerializeField]
    protected ParticleSystemDataObj sysObj;

    protected Entity emitter;
    protected EntityManager em;
    protected bool isEmitting;

    private void Awake()
    {
        em = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    protected void Init(EntityArchetype particle, ComponentType systemTag)
    {
        NativeArray<Entity> entArr = new NativeArray<Entity>(maxNumEnts, Allocator.Temp);
        em.CreateEntity(particle, entArr);

        foreach (Entity e in entArr)
        {
            em.SetSharedComponentData(e, new RenderMesh { mesh = entityObj.mesh, material = entityObj.material, });
            em.SetComponentData(e, ParticleEntityData.Create(entityObj));
        }

        entArr.Dispose();

        //emitter entity
        emitter = em.CreateEntity(typeof(ParticleSystemData), systemTag);
        em.SetComponentData(emitter, ParticleSystemData.Create(sysObj));
        if (sysObj.enabledOnAwake)
            Emit();
    }

    public void Emit()
    {
        em.AddComponent(emitter, typeof(ParticleEmitTag));
        isEmitting = true;
    }
    public void StopEmit()
    {
        em.RemoveComponent(emitter, typeof(ParticleEmitTag));
        isEmitting = false;
    }

}
