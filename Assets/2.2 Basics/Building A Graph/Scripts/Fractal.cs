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

namespace Basics.BuildingAGraph
{
    public class Fractal : MonoBehaviour
    {
        [SerializeField, Range(1, 8)] private int depth = 4; // the depth of the fractal iteration

        [SerializeField] private Mesh mesh = default;
        [SerializeField] private Material material = default;

        private static readonly Vector3[] directions =
        {
            Vector3.up, Vector3.right, Vector3.left, Vector3.forward, Vector3.back
        };

        private static readonly Quaternion[] rotations =
        {
            Quaternion.identity, Quaternion.Euler(0f, 0f, -90f), Quaternion.Euler(0f, 0f, 90f),
            Quaternion.Euler(90f, 0f, 0f), Quaternion.Euler(-90f, 0f, 0f)
        };

        private struct FractalPart
        {
            public Vector3 direction;
            public Vector3 worldPosition;
            public Quaternion rotation;
            public Quaternion worldRotation;
            public float spinAngle;
        }

        [BurstCompile(CompileSynchronously = true)]
        private struct UpdateFractalLevelJob : IJobFor
        {
            public float spinAngleDelta;
            public float scale;

            [ReadOnly] public NativeArray<FractalPart> parents;
            public NativeArray<FractalPart> parts;

            [WriteOnly] public NativeArray<Matrix4x4> matrices;

            public void Execute(int index)
            {
                FractalPart parent = parents[index / 5];
                FractalPart part = parts[index];
                part.spinAngle += spinAngleDelta;
                part.worldRotation = parent.worldRotation * (part.rotation * Quaternion.Euler(0f, part.spinAngle, 0f));
                part.worldPosition = parent.worldPosition + parent.worldRotation * (1.5f * scale * part.direction);

                parts[index] = part;
                matrices[index] = Matrix4x4.TRS(part.worldPosition, part.worldRotation, scale * Vector3.one);
            }
        }

        private NativeArray<FractalPart>[] parts;
        private NativeArray<Matrix4x4>[] matrices;
        private ComputeBuffer[] matricesBuffers;

        private static readonly int matricesId = Shader.PropertyToID("_Matrices");
        private static MaterialPropertyBlock propertyBlock;

        private void OnEnable()
        {
            parts = new NativeArray<FractalPart>[depth];
            matrices = new NativeArray<Matrix4x4>[depth];
            matricesBuffers = new ComputeBuffer[depth];

            // Because the Matrix4x4 is made up of 16 float numbers, each
            // float numbers has 4 bytes, so the size of a Matrix4x4 is 16 * 4
            int stride = 16 * 4;

            // Calculate the amount of points each level, because each point has 
            // 5 sub-points, so the amount of points in each level is 5 times of 
            // the previous level
            for (int i = 0, length = 1; i < parts.Length; i++, length *= 5)
            {
                parts[i] = new NativeArray<FractalPart>(length, Allocator.Persistent);
                matrices[i] = new NativeArray<Matrix4x4>(length, Allocator.Persistent);
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
            float spinAngleDelta = 22.5f * Time.deltaTime;

            FractalPart root = parts[0][0];
            root.spinAngle += spinAngleDelta;
            // root.worldRotation = root.rotation * Quaternion.Euler(0f, root.spinAngle, 0f);
            root.worldRotation = transform.rotation * (root.rotation * Quaternion.Euler(0f, root.spinAngle, 0f));
            root.worldPosition = transform.position;
            parts[0][0] = root;
            float objectScale = transform.lossyScale.x;
            matrices[0][0] = Matrix4x4.TRS(root.worldPosition, root.worldRotation, objectScale * Vector3.one);

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
                }.Schedule(parts[i].Length, jobHandle);
            }

            jobHandle.Complete();

            Bounds bounds = new Bounds(root.worldPosition, 3f * objectScale * Vector3.one);
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


        /***********************************************************************
         *              ?????????????????Ч???????????????
         ***********************************************************************/

        //private void Start()
        //{
        //    name = "Fractal_" + depth;

        //    if (depth <= 1) // ??????С?1
        //    {
        //        return;
        //    }

        //    Fractal childUp = CreateChild(Vector3.up, Quaternion.identity);
        //    Fractal childRight = CreateChild(Vector3.right, Quaternion.Euler(0f, 0f, -90f));
        //    Fractal childLeft = CreateChild(Vector3.left, Quaternion.Euler(0f, 0f, 90f));
        //    Fractal childForward = CreateChild(Vector3.forward, Quaternion.Euler(90f, 0f, 0f));
        //    Fractal childBack = CreateChild(Vector3.back, Quaternion.Euler(-90f, 0f, 0f));

        //    childUp.transform.SetParent(transform, false);
        //    childRight.transform.SetParent(transform, false);
        //    childLeft.transform.SetParent(transform, false);
        //    childForward.transform.SetParent(transform, false);
        //    childBack.transform.SetParent(transform, false);
        //}

        //private void Update()
        //{
        //    transform.Rotate(0f, 22.5f * Time.deltaTime, 0f); // ??????????y????????22.5??
        //}

        /// <summary>
        /// ????????????????????壬????????????????????????????????????
        /// </summary>
        /// <param name="direction">??????????</param>
        /// <param name="rotation">?????????</param>
        /// <returns>?????????Fractal?????????</returns>
        //private Fractal CreateChild(Vector3 direction, Quaternion rotation)
        //{
        //    Fractal child = Instantiate(this);
        //    child.depth = depth - 1;

        //    Transform childTransform = child.transform;
        //    childTransform.localPosition = 0.75f * direction;
        //    childTransform.localRotation = rotation;
        //    childTransform.localScale = 0.5f * Vector3.one;

        //    return child;
        //}
    }
}
