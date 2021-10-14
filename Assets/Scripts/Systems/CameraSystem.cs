using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class CameraSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var playerQuery = GetEntityQuery(typeof(PlayerData), typeof(Translation));

        if (playerQuery.CalculateEntityCount() == 0)
            return;

        var player = playerQuery.GetSingletonEntity();
        var playerPos = GetComponent<Translation>(player).Value;
        var minDist = float.MaxValue;

        var cameraQuery = GetEntityQuery(typeof(CameraTag), typeof(FollowData));

        if (cameraQuery.CalculateEntityCount() == 0)
            return;

        var camera = cameraQuery.GetSingletonEntity();

        var camFollow = GetComponent<FollowData>(camera);

        Entities
            .WithAll<CameraPointData>()
            .ForEach((Entity e, in Translation translation) =>
            {
                var distCalc = math.distance(playerPos, translation.Value);
                if (distCalc < minDist)
                {
                    minDist = distCalc;
                    camFollow.target = e;
                    SetComponent(camera, camFollow);
                }
            }).Run();
    }
}
