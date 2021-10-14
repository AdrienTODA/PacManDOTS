using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

class CollectionSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>().CreateCommandBuffer();

        Entities
            .WithAll<PlayerData>()
            .ForEach((Entity playerEntity, DynamicBuffer<TriggerBuffer> triggerBuffer) =>
            {
                for (int i = 0; i < triggerBuffer.Length; i++)
                {
                    var e = triggerBuffer[i].entity;
                    if (HasComponent<CollectableData>(e) && !HasComponent<KillData>(e))
                    {
                        ecb.AddComponent(e, new KillData() { killTimer = 0 });
                        GameManager.Instance.AddPoints(GetComponent<CollectableData>(e).points);
                    }
                    if (HasComponent<PowerPillData>(e) && !HasComponent<KillData>(e))
                    {
                        ecb.AddComponent(playerEntity, new PowerPillData() { pillTimer = 10 });
                        ecb.AddComponent(e, new KillData() { killTimer = 0 });
                    }
                }
            }).WithoutBurst().Run();
    }
}