using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Andrea
{
    [RequireComponent(typeof(AABB))]
    public class PlayerMovement : MonoBehaviour
    {
        /// <summary>
        /// The speed multiplier for horizontal movement.
        /// </summary>
        public float speed = 5;

        /// <summary>
        /// The amount of force applied when jumping.
        /// </summary>
        public float jumpImpulse = 12;

        /// <summary>
        /// The acceleration due to gravity (m/s^2)
        /// </summary>
        public float gravity = 25;

        /// <summary>
        /// Whether or not the player is currently standing on the ground.
        /// </summary>
        bool isGrounded = false;

        /// <summary>
        /// Whether or not the player is moving upwards on a jump arc (the spacebar is held down).
        /// </summary>
        bool isJumping = false;

        /// <summary>
        /// The current velocity of the player (m/s).
        /// </summary>
        Vector3 velocity = new Vector3();

        void Start()
        {
            
        }
        void Update()
        {
            DoPhysicsVertical();
            DoPhysicsHorizontal();

            // add velocity to position
            transform.position += velocity * Time.deltaTime;

            //ClampToGroundPlane();
            isGrounded = false;
        }

        private void ClampToGroundPlane()
        {
            // clamp to ground plane: (y=0)
            if (transform.position.y < 0)
            // player is below the ground:
            {
                Vector3 pos = transform.position;
                pos.y = 0;
                transform.position = pos;

                velocity.y = 0;

                isGrounded = true;
            }
            else
            {
                isGrounded = false;
            }
        }

        private void DoPhysicsHorizontal()
        {
            float h = Input.GetAxis("Horizontal");
            velocity.x = h * speed;

        }

        private void DoPhysicsVertical()
        {
            if (Input.GetButtonDown("Jump") && isGrounded)
            // jump was just pressed:
            {
                velocity.y = jumpImpulse;
                isJumping = true;
            }
            // if not holding jump, cancel jump:
            if (!Input.GetButton("Jump"))
            {
                isJumping = false;
            }
            // if past jump peak, cancel jump:
            if (velocity.y < 0)
            {
                isJumping = false;
            }

            //add acceleration due to gravity
            float gravityMultiplier = (isJumping) ? 0.5f : 1f;
            velocity.y -= gravity * Time.deltaTime * gravityMultiplier;
        }

        public void ApplyFix(Vector3 fix)
        {
            if (fix.x != 0)
            {
                velocity.x = 0;
            }
            if (fix.y > 0 && velocity.y < 0)
            {
                velocity.y = 0;
            }
            if (fix.y < 0 && velocity.y > 0)
            {
                velocity.y = 0;
            }
            if (fix.y > 0) 
            {
                isGrounded = true;
            }
        }

        public void LaunchUpwards(float upwardVelocity)
        {
            velocity.y = upwardVelocity;
        }

    }
}