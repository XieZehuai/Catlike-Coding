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
    public class GPUGraph : MonoBehaviour
    {
        [SerializeField] private int pointNum = 10; // 点的数量（数量为（pointNum * 2）^ 2）
        [SerializeField] [Range(0.01f, 1f)] private float scale = 0.2f; // 点的缩放
        [SerializeField] [Range(0.01f, 1f)] private float interval = 0.1f; // 点之间的距离
        [SerializeField] private Func2DEnum func2D = default; // 2D图像函数
        [SerializeField] private Func3DEnum func3D = default; // 3D图像函数
        [SerializeField] private bool show3DGraph = default; // 是否显示3D图像，通过它来控制显示2D还是3D图像
        [SerializeField, Min(0.1f)] private float changeDuration = 1f;
        [SerializeField, Min(0.1f)] private float transitionDuration = 1f;

        private int length; // points列表的长度（用一维数组表示二维数组，长度为（pointNum * 2）^ 2）

        private float timer = 0f;
        private bool isTransitioning;
        private Func3DEnum nextFunction;

        private void Update()
        {
            timer += Time.deltaTime;

            if (!isTransitioning && timer >= changeDuration)
            {
                isTransitioning = true;
                timer = 0f;
                ChangeFunctionRandomly();
            }
        }

        private void ChangeFunctionRandomly()
        {
            nextFunction = (Func3DEnum)UnityEngine.Random.Range(0, 8);
        }

        private void DrawPoint(Transform point, float x, float y, float z = 0f)
        {
            point.position = new Vector3(x, y, z);
            point.localScale = Vector3.one * scale;
        }

        private void DrawPoint(Transform point, Vector3 position)
        {
            point.position = position;
            point.localScale = Vector3.one * scale;
        }
    }
}
