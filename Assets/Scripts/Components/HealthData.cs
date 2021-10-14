using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct HealthData : IComponentData
{
    public float health, invincibleTimer, killTimer;
    public FixedString64 damageSfx, deathSfx;
}