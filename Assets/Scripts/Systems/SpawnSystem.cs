using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class SpawnSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .ForEach((ref Spawner spawner, in Translation trs, in Rotation rot) =>
            {
                if (!EntityManager.Exists(spawner.spawnObject))
                {
                    spawner.spawnObject = EntityManager.Instantiate(spawner.spawnPrefab);
                    EntityManager.SetComponentData(spawner.spawnObject, trs);
                    EntityManager.SetComponentData(spawner.spawnObject, rot);
                }
            }).WithStructuralChanges().Run();
    }
}
