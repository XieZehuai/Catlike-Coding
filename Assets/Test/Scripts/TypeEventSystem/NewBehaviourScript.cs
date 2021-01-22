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
    public struct OnUpdate
    {
        public string message;
    }

    public struct OnStart
    {
        public int id;
    }


    public class NewBehaviourScript : MonoBehaviour
    {
        [SerializeField] private string message = "Hello World";
        [SerializeField] private int id = 10086;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                TypeEventSystem.Register<OnUpdate>(OnUpdate);
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                TypeEventSystem.Register<OnStart>(OnStart);
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                TypeEventSystem.UnRegister<OnUpdate>(OnUpdate);
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                TypeEventSystem.UnRegister<OnStart>(OnStart);
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                TypeEventSystem.Send(new OnUpdate { message = message });
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                TypeEventSystem.Send(new OnStart { id = id });
            }
        }

        private void OnUpdate(OnUpdate context)
        {
            Debug.Log("Invoke OnUpdate, message: " + context.message);
        }

        private void OnStart(OnStart context)
        {
            Debug.Log("Invoke OnStart, id: " + context.id);
        }
    }
}
