/*
 * Author: Huai
 * Create: 2021/1/10 3:57:32
 *
 * Description:
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Basics.BuildingAGraph
{
    /// <summary>
    /// 2D��������
    /// </summary>
    public enum Func2DEnum
    {
        FuncSin,
        FuncSin1,
        FuncCos
    }

    /// <summary>
    /// 3D��������
    /// </summary>
    public enum Func3DEnum
    {
        FuncMultiSin1,
        FuncMultiSin2,
        FuncMultiSin3,
        Ripple,
        Cylinder,
        Sphere,
        Torus,
        Custom
    }

    public class Function
    {
        public static Function Instance { get; } = new Function();

        public delegate Vector3 GraphFunction(float u, float v);

        private static Dictionary<Func3DEnum, GraphFunction> funcDic;
        private const float pi = Mathf.PI;

        private Function()
        {
            funcDic = new Dictionary<Func3DEnum, GraphFunction>();
        }

        /*
         * ��ά�������ýӿڣ����ɶ�άͼ��
         */
        public float Func2D(float x, Func2DEnum func2D)
        {
            // �÷��䶯̬���ݺ���ö�ٻ�ȡ��Ӧ�ĺ�����ִ�У���ȡ�ĺ���������public��
            // Ч�ʱȽϵ�
            Type type = typeof(Function);
            MethodInfo methodInfo = type.GetMethod(func2D.ToString());

            if (methodInfo == null)
            {
                Debug.Log($"��ȡ����{func2D.ToString()}ʧ��");
                return 0;
            }

            return (float)methodInfo.Invoke(Instance, new object[] { x });
        }

        /*
         * ��ά�������ýӿڣ�������άͼ��
         */
        public Vector3 Func3D(float u, float v, Func3DEnum func)
        {
            // ��ί�еķ���ʵ�ָ���ö��ִ�ж�Ӧ���������������Ǿ�̬�ģ���������˽�е�
            if (!funcDic.ContainsKey(func))
            {
                try
                {
                    // ��̬����ö�ٶ�Ӧ�ĺ���ί�У�����ӵ��ֵ���
                    GraphFunction function =
                        (GraphFunction)Delegate.CreateDelegate(typeof(GraphFunction), this, func.ToString());
                    funcDic.Add(func, function);
                }
                catch (ArgumentException)
                {
                    Debug.Log("�޷����غ�����" + func);
                }
            }

            return funcDic[func](u, v);
        }

        /*
         * ����һ�������ĺ���
         */
        public float FuncSin(float x)
        {
            return 2f * Mathf.Sin(pi * (x + Time.time));
        }

        public float FuncSin1(float x)
        {
            float y = Mathf.Sin(pi * (x + Time.time));
            y += Mathf.Sin(2f * pi * (x + 2f * Time.time)) / 2f;
            y /= 1.5f;
            return y;
        }

        public float FuncCos(float x)
        {
            return Mathf.Cos(pi * (x + Time.time));
        }

        /*
         * �������������ĺ���
         */
        private Vector3 FuncMultiSin1(float u, float v)
        {
            Vector3 position = new Vector3 { x = u, y = Mathf.Sin(pi * (u + v + Time.time)), z = v };
            return position;
        }

        private Vector3 FuncMultiSin2(float u, float v)
        {
            Vector3 position = new Vector3();
            position.x = u;

            float y = Mathf.Sin(pi * (u + Time.time));
            y += Mathf.Sin(pi * (v + Time.time));
            y *= 0.5f;
            position.y = y;

            position.z = v;

            return position;
        }

        private Vector3 FuncMultiSin3(float u, float v)
        {
            Vector3 position = new Vector3();
            position.x = u;

            float y = 4f * Mathf.Sin(pi * (u + v + Time.time * 0.5f));
            y += Mathf.Sin(pi * (u + Time.time));
            y += Mathf.Sin(2f * pi * (v + Time.time * 2f)) * 0.5f;
            y /= 5.5f;
            position.y = y;

            position.z = v;

            return position;
        }

        private Vector3 Ripple(float u, float v)
        {
            Vector3 position = new Vector3();
            position.x = u;

            float d = Mathf.Sqrt(u * u + v * v);
            float y = Mathf.Sin(pi * (4f * d - Time.time * 2f));
            y /= 10f * d + 1f;
            position.y = y;

            position.z = v;
            return position;
        }

        private Vector3 Cylinder(float u, float v)
        {
            float radius = 0.8f + Mathf.Sin(pi * (6f * u + 2f * v + Time.time)) * 0.2f;
            Vector3 position = new Vector3();

            position.x = radius * Mathf.Sin(pi * u);
            position.y = v;
            position.z = radius * Mathf.Cos(pi * u);
            return position;
        }

        private Vector3 Sphere(float u, float v)
        {
            Vector3 position = new Vector3();
            float radius = 0.8f + Mathf.Sin(pi * (6f * u + Time.time)) * 0.1f;
            radius += Mathf.Sin(pi * (4f * v + Time.time)) * 0.1f;
            float s = radius * Mathf.Cos(pi * v * 0.5f);

            position.x = s * Mathf.Cos(pi * u);
            position.y = radius * Mathf.Sin(pi * v * 0.5f);
            position.z = s * Mathf.Sin(pi * u);

            return position;
        }

        private Vector3 Torus(float u, float v)
        {
            Vector3 position = new Vector3();
            float r1 = 0.7f + 0.1f * Mathf.Sin(pi * (6f * u + 0.5f * Time.time));
            float r2 = 0.15f + 0.05f * Mathf.Sin(pi * (8f * u + 4f * v + 2f * Time.time));
            float s = r1 + r2 * Mathf.Cos(pi * v);
            position.x = s * Mathf.Sin(pi * u);
            position.y = r2 * Mathf.Sin(pi * v);
            position.z = s * Mathf.Cos(pi * u);

            return position;
        }

        private Vector3 Custom(float u, float v)
        {
            Vector3 position = new Vector3();
            float delta = 1f + Mathf.Sin(pi * (6f * u + 3f * v + Time.time));
            float d = 1f + Mathf.Cos(pi * (v * Time.time));

            position.x = (u + v) * delta;
            position.y = (u - v);
            position.z = u * v * d;

            return position;
        }
    }
}
