using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct FollowData : IComponentData
{
    public Entity target;
    public float distance, speedMove, speedRot;
    public float3 offset;
    public bool freezeXPos, freezeYPos, freezeZPos, freezeRot;
}
