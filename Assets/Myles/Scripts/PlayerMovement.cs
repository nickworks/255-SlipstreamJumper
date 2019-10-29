using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Myles {
    [RequireComponent(typeof(AABB))]
    public class PlayerMovement : MonoBehaviour
    {
        /// <summary>
        /// The speed multiplier for horizontal movement.
        /// </summary>
        public float speed = 5;

        /// <summary>
        /// The acceleration due to gravity, measured in meters / second squared.
        /// </summary>
        public float gravity = 10;

        /// <summary>
        /// The amount of force to use when jumping;
        /// </summary>
        public float jumpImpulse = 5;


        /// <summary>
        /// Whether or not the player is currently standing on the ground.
        /// </summary>
        private bool isGrounded = false;

        /// <summary>
        /// Whether or not the player is movng upwards on a jump arc (and holding the jump button).
        /// </summary>
        bool isJumping = false;


        /// <summary>
        /// The current velocity of the player, measured in meters / second.
        /// </summary>
        Vector3 velocity = new Vector3();

        
        void Start()
        {
            //Time.timeScale = 0.5f;
        }


        void Update()
        {
            DoPhysicsVertical();
            DoPhysicsHorizontal();

            //add velocity to position:
            transform.position += velocity * Time.deltaTime;

            //ClampToGroundPlane();
            isGrounded = false;

        }

        private void ClampToGroundPlane()
        {
            // clamp to ground plane: (y=0)
            if (transform.position.y < 0)
            {  // player is below the ground:
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
            {
                // jump was just pressed:
                velocity.y = jumpImpulse;
                isJumping = true;
            }
            //if not holding jemp, cancel jum:
            if (!Input.GetButton("Jump")) isJumping = false;
            // if past jump peak, cancel jump:
            if (velocity.y < 0) isJumping = false;


            // add acceleration to our velocity:
            float gravityMultiplier = (isJumping) ? 0.5f : 1;


            velocity.y -= gravity * Time.deltaTime * gravityMultiplier;

            // add velocity to position
            transform.position += velocity * Time.deltaTime;
        }


        public void ApplyFix(Vector3 fix)
        {
            if (fix.x != 0) velocity.x = 0;
            if (fix.y != 0) velocity.y = 0;
            if (fix.y > 0) isGrounded = true;
        }

    }
}
