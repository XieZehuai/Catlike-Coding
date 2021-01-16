/*
 * Author: Huai
 * Create: 2021/1/12 13:25:02
 *
 * Description:
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Basics.BuildingAGraph;

namespace Basics.ComputeShaders
{
    public enum TransitionMode
    {
        Cycle,
        Random,
    }


    public enum GPUFunction
    {
        Wave,
        MultiWave,
        Ripple,
        Shpere,
        Torus,
    }


    public class GPUGraph : MonoBehaviour
    {
        private const int MAX_RESOLUTION = 200;

        [SerializeField] ComputeShader computeShader = default;
        [SerializeField] private Material material = default;
        [SerializeField] private Mesh mesh = default;
        [SerializeField, Range(10, MAX_RESOLUTION)] private int resolution = 20;
        [SerializeField] private GPUFunction function = GPUFunction.Wave;
        [SerializeField] private TransitionMode transitionMode = TransitionMode.Cycle;
        [SerializeField, Min(0f)] private float functionDuration = 1f;
        [SerializeField, Min(0f)] private float transitionDuration = 1f;

        private float duration;
        private bool isTransitioning;
        private GPUFunction nextFunction;

        private ComputeBuffer positionBuffer;
        private static readonly int positionId = Shader.PropertyToID("_Position");
        private static readonly int resolutionId = Shader.PropertyToID("_Resolution");
        private static readonly int stepId = Shader.PropertyToID("_Step");
        private static readonly int timeId = Shader.PropertyToID("_Time");
        private static readonly int transitionProgressId = Shader.PropertyToID("_TransitionProgress");

        private void OnEnable()
        {
            // 因为在GPU上没有数据类型的概念，所以需要自己计算每个数据元素的大小
            // Vector3包含3个float类型字段，每个float类型4字节，所以大小为3 * 4
            positionBuffer = new ComputeBuffer(MAX_RESOLUTION * MAX_RESOLUTION, 3 * 4);
        }

        private void Update()
        {
            duration += Time.deltaTime;

            if (!isTransitioning && duration >= functionDuration)
            {
                isTransitioning = true;
                duration = 0f;
                ChangeFunction();
            }

            UpdatetFunctionOnGPU();
        }

        private void UpdatetFunctionOnGPU()
        {
            float step = 2f / resolution;
            computeShader.SetInt(resolutionId, resolution);
            computeShader.SetFloat(stepId, step);
            computeShader.SetFloat(timeId, Time.time);

            if (isTransitioning)
            {
                float progress = duration / transitionDuration;
                if (progress >= transitionDuration)
                {
                    duration = 0f;
                    function = nextFunction;
                    isTransitioning = false;
                }
                else
                {
                    computeShader.SetFloat(transitionProgressId, Mathf.SmoothStep(0f, 1f, progress));
                }
            }

            //int kernelIndex = (int)function + (int)(isTransitioning ? nextFunction : function) * 5;
            int kernelIndex = (int)function * 6 + nextFunction - function;
            computeShader.SetBuffer(kernelIndex, positionId, positionBuffer);

            int groups = Mathf.CeilToInt(resolution / 8f);
            computeShader.Dispatch(kernelIndex, groups, groups, 1);

            material.SetBuffer(positionId, positionBuffer);
            material.SetFloat(stepId, step);
            Bounds bounds = new Bounds(Vector3.zero, new Vector3(2f, 2f + 2f / resolution, 2f));
            Graphics.DrawMeshInstancedProcedural(mesh, 0, material, bounds, resolution * resolution);
        }

        private void ChangeFunction()
        {
            if (transitionMode == TransitionMode.Cycle)
            {
                nextFunction = (GPUFunction)(((int)function + 1) % 5);
            }
            else
            {
                nextFunction = (GPUFunction)UnityEngine.Random.Range(0, 5);
            }
        }

        private void OnDisable()
        {
            positionBuffer.Release();
            positionBuffer = null;
        }
    }
}
