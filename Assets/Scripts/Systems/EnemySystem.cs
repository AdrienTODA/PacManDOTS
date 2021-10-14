using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using Unity.Physics.Systems;

//[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class EnemySystem : SystemBase
{
    private Random rng = new Random(1234);
    
    protected override void OnUpdate()
    {
        var rayCaster = new MovementRaycast() { pw = World.GetOrCreateSystem<BuildPhysicsWorld>().PhysicsWorld };
        rng.NextInt();
        var rngTemp = rng;

        Entities
            .ForEach((ref MovableData mov, ref EnemyData enemy, in Translation translation) =>
            {
                if (math.distance(translation.Value, enemy.previousCell) > 0.9f)
                {
                    enemy.previousCell = math.round(translation.Value);

                    // perform raycasts here
                    var validDir = new NativeList<float3>(Allocator.Temp);

                    if (!rayCaster.CheckRay(translation.Value, new float3(0, 0, -1), mov.direction))
                        validDir.Add(new float3(0, 0, -1));
                    if (!rayCaster.CheckRay(translation.Value, new float3(0, 0, 1), mov.direction))
                        validDir.Add(new float3(0, 0, 1));
                    if (!rayCaster.CheckRay(translation.Value, new float3(-1, 0, 0), mov.direction))
                        validDir.Add(new float3(-1, 0, 0));
                    if (!rayCaster.CheckRay(translation.Value, new float3(1, 0, 0), mov.direction))
                        validDir.Add(new float3(1, 0, 0));

                    mov.direction = validDir[rngTemp.NextInt(validDir.Length)];

                    validDir.Dispose();
                }
            }).Schedule();
    }

    private struct MovementRaycast
    {
        [ReadOnly]
        public PhysicsWorld pw;

        public bool CheckRay(float3 pos, float3 direction, float3 currentDirection)
        {
            if (direction.Equals(-currentDirection))
                return true;

            var ray = new RaycastInput()
            {
                Start = pos,
                End = pos + (direction * .9f),
                Filter = new CollisionFilter()
                {
                    GroupIndex = 0,
                    BelongsTo = 1u << 1,
                    CollidesWith = 1u << 2
                }
            };

            return pw.CastRay(ray);
        }
    }
}
