/*
 * Author: Huai
 * Create: 2021/1/23 3:33:33
 *
 * Description:
 */

using System;
using UnityEngine;

namespace Testing.TypeEventSystem
{
    public class NewBehaviourScript : MonoBehaviour
    {
        private void Awake()
        {
            print("awake");
        }

        private void Start()
        {
            print("start");
        }

        private void Update()
        {
            print("update");
        }

        private void FixedUpdate()
        {
            print("fixed update");
        }

        private void OnDestroy()
        {
            print("destroy");
        }
    }
}
