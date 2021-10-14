using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    public Animator playerAnim;
    public Transform playerTransform;

    // Update is called once per frame
    void Update()
    {
        var em = World.DefaultGameObjectInjectionWorld.EntityManager;
        var playerQuery = em.CreateEntityQuery(typeof(PlayerData), typeof(PhysicsVelocity), typeof(Translation), typeof(Rotation), typeof(MovableData));

        if (playerQuery.CalculateEntityCount() > 0)
        {
            Reset();
            var playerEntity = playerQuery.GetSingletonEntity();
            var speed = math.length(em.GetComponentData<PhysicsVelocity>(playerEntity).Linear);
            var dir = em.GetComponentData<MovableData>(playerEntity).direction;

            if (math.length(dir) > .2f)
            {
                playerTransform.rotation = Quaternion.LookRotation(dir);
            }

            playerTransform.position = em.GetComponentData<Translation>(playerEntity).Value;
            playerAnim.SetFloat("Speed", speed * 10f);
        }
    }

    public void Win()
    {
        playerAnim.SetBool("Win", true);
        playerAnim.SetFloat("Speed", 0);
    }

    public void Lose()
    {
        playerAnim.SetBool("Hit", true);
        playerAnim.SetFloat("Speed", 0);
    }

    public void Reset()
    {
        playerAnim.SetBool("Win", false);
        playerAnim.SetBool("Hit", false);
    }
}
