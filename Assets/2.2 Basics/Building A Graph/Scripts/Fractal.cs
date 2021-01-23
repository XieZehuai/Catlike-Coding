/*
 * Author: Huai
 * Create: 2021/1/18 2:00:47
 *
 * Description:
 */

using System;
using UnityEngine;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using quaternion = Unity.Mathematics.quaternion;

namespace Basics.BuildingAGraph
{
    public class Fractal : MonoBehaviour
    {
        [SerializeField, Range(1, 9)] private int depth = 4; // the depth of the fractal iteration

        [SerializeField] private Mesh mesh = default;
        [SerializeField] private Material material = default;

        private static readonly float3[] directions =
        {
            up(), right(), left(), forward(), back()
        };

        private static readonly quaternion[] rotations =
        {
            quaternion.identity,
            quaternion.RotateZ(-0.5f * PI), quaternion.RotateZ(0.5f * PI),
            quaternion.RotateX(0.5f * PI), quaternion.RotateX(-0.5f * PI)
        };

        private struct FractalPart
        {
            public float3 direction;
            public float3 worldPosition;
            public quaternion rotation;
            public quaternion worldRotation;
            public float spinAngle;
        }

        [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
        private struct UpdateFractalLevelJob : IJobFor
        {
            public float spinAngleDelta;
            public float scale;

            [ReadOnly] public NativeArray<FractalPart> parents;
            public NativeArray<FractalPart> parts;

            [WriteOnly] public NativeArray<float3x4> matrices;

            public void Execute(int index)
            {
                FractalPart parent = parents[index / 5];
                FractalPart part = parts[index];

                part.spinAngle += spinAngleDelta;

                part.worldRotation = mul(parent.worldRotation, mul(part.rotation, quaternion.RotateY(part.spinAngle)));

                part.worldPosition = parent.worldPosition + mul(parent.worldRotation, 1.5f * scale * part.direction);

                parts[index] = part;

                // Bacause there is no TRS method for float3x4, so we have to
                // assemble the matri
                float3x3 r = float3x3(part.worldRotation) * scale;
                matrices[index] = float3x4(r.c0, r.c1, r.c2, part.worldPosition);
            }
        }

        private NativeArray<FractalPart>[] parts;
        private NativeArray<float3x4>[] matrices;
        private ComputeBuffer[] matricesBuffers;

        private static readonly int matricesId = Shader.PropertyToID("_Matrices");
        private static MaterialPropertyBlock propertyBlock;

        private void OnEnable()
        {
            parts = new NativeArray<FractalPart>[depth];
            matrices = new NativeArray<float3x4>[depth];
            matricesBuffers = new ComputeBuffer[depth];

            // Because the float3x4 is made up of 12 float numbers, each
            // float numbers has 4 bytes, so the size of a float3x4 is 12 * 4
            int stride = 12 * 4;

            // Calculate the amount of points each level, because each point has 
            // 5 sub-points, so the amount of points in each level is 5 times of 
            // the previous level
            for (int i = 0, length = 1; i < parts.Length; i++, length *= 5)
            {
                parts[i] = new NativeArray<FractalPart>(length, Allocator.Persistent);
                matrices[i] = new NativeArray<float3x4>(length, Allocator.Persistent);
                matricesBuffers[i] = new ComputeBuffer(length, stride);
            }

            parts[0][0] = CreateParts(0);

            for (int i = 1; i < parts.Length; i++)
            {
                NativeArray<FractalPart> part = parts[i];

                for (int j = 0; j < part.Length; j += 5)
                {
                    for (int k = 0; k < 5; k++)
                    {
                        parts[i][j + k] = CreateParts(k);
                    }
                }
            }

            if (propertyBlock == null)
            {
                propertyBlock = new MaterialPropertyBlock();
            }
        }

        private void Update()
        {
            float spinAngleDelta = 0.125f * PI * Time.deltaTime;
            FractalPart root = parts[0][0];

            root.spinAngle += spinAngleDelta;

            root.worldRotation = mul(transform.rotation, mul(root.rotation, quaternion.RotateY(root.spinAngle)));

            root.worldPosition = transform.position;
            parts[0][0] = root;
            float objectScale = transform.lossyScale.x;

            float3x3 r = float3x3(root.worldRotation) * objectScale;
            matrices[0][0] = float3x4(r.c0, r.c1, r.c2, root.worldPosition);

            float scale = objectScale;
            JobHandle jobHandle = default;

            for (int i = 1; i < parts.Length; i++)
            {
                scale *= 0.5f;
                jobHandle = new UpdateFractalLevelJob
                {
                    spinAngleDelta = spinAngleDelta,
                    scale = scale,
                    parents = parts[i - 1],
                    parts = parts[i],
                    matrices = matrices[i]
                }.ScheduleParallel(parts[i].Length, 5, jobHandle);
            }

            jobHandle.Complete();

            Bounds bounds = new Bounds(root.worldPosition, 3f * objectScale * float3(objectScale));

            for (int i = 0; i < matricesBuffers.Length; i++)
            {
                ComputeBuffer buffer = matricesBuffers[i];
                buffer.SetData(matrices[i]);
                propertyBlock.SetBuffer(matricesId, buffer);
                Graphics.DrawMeshInstancedProcedural(mesh, 0, material, bounds, buffer.count, propertyBlock);
            }
        }

        private void OnDisable()
        {
            for (int i = 0; i < matricesBuffers.Length; i++)
            {
                matricesBuffers[i].Release();
                parts[i].Dispose();
                matrices[i].Dispose();
            }

            parts = null;
            matrices = null;
            matricesBuffers = null;
        }

        private void OnValidate()
        {
            if (parts != null && enabled)
            {
                OnDisable();
                OnEnable();
            }
        }

        private FractalPart CreateParts(int childIndex)
        {
            return new FractalPart
            {
                direction = directions[childIndex],
                rotation = rotations[childIndex],
            };
        }
    }
}
