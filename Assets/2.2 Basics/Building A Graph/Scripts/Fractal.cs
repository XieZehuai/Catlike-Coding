/*
 * Author: Huai
 * Create: 2021/1/18 2:00:47
 *
 * Description:
 */

using System;
using UnityEngine;

namespace Basics.BuildingAGraph
{
    public class Fractal : MonoBehaviour
    {
        [SerializeField, Range(1, 8)] private int depth = 4; // ���ε��������

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


        /***********************************************************************
         *              �·�����Ч�ʸ���
         ***********************************************************************/
        private struct FractalPart
        {
            public Vector3 direction;
            public Vector3 worldPosition;
            public Quaternion rotation;
            public Quaternion worldRotation;
            public float spinAngle;
        }

        private FractalPart[][] parts;
        private Matrix4x4[][] matrices;
        private ComputeBuffer[] matricesBuffers;

        private static readonly int matricesId = Shader.PropertyToID("_Matrices");
        private static MaterialPropertyBlock propertyBlock;

        private void OnEnable()
        {
            parts = new FractalPart[depth][];
            matrices = new Matrix4x4[depth][];
            matricesBuffers = new ComputeBuffer[depth];
            int stride = 16 * 4; // һ��Matrix4x4�������16��float�������ݣ�Ҳ����16 * 4�ֽڵ�����

            // ����ÿ��ڵ�ĸ�������һ��Ϊ���ڵ㣬ֻ��һ����ÿ���ڵ㶼������5���ӽڵ�
            // ��������һ��ڵ������Ϊ��ǰ���5��
            for (int i = 0, length = 1; i < parts.Length; i++, length *= 5)
            {
                parts[i] = new FractalPart[length];
                matrices[i] = new Matrix4x4[length];
                matricesBuffers[i] = new ComputeBuffer(length, stride);
            }

            parts[0][0] = CreateParts(0);

            for (int i = 1; i < parts.Length; i++)
            {
                FractalPart[] part = parts[i];

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
            for (int i = 1; i < parts.Length; i++)
            {
                scale *= 0.5f;
                FractalPart[] parentLevel = parts[i - 1];
                FractalPart[] level = parts[i];
                Matrix4x4[] levelMatrices = matrices[i];

                for (int j = 0; j < level.Length; j++)
                {
                    FractalPart parent = parentLevel[j / 5];
                    FractalPart part = level[j];

                    part.spinAngle += spinAngleDelta;
                    part.worldRotation = parent.worldRotation * (part.rotation * Quaternion.Euler(0f, part.spinAngle, 0f));
                    part.worldPosition = parent.worldPosition + parent.worldRotation * (scale * part.direction * 1.5f);

                    level[j] = part;
                    levelMatrices[j] = Matrix4x4.TRS(part.worldPosition, part.worldRotation, scale * Vector3.one);
                }
            }

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
         *              �����ַ������ɵ�ͼ��Ч�ʲ��ߣ���ʱ��ע�͵�
         ***********************************************************************/

        /*
         * Awake�����ڶ���ʵ��������ã�Start������Update����������֮ǰ���ã�
         * ��Update����ֻ���ڴ���active״̬�²Żᱻ���ã�����Start��OnEnable����
         * ֮����á�
         * 
         * ���������Instantiate(this)��¡�Լ�������Ƿ���Awake������Ļ�����¡���ɵ�
         * �����ֻ�ʵ���������ϵ�������Awake����������˲��������������¡��������Ҫ����
         * Start����ã�����ÿ����¡����ʵ������ֻ���ڵ���Update֮ǰ��¡�Լ�����Update
         * ÿ֡����һ�Σ�Ҳ����˵ÿ֡��¡��һ���µĶ���
         */
        //private void Start()
        //{
        //    name = "Fractal_" + depth;

        //    if (depth <= 1) // ���ֵ��СΪ1
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
        //    transform.Rotate(0f, 22.5f * Time.deltaTime, 0f); // ����������y��ÿ����ת22.5��
        //}

        /// <summary>
        /// ���ݵ�ǰ������һ�������壬����������������ڵ�ǰ����ķ����Լ���ת�Ƕ�
        /// </summary>
        /// <param name="direction">������ķ���</param>
        /// <param name="rotation">������ĽǶ�</param>
        /// <returns>�������ϵ�Fractal���������</returns>
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
