using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

class FollowSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var dt = Time.DeltaTime;

        Entities
            .WithAll<Translation, Rotation>()
            .ForEach((Entity e, in FollowData followData) =>
            {
                if (HasComponent<Translation>(followData.target) && HasComponent<Rotation>(followData.target))
                {
                    var currentPos = GetComponent<Translation>(e).Value;
                    var currentRot = GetComponent<Rotation>(e).Value;

                    var targetPos = GetComponent<Translation>(followData.target).Value;
                    var targetRot = GetComponent<Rotation>(followData.target).Value;

                    /* My implementation of the follow, without the lerp effect
                     * currentPos = new float3(targetPos.x, followData.offset.y, targetPos.z);

                    SetComponent(e, new Translation() { Value = currentPos });*/

                    // Or if you want to Lerp (don't forget to add a reference to deltaTime dt at the top of the loop) :
                     targetPos += math.mul(targetRot, targetPos) * -followData.distance;
                     targetPos += followData.offset;
                     
                     targetPos = math.lerp(currentPos, targetPos, dt * followData.speedMove);
                     //targetRot = math.lerp(currentRot.value, targetRot.value, dt * followData.speedRot);
                     
                     SetComponent(e, new Translation() { Value = targetPos });
                     //SetComponent(e, new Rotation() { Value = targetRot });
                }
            }).Schedule();
    }
}