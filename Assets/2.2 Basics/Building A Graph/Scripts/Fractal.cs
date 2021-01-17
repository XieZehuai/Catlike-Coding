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
        private void Start()
        {
            name = "Fractal_" + depth;

            if (depth <= 1) // ���ֵ��СΪ1
            {
                return;
            }

            Fractal childUp = CreateChild(Vector3.up, Quaternion.identity);
            Fractal childRight = CreateChild(Vector3.right, Quaternion.Euler(0f, 0f, -90f));
            Fractal childLeft = CreateChild(Vector3.left, Quaternion.Euler(0f, 0f, 90f));
            Fractal childForward = CreateChild(Vector3.forward, Quaternion.Euler(90f, 0f, 0f));
            Fractal childBack = CreateChild(Vector3.back, Quaternion.Euler(-90f, 0f, 0f));

            childUp.transform.SetParent(transform, false);
            childRight.transform.SetParent(transform, false);
            childLeft.transform.SetParent(transform, false);
            childForward.transform.SetParent(transform, false);
            childBack.transform.SetParent(transform, false);
        }

        private void Update()
        {
            transform.Rotate(0f, 22.5f * Time.deltaTime, 0f); // ����������y��ÿ����ת22.5��
        }

        /// <summary>
        /// ���ݵ�ǰ������һ�������壬����������������ڵ�ǰ����ķ����Լ���ת�Ƕ�
        /// </summary>
        /// <param name="direction">������ķ���</param>
        /// <param name="rotation">������ĽǶ�</param>
        /// <returns>�������ϵ�Fractal���������</returns>
        private Fractal CreateChild(Vector3 direction, Quaternion rotation)
        {
            Fractal child = Instantiate(this);
            child.depth = depth - 1;

            Transform childTransform = child.transform;
            childTransform.localPosition = 0.75f * direction;
            childTransform.localRotation = rotation;
            childTransform.localScale = 0.5f * Vector3.one;

            return child;
        }
    }
}
