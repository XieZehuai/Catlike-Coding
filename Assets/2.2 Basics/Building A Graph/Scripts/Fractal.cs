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
            public Quaternion rotation;
            public Transform transform;
        }

        private FractalPart[][] parts;

        private void Awake()
        {
            parts = new FractalPart[depth][];

            // ����ÿ��ڵ�ĸ�������һ��Ϊ���ڵ㣬ֻ��һ����ÿ���ڵ㶼������5���ӽڵ�
            // ��������һ��ڵ������Ϊ��ǰ���5��
            for (int i = 0, length = 1; i < parts.Length; i++, length *= 5)
            {
                parts[i] = new FractalPart[length];
            }

            float scale = 1f;
            parts[0][0] = CreateParts(0, 0, scale);

            for (int i = 1; i < parts.Length; i++)
            {
                FractalPart[] part = parts[i];
                scale *= 0.5f;

                for (int j = 0; j < part.Length; j += 5)
                {
                    for (int k = 0; k < 5; k++)
                    {
                        parts[i][j + k] = CreateParts(i, k, scale);
                    }
                }
            }
        }

        private void Update()
        {
            Quaternion deltaRotation = Quaternion.Euler(0f, 22.5f * Time.deltaTime, 0f);

            FractalPart root = parts[0][0];
            root.rotation *= deltaRotation;
            root.transform.localRotation = root.rotation;
            parts[0][0] = root;

            for (int i = 1; i < parts.Length; i++)
            {
                FractalPart[] parentLevel = parts[i - 1];
                FractalPart[] level = parts[i];

                for (int j = 0; j < level.Length; j++)
                {
                    Transform parent = parentLevel[j / 5].transform;
                    FractalPart part = level[j];
                    part.rotation *= deltaRotation;

                    part.transform.localRotation = parent.localRotation * part.rotation;
                    part.transform.localPosition = parent.localPosition + parent.localRotation * (part.transform.localScale.x * part.direction * 1.5f);

                    level[j] = part;
                }
            }
        }

        private FractalPart CreateParts(int level, int childIndex, float scale)
        {
            GameObject obj = new GameObject("Fractal Part level " + level + "  child " + childIndex);

            obj.transform.localScale = scale * Vector3.one;
            obj.transform.SetParent(transform, false);
            obj.AddComponent<MeshFilter>().mesh = mesh;
            obj.AddComponent<MeshRenderer>().material = material;

            return new FractalPart
            {
                direction = directions[childIndex],
                rotation = rotations[childIndex],
                transform = obj.transform
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
