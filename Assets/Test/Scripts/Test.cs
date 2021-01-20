/*
 * Author: Huai
 * Create: 2021/1/21 2:39:53
 *
 * Description:
 */

using System;
using System.Collections;
using UnityEngine;

namespace Testing
{
    public class AsyncObject
    {
        public bool isFinish;
        public Action OnStart;
        public Action OnUpdate;
        public Action OnComplete;
    }


    public class Test : MonoBehaviour
    {
        public static Test Instance;

        private void Awake()
        {
            Instance = this;
        }

        public AsyncObject StartEvent()
        {
            AsyncObject asyncObject = new AsyncObject();

            StartCoroutine(DoStart(asyncObject));

            return asyncObject;
        }

        private IEnumerator DoStart(AsyncObject asyncObject)
        {
            yield return null;

            asyncObject.OnStart?.Invoke();

            for (int i = 0; i < 10; i++)
            {
                Debug.Log("i " + i);
                asyncObject.OnUpdate?.Invoke();
                yield return null;
            }

            asyncObject.isFinish = true;
            asyncObject.OnComplete?.Invoke();
        }
    }
}
