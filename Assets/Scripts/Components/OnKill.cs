using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public struct OnKill : IComponentData
{
    public FixedString64 sfxName;
    public Entity spawnPrefab;
    public int pointValue;
}
