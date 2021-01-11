/*
 * Author: Huai
 * Create: 2021/1/10 3:46:25
 *
 * Description:
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Basics.BuildingAGraph
{
    public class Graph : MonoBehaviour
    {
        [SerializeField] private Transform pointPrefabs; // 点的预设
        [SerializeField] private int pointNum = 10; // 点的数量（数量为（pointNum * 2）^ 2）
        [SerializeField] [Range(0.01f, 1f)] private float scale = 0.2f; // 点的缩放
        [SerializeField] [Range(0.01f, 1f)] private float interval = 0.1f; // 点之间的距离
        [SerializeField] private Func2DEnum func2D = default; // 2D图像函数
        [SerializeField] private Func3DEnum func3D = default; // 3D图像函数
        [SerializeField] private bool show3DGraph = default; // 是否显示3D图像，通过它来控制显示2D还是3D图像
        [SerializeField, Min(0.1f)] private float changeDuration = 1f;
        [SerializeField, Min(0.1f)] private float transitionDuration = 1f;

        private readonly List<Transform> points = new List<Transform>(); // 储存所有点的实体
        private int length; // points列表的长度（用一维数组表示二维数组，长度为（pointNum * 2）^ 2）

        private float timer = 0f;
        private bool isTransitioning;
        private Func3DEnum nextFunction;

        private void Start()
        {
            nextFunction = func3D;

            // 获取点列表长度并实例化点
            length = (int)Mathf.Pow(pointNum * 2, 2);
            for (int i = 0; i < length; i++)
            {
                points.Add(Instantiate(pointPrefabs, transform));
            }
        }

        private void Update()
        {
            timer += Time.deltaTime;

            if (!isTransitioning && timer >= changeDuration)
            {
                isTransitioning = true;
                timer = 0f;
                ChangeFunctionRandomly();
            }

            if (isTransitioning)
            {
                DrawGraphTransition();

                if (timer >= transitionDuration)
                {
                    timer = 0f;
                    func3D = nextFunction;
                    isTransitioning = false;
                }
            }
            else
            {
                DrawGraph(); // 画图，在Update里调用让图像能根据数据的变化实时改变
            }
        }

        // 画图
        private void DrawGraph()
        {
            for (float v = 0; v < pointNum * 2; v++)
            {
                for (float u = 0; u < pointNum * 2; u++)
                {
                    float index = v * pointNum * 2 + u; // 把二维坐标转化为一维坐标
                    float y; // 点的y坐标的值

                    // 判断要画的是2D图像还是3D图像
                    if (show3DGraph)
                    {

                        Vector3 position = Function.Instance.Func3D(u / pointNum - 1f, v / pointNum - 1f, func3D);
                        DrawPoint(points[(int)index], position * interval * 20f);
                    }
                    else
                    {
                        y = Function.Instance.Func2D(u / pointNum - 1f, func2D);
                        DrawPoint(points[(int)index], (u - pointNum) * interval, y * interval * 10f, (v - pointNum) * interval);
                    }
                }
            }
        }

        private void DrawGraphTransition()
        {
            Function.GraphFunction from = Function.Instance.GetFunction(func3D);
            Function.GraphFunction to = Function.Instance.GetFunction(nextFunction);
            float progress = timer / transitionDuration;

            for (float v = 0; v < pointNum * 2; v++)
            {
                for (float u = 0; u < pointNum * 2; u++)
                {
                    float index = v * pointNum * 2 + u; // 把二维坐标转化为一维坐标
                    float y; // 点的y坐标的值

                    // 判断要画的是2D图像还是3D图像
                    if (show3DGraph)
                    {
                        Vector3 position = Function.Morph(u / pointNum - 1f, v / pointNum - 1f, from, to, progress);
                        DrawPoint(points[(int)index], position * interval * 20f);
                    }
                    else
                    {
                        y = Function.Instance.Func2D(u / pointNum - 1f, func2D);
                        DrawPoint(points[(int)index], (u - pointNum) * interval, y * interval * 10f, (v - pointNum) * interval);
                    }
                }
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
