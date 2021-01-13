/*
 * Author: Huai
 * Create: 2021/1/14 2:16:05
 *
 * Description:
 */

using System;
using UnityEngine;
using Unity.Jobs;
using Unity.Entities;

namespace Testing
{
    [AlwaysSynchronizeSystem]
    public class PlayerInputSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            Entities.ForEach((ref MovementData movementData, in InputData inputData) =>
            {
                movementData.horizontal = 0;
                movementData.horizontal += Input.GetKey(inputData.rightKey) ? 1 : 0;
                movementData.horizontal -= Input.GetKey(inputData.leftKey) ? 1 : 0;

                movementData.vertical = 0;
                movementData.vertical += Input.GetKey(inputData.upKey) ? 1 : 0;
                movementData.vertical -= Input.GetKey(inputData.downKey) ? 1 : 0;
            }).Run();

            return default;
        }
    }
}
