using System;
using UnityEngine;

namespace Movement
{
    public class MovingSphere : MonoBehaviour
    {
        [Header("小球最大移动速度")]
        [SerializeField, Range(0f, 100f)] private float speed = 10f;
        [Header("小球最大加速度")]
        [SerializeField, Range(0f, 100f)] private float maxAcceleration = 10f;
        [Header("小球可移动区域")]
        [SerializeField] private Rect allowedArea = new Rect(-5f, -5f, 10f, 10f);
        [Header("小球碰到边界后反弹力量的大小")]
        [SerializeField, Range(0f, 1f)] private float bounciness = 0.5f;

        private Vector3 velocity;

        private void Update()
        {
            // 获取输入
            Vector2 playerInput;
            playerInput.x = Input.GetAxis("Horizontal");
            playerInput.y = Input.GetAxis("Vertical");
            playerInput = Vector2.ClampMagnitude(playerInput, 1f); // 将输入向量的值限制在1以内

            Vector3 desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * speed; // 计算目标速度
            float maxSpeedChange = maxAcceleration * Time.deltaTime; // 计算加速度
            // 让当前速度增加到目标速度
            velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
            velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);
            Vector3 displacement = velocity * Time.deltaTime; // 计算位移

            Vector3 newPosition = transform.localPosition + displacement; // 计算移动后的新位置

            if (newPosition.x < allowedArea.xMin)
            {
                newPosition.x = allowedArea.xMin;
                velocity.x = -velocity.x * bounciness;
            }
            else if (newPosition.x > allowedArea.xMax)
            {
                newPosition.x = allowedArea.xMax;
                velocity.x = -velocity.x * bounciness;
            }

            if (newPosition.z < allowedArea.yMin)
            {
                newPosition.z = allowedArea.yMin;
                velocity.z = -velocity.z * bounciness;
            }
            else if (newPosition.z > allowedArea.yMax)
            {
                newPosition.z = allowedArea.yMax;
                velocity.z = -velocity.z * bounciness;
            }

            transform.localPosition = newPosition; // 应用移动
        }
    }
}