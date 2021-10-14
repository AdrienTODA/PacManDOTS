using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class DamageSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var dt = Time.DeltaTime;

        Entities
            .ForEach((DynamicBuffer<CollisionBuffer> collisionBuffer, ref HealthData hp) =>
            {
                for (int i = 0; i < collisionBuffer.Length; i++)
                {
                    if (hp.invincibleTimer <= 0 && HasComponent<DamageData>(collisionBuffer[i].entity))
                    {
                        hp.health -= GetComponent<DamageData>(collisionBuffer[i].entity).damage;
                        hp.invincibleTimer = 1;
                        AudioManager.Instance.PlaySfxRequest(hp.damageSfx.ToString());
                    }
                }
            }).WithoutBurst().Run();

        Entities
            .WithNone<KillData>()
            .ForEach((Entity e, ref HealthData hp) =>
            {
                hp.invincibleTimer -= dt;
                if (hp.health <= 0)
                {
                    AudioManager.Instance.PlaySfxRequest(hp.deathSfx.ToString());
                    EntityManager.AddComponentData(e, new KillData() { killTimer = hp.killTimer });
                }
            }).WithStructuralChanges().Run();

        var ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        var ecb = ecbSystem.CreateCommandBuffer();

        Entities
            .ForEach((Entity e, ref KillData killData, in Translation trs, in Rotation rot) =>
            {
                killData.killTimer -= dt;

                if (killData.killTimer <= 0)
                {
                    if (HasComponent<OnKill>(e))
                    {
                        var onKill = GetComponent<OnKill>(e);
                        AudioManager.Instance.PlaySfxRequest(onKill.sfxName.ToString());
                        GameManager.Instance.AddPoints(onKill.pointValue);

                        if (EntityManager.Exists(onKill.spawnPrefab))
                        {
                            var spawnedEntity = ecb.Instantiate(onKill.spawnPrefab);
                            ecb.AddComponent(spawnedEntity, trs);
                            ecb.AddComponent(spawnedEntity, rot);
                        }
                    }

                    ecb.DestroyEntity(e);
                }
            }).WithoutBurst().Run();
    }
}
