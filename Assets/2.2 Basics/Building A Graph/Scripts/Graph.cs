/*
 * Author: Huai
 * Create: 2021/1/10 3:46:25
 *
 * Description:
 */

using System;
using UnityEngine;

namespace Basics
{
    public class Graph : MonoBehaviour
    {
        [SerializeField] private Transform pointPrefab = default;
        [SerializeField, Range(10, 100)] private int resolution = 10; // 分辨率，也就是点的数量

        private Transform[] points;

        private void Awake()
        {
            points = new Transform[resolution];
            float step = 2f / resolution;
            Vector3 position = Vector3.zero;
            Vector3 scale = Vector3.one * step;

            for (int i = 0; i < points.Length; i++)
            {
                points[i] = Instantiate(pointPrefab, transform);

                position.x = (i + 0.5f) * step - 1f;
                points[i].localPosition = position;

                points[i].localScale = scale;
            }
        }

        private void Update()
        {
            float time = Time.time;
            for (int i = 0; i < points.Length; i++)
            {
                Transform point = points[i];
                Vector3 position = point.localPosition;
                position.y = Mathf.Sin(Mathf.PI * (position.x + time));
                point.localPosition = position;
            }
        }
    }
}
