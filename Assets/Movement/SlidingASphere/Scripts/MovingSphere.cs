using System;
using UnityEngine;

namespace Movement
{
    public class MovingSphere : MonoBehaviour
    {
        [SerializeField, Range(0f, 100f)] private float speed = 10f;

        private void Update()
        {
            Vector2 playerInput;
            playerInput.x = Input.GetAxis("Horizontal");
            playerInput.y = Input.GetAxis("Vertical");
            playerInput = Vector2.ClampMagnitude(playerInput, 1f);

            Vector3 velocity = new Vector3(playerInput.x, 0f, playerInput.y) * speed;
            Vector3 displacement = velocity * Time.deltaTime;

            transform.localPosition += displacement;
        }
    }
}