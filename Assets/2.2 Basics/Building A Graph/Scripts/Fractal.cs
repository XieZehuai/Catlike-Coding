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
        [SerializeField, Range(1, 8)] private int depth = 4; // 分形迭代的深度

        /*
         * Awake方法在对象被实例化后调用，Start方法在Update方法被调用之前调用，
         * 而Update方法只会在处于active状态下才会被调用，所以Start在OnEnable方法
         * 之后调用。
         * 
         * 在这里调用Instantiate(this)克隆自己，如果是放在Awake方法里的话，克隆生成的
         * 对象又会实例化后马上调用它的Awake方法，导致瞬间生成无数个克隆对象，所以要放在
         * Start里调用，这样每个克隆对象实例化后只会在调用Update之前克隆自己，而Update
         * 每帧调用一次，也就是说每帧克隆出一个新的对象
         */
        private void Start()
        {
            name = "Fractal_" + depth;

            if (depth <= 1) // 深度值最小为1
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
            transform.Rotate(0f, 22.5f * Time.deltaTime, 0f); // 让物体绕着y轴每秒旋转22.5度
        }

        /// <summary>
        /// 根据当前对象复制一个子物体，并设置子物体相对于当前物体的方向以及旋转角度
        /// </summary>
        /// <param name="direction">子物体的方向</param>
        /// <param name="rotation">子物体的角度</param>
        /// <returns>子物体上的Fractal组件的引用</returns>
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
