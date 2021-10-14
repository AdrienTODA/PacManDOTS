using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class PlayerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var x = Input.GetAxis("Horizontal");
        var z = Input.GetAxis("Vertical");
        var dt = Time.DeltaTime;

        Entities
            .WithAll<PlayerData>()
            .ForEach((ref MovableData mov) =>
            {
                mov.direction = new float3(x, 0, z);
            }).Schedule();

        var ecb = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>().CreateCommandBuffer();

        Entities
            .WithAll<PlayerData>()
            .ForEach((Entity e, ref HealthData hp, ref PowerPillData ppd, ref DamageData dmgData) =>
            {
                dmgData.damage = 100;
                ppd.pillTimer -= dt;
                hp.invincibleTimer = ppd.pillTimer;
                AudioManager.Instance.PlayMusicRequest("powerup");

                if (ppd.pillTimer <= 0)
                {
                    AudioManager.Instance.PlayMusicRequest("game");
                    ecb.RemoveComponent<PowerPillData>(e);
                    dmgData.damage = 0;
                }
            }).WithoutBurst().Run();
    }
}