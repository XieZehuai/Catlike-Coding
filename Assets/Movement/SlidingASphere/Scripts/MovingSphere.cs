using System;
using UnityEngine;

namespace Movement
{
    public class MovingSphere : MonoBehaviour
    {
        [Header("小球最大移动速度")]
        [SerializeField, Range(0f, 100f)] private float speed = 10f;
        [Header("小球在地面移动时的最大加速度")]
        [SerializeField, Range(0f, 100f)] private float maxAcceleration = 10f;
        [Header("小球在空中移动时的最大加速度")]
        [SerializeField, Range(0f, 100f)] private float maxAirAcceleration = 0f;
        [Header("小球跳跃高度")]
        [SerializeField, Range(0f, 10f)] private float jumpHeight = 2f;
        [Header("在空中连续跳跃的次数（不包括在地面的跳跃）")]
        [SerializeField, Range(0, 5)] private int maxAirJump = 0;
        [Header("地面最大倾斜角度（小于这个角度的视为地面）")]
        [SerializeField, Range(0f, 90f)] private float maxGroundAngle = 40f;

        private new Rigidbody rigidbody;
        private Vector3 velocity; // 当前速度
        private Vector3 desiredVelocity; // 根据移动输入计算得出的目标速度
        private bool desiredJump; // 是否要跳跃（在Update里检测，FixedUpdate里执行）
        private bool isGrounded;
        private int jumpPhase;
        private float minGroundDotProduct;
        private Vector3 contactNormal;

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
            OnValidate();
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
            UpdateState();
            AdjustVelocity();

            if (desiredJump)
            {
                desiredJump = false;
                Jump();
            }

            rigidbody.velocity = velocity;
            ClearState();
        }

        private void UpdateState()
        {
            velocity = rigidbody.velocity;
            if (isGrounded)
            {
                jumpPhase = 0;
                contactNormal.Normalize();
            }
            else
            {
                contactNormal = Vector3.up;
            }
        }

        private void ClearState()
        {
            isGrounded = false;
            contactNormal = Vector3.zero;
        }

        private void AdjustVelocity()
        {
            Vector3 xAxis = ProjectOnContactPlane(Vector3.right).normalized;
            Vector3 zAxis = ProjectOnContactPlane(Vector3.forward).normalized;

            float currentX = Vector3.Dot(velocity, xAxis);
            float currentZ = Vector3.Dot(velocity, zAxis);

            float acceleration = isGrounded ? maxAcceleration : maxAirAcceleration;
            float maxSpeedChange = acceleration * Time.deltaTime;

            float newX = Mathf.MoveTowards(currentX, desiredVelocity.x, maxSpeedChange);
            float newZ = Mathf.MoveTowards(currentZ, desiredVelocity.z, maxSpeedChange);

            velocity += xAxis * (newX - currentX) + zAxis * (newZ - currentZ);
        }

        private void Jump()
        {
            if (isGrounded || jumpPhase < maxAirJump)
            {
                jumpPhase++;

                // 速度-位移公式：初速度 V0，末速度 vt，加速度 a，位移 s
                // vt^2 - v0^2 = 2 * a * s
                float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
                float alignedSpeed = Vector3.Dot(velocity, contactNormal);
                if (alignedSpeed > 0f)
                {
                    jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
                }

                velocity += contactNormal * jumpSpeed;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            EvaluateCollision(collision);
        }

        private void OnCollisionStay(Collision collision)
        {
            EvaluateCollision(collision);
        }

        private void EvaluateCollision(Collision collision)
        {
            for (int i = 0; i < collision.contactCount; i++)
            {
                Vector3 normal = collision.GetContact(i).normal;
                if (normal.y >= minGroundDotProduct)
                {
                    isGrounded = true;
                    contactNormal += normal;
                }
            }
        }

        private Vector3 ProjectOnContactPlane(Vector3 vector)
        {
            return vector - contactNormal * Vector3.Dot(vector, contactNormal);
        }

        private void OnValidate()
        {
            minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
        }
    }
}