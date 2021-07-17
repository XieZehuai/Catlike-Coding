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

        private new Rigidbody rigidbody;
        private Vector3 velocity; // 当前速度
        private Vector3 desiredVelocity; // 根据移动输入计算得出的目标速度
        private bool desiredJump; // 是否要跳跃（在Update里检测，FixedUpdate里执行）

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            // 获取输入
            Vector2 playerInput;
            playerInput.x = Input.GetAxis("Horizontal");
            playerInput.y = Input.GetAxis("Vertical");
            playerInput = Vector2.ClampMagnitude(playerInput, 1f); // 将输入向量的值限制在1以内
            desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * speed; // 计算目标速度

            desiredJump |= Input.GetButtonDown("Jump");
        }

        private void FixedUpdate()
        {
            velocity = rigidbody.velocity;
            float maxSpeedChange = maxAcceleration * Time.deltaTime; // 计算加速度
            // 让当前速度增加到目标速度
            velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
            velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);

            if (desiredJump)
            {
                desiredJump = false;
                Jump();
            }

            rigidbody.velocity = velocity;
        }

        private void Jump()
        {
            velocity.y += 5f;
        }
    }
}