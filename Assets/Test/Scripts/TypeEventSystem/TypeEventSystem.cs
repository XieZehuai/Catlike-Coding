/*
 * Author: Huai
 * Create: 2021/1/23 1:51:18
 *
 * Description:
 *      简单的类型事件系统
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Testing.TypeEventSystem
{
    public class TypeEventSystem
    {
        // 以参数类型作为Key，用HashSet作为Value，保存每一种类型的事件
        private static readonly Dictionary<Type, HashSet<Delegate>> actions = new Dictionary<Type, HashSet<Delegate>>();

        /// <summary>
        /// 注册事件
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="action">对应参数类型的委托</param>
        public static void Register<T>(Action<T> action)
        {
            Type type = typeof(T);

            if (!actions.ContainsKey(type))
            {
                Debug.Log("注册事件" + action.ToString());
                actions[type] = new HashSet<Delegate> { action };
            }
            else
            {
                if (!actions[type].Contains(action))
                {
                    Debug.Log("注册事件" + action.ToString());
                    actions[type].Add(action);
                }
                else
                {
                    Debug.Log("已经注册了该事件" + action.ToString());
                }
            }
        }

        /// <summary>
        /// 注销事件
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="action">对应参数类型的委托</param>
        public static void UnRegister<T>(Action<T> action)
        {
            Type type = typeof(T);

            if (!actions.ContainsKey(type) || !actions[type].Contains(action))
            {
                Debug.Log("没有注册该事件" + action.ToString());
            }
            else
            {
                Debug.Log("注销事件" + action.ToString());
                actions[type].Remove(action);
            }
        }

        /// <summary>
        /// 发送消息给所有已经注册的对应参数类型的委托
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="context">对应参数类型的数据</param>
        public static void Send<T>(T context)
        {
            Type type = typeof(T);

            if (!actions.ContainsKey(type))
            {
                Debug.Log("没有注册该类型的事件");
                return;
            }

            Debug.Log("发送消息" + type.ToString());
            HashSet<Delegate> set = actions[type];
            foreach (Delegate item in set)
            {
                Action<T> action = (Action<T>)item;
                action(context);
            }
        }
    }
}
