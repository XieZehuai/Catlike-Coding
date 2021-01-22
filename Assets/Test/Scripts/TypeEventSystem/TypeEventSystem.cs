/*
 * Author: Huai
 * Create: 2021/1/23 1:51:18
 *
 * Description:
 *      �򵥵������¼�ϵͳ
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Testing.TypeEventSystem
{
    public class TypeEventSystem
    {
        // �Բ���������ΪKey����HashSet��ΪValue������ÿһ�����͵��¼�
        private static readonly Dictionary<Type, HashSet<Delegate>> actions = new Dictionary<Type, HashSet<Delegate>>();

        /// <summary>
        /// ע���¼�
        /// </summary>
        /// <typeparam name="T">��������</typeparam>
        /// <param name="action">��Ӧ�������͵�ί��</param>
        public static void Register<T>(Action<T> action)
        {
            Type type = typeof(T);

            if (!actions.ContainsKey(type))
            {
                Debug.Log("ע���¼�" + action.ToString());
                actions[type] = new HashSet<Delegate> { action };
            }
            else
            {
                if (!actions[type].Contains(action))
                {
                    Debug.Log("ע���¼�" + action.ToString());
                    actions[type].Add(action);
                }
                else
                {
                    Debug.Log("�Ѿ�ע���˸��¼�" + action.ToString());
                }
            }
        }

        /// <summary>
        /// ע���¼�
        /// </summary>
        /// <typeparam name="T">��������</typeparam>
        /// <param name="action">��Ӧ�������͵�ί��</param>
        public static void UnRegister<T>(Action<T> action)
        {
            Type type = typeof(T);

            if (!actions.ContainsKey(type) || !actions[type].Contains(action))
            {
                Debug.Log("û��ע����¼�" + action.ToString());
            }
            else
            {
                Debug.Log("ע���¼�" + action.ToString());
                actions[type].Remove(action);
            }
        }

        /// <summary>
        /// ������Ϣ�������Ѿ�ע��Ķ�Ӧ�������͵�ί��
        /// </summary>
        /// <typeparam name="T">��������</typeparam>
        /// <param name="context">��Ӧ�������͵�����</param>
        public static void Send<T>(T context)
        {
            Type type = typeof(T);

            if (!actions.ContainsKey(type))
            {
                Debug.Log("û��ע������͵��¼�");
                return;
            }

            Debug.Log("������Ϣ" + type.ToString());
            HashSet<Delegate> set = actions[type];
            foreach (Delegate item in set)
            {
                Action<T> action = (Action<T>)item;
                action(context);
            }
        }
    }
}
