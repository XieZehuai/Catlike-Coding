/*
 * Author: Huai
 * Create: 2021/1/14 2:36:19
 *
 * Description:
 */

using System;
using UnityEngine;

namespace Testing
{
    public class MonoCube : MonoBehaviour
    {
        public float speed = 5f;

        private int horizontal;
        private int vertical;

        private void Update()
        {
            horizontal = 0;
            vertical = 0;

            if (Input.GetKey(KeyCode.D))
            {
                horizontal += 1;
            }
            if (Input.GetKey(KeyCode.A))
            {
                horizontal -= 1;
            }
            if (Input.GetKey(KeyCode.W))
            {
                vertical += 1;
            }
            if (Input.GetKey(KeyCode.S))
            {
                vertical -= 1;
            }

            float deltaTime = Time.deltaTime;
            transform.position += new Vector3(horizontal * speed * deltaTime, vertical * speed * deltaTime, 0f);
        }
    }
}
