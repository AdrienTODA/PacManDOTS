using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

public class CollisionSystem : SystemBase
{
    private struct CollisionSystemJob : ICollisionEventsJob
    {
        public BufferFromEntity<CollisionBuffer> collisions;

        public void Execute(CollisionEvent collisionEvent)
        {
            if (collisions.HasComponent(collisionEvent.EntityA))
                collisions[collisionEvent.EntityA].Add(new CollisionBuffer() { entity = collisionEvent.EntityB });
            if (collisions.HasComponent(collisionEvent.EntityB))
                collisions[collisionEvent.EntityB].Add(new CollisionBuffer() { entity = collisionEvent.EntityA });
        }
    }

    private struct TriggerSystemJob : ITriggerEventsJob
    {
        public BufferFromEntity<TriggerBuffer> triggers;

        public void Execute(TriggerEvent triggerEvent)
        {
            if (triggers.HasComponent(triggerEvent.EntityA))
                triggers[triggerEvent.EntityA].Add(new TriggerBuffer() { entity = triggerEvent.EntityB });
            if (triggers.HasComponent(triggerEvent.EntityB))
                triggers[triggerEvent.EntityB].Add(new TriggerBuffer() { entity = triggerEvent.EntityA });
        }
    }

    protected override void OnUpdate()
    {
        var pw = World.GetOrCreateSystem<BuildPhysicsWorld>().PhysicsWorld;
        var sim = World.GetOrCreateSystem<StepPhysicsWorld>().Simulation;

        Entities
            .ForEach((DynamicBuffer<CollisionBuffer> collisions) =>
            {
                collisions.Clear();
            }).Run();

        var colJobHandle = new CollisionSystemJob()
        {
            collisions = GetBufferFromEntity<CollisionBuffer>()
        }
        .Schedule(sim, ref pw, Dependency);

        colJobHandle.Complete();

        Entities
            .ForEach((DynamicBuffer<TriggerBuffer> triggers) =>
            {
                triggers.Clear();
            }).Run();

        var trigJobHandle = new TriggerSystemJob()
        {
            triggers = GetBufferFromEntity<TriggerBuffer>()
        }
        .Schedule(sim, ref pw, Dependency);

        trigJobHandle.Complete();
    }
}
