using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class CameraAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public AudioListener audioListener;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new CameraTag() { });

        conversionSystem.AddHybridComponent(audioListener);
    }
}
