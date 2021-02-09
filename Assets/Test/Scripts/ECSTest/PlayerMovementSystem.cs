/*
 * Author: Huai
 * Create: 2021/1/14 2:22:27
 *
 * Description:
 */

using System;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Testing
{
    [AlwaysSynchronizeSystem]
    public class PlayerMovementSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            float deltaTime = Time.DeltaTime;

            Entities.ForEach((ref Translation trans, in MovementData movementData) =>
            {
                trans.Value.x += movementData.horizontal * movementData.speed * deltaTime;
                trans.Value.y += movementData.vertical * movementData.speed * deltaTime;
            }).Run();

            return default;
        }
    }
}
