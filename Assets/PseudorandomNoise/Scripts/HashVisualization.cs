using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;
using UnityEngine;

namespace PseudorandomNoise.Hashing
{
    public class HashVisualization : MonoBehaviour
    {
        #region HashJob Struct
        [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
        struct HashJob : IJobFor
        {
            [WriteOnly] public NativeArray<uint> hashes;

            public void Execute(int index)
            {
                hashes[index] = (uint)index;
            }
        }
        #endregion


        private static int hashesId = Shader.PropertyToID("_Hashes");
        private static int configId = Shader.PropertyToID("_Config");

        [SerializeField] private Mesh instanceMesh;
        [SerializeField] private Material material;
        [SerializeField, Range(1, 512)] private int resolution = 16;

        private NativeArray<uint> hashes;
        private ComputeBuffer hashesBuffer;
        private MaterialPropertyBlock propertyBlock;

        private void OnEnable()
        {
            int length = resolution * resolution;
            hashes = new NativeArray<uint>(length, Allocator.Persistent);
            hashesBuffer = new ComputeBuffer(length, 4);

            HashJob hashJob = new HashJob { hashes = hashes };
            hashJob.ScheduleParallel(hashes.Length, resolution, default).Complete();

            hashesBuffer.SetData(hashes);

            if (propertyBlock == null)
            {
                propertyBlock = new MaterialPropertyBlock();
            }
            propertyBlock.SetBuffer(hashesId, hashesBuffer);
            propertyBlock.SetVector(configId, new Vector4(resolution, 1f / resolution));
        }

        private void Update()
        {
            Bounds bounds = new Bounds(Vector3.zero, Vector3.one);
            Graphics.DrawMeshInstancedProcedural(instanceMesh, 0, material, bounds, hashes.Length, propertyBlock);
        }

        private void OnDisable()
        {
            hashes.Dispose();
            hashesBuffer.Release();
            hashesBuffer = null;
        }

        private void OnValidate()
        {
            if (hashesBuffer != null && enabled)
            {
                OnDisable();
                OnEnable();
            }
        }
    }
}
