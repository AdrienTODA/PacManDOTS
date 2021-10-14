using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

class MovableSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithName("MovableSystem")
            .ForEach((ref PhysicsVelocity physics, in MovableData mov) =>
            {
                var step = mov.direction * mov.speed;
                physics.Linear = step;
            }).Schedule();
    }
}