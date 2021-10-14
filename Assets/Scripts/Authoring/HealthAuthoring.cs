using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class HealthAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public float killTimer, hpValue;
    public string dmgSfx, deathSfx;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new HealthData()
        {
            invincibleTimer = 0,
            killTimer = killTimer,
            health = hpValue,
            damageSfx = new Unity.Collections.FixedString64(dmgSfx),
            deathSfx = new Unity.Collections.FixedString64(deathSfx)
        });
    }
}
