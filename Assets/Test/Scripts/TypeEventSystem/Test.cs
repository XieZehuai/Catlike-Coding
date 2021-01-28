/*
 * Author: Huai
 * Create: 2021/1/28 23:11:16
 *
 * Description:
 */

using System;
using UnityEngine;

namespace Testing.TypeEventSystem
{
	public class Test : MonoBehaviour
	{
        private void Start()
        {
            GameObject obj = new GameObject("test obj");
            obj.AddComponent<NewBehaviourScript>();
        }
    }
}
