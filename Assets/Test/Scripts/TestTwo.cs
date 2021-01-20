/*
 * Author: Huai
 * Create: 2021/1/21 2:49:56
 *
 * Description:
 */

using System;
using UnityEngine;

namespace Testing
{
    public class TestTwo : MonoBehaviour
    {
        private void Start()
        {
            AsyncObject asyncObject = Test.Instance.StartEvent();

            asyncObject.OnStart += () => { print("OnStart"); };
            asyncObject.OnUpdate += () => { print("OnUpdate"); };
            asyncObject.OnComplete += () =>
            {
                if (asyncObject.isFinish)
                {
                    print("Is Finish");
                }
            };
        }
    }
}
