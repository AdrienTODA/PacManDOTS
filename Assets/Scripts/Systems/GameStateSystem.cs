using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[AlwaysUpdateSystem]
public class GameStateSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var pelletQuery = GetEntityQuery(ComponentType.ReadOnly<PelletData>());
        var pelletsCount = pelletQuery.CalculateEntityCount();
        var playerQuery = GetEntityQuery(ComponentType.ReadOnly<PlayerData>());
        var playerCount = playerQuery.CalculateEntityCount();
        var enemyQuery = GetEntityQuery(ComponentType.ReadOnly<EnemyData>());
        var spawnerQuery = GetEntityQuery(typeof(Spawner));

        GameManager.Instance.UpdatePelletsCount(pelletsCount);

        if (pelletsCount <= 0 && playerCount > 0)
        {
            GameManager.Instance.Win();

            Entities
                .WithAll<PhysicsVelocity>()
                .ForEach((Entity e) =>
                {
                    EntityManager.RemoveComponent<PhysicsVelocity>(e);
                    EntityManager.RemoveComponent<PowerPillData>(e);
                }).WithStructuralChanges().Run();
        }

        Entities
            .WithAll<PlayerData, PhysicsVelocity>()
            .ForEach((Entity e, in KillData killData) =>
            {
                EntityManager.RemoveComponent<PhysicsVelocity>(e);
                EntityManager.RemoveComponent<MovableData>(e);
                GameManager.Instance.LoseLife();

                if (GameManager.Instance.lives < 0)
                {
                    var spawnerArray = spawnerQuery.ToEntityArray(Allocator.TempJob);

                    foreach (var spawner in spawnerArray)
                    {
                        EntityManager.DestroyEntity(spawner);
                    }

                    spawnerArray.Dispose();
                }

                var enemyArray = enemyQuery.ToEntityArray(Allocator.TempJob);

                foreach (var enemy in enemyArray)
                {
                    EntityManager.RemoveComponent<PhysicsVelocity>(enemy);
                    EntityManager.RemoveComponent<MovableData>(enemy);
                    EntityManager.RemoveComponent<OnKill>(enemy);
                    EntityManager.AddComponentData(enemy, killData);
                }

                enemyArray.Dispose();
            }).WithStructuralChanges().Run();

        // If you REALLY need to read something from the GameManager GameObject
        // you can use an Entities.ForEach loop with .WithoutBurst() and .Run()
        // with GameManager gameMan GameObject passed in the loop to be able to
        // access it. You also need to add Convert to Entity script to the GameManager
        // with Convert and Inject GameObject option to retain the GameObject behaviour
    }
}
