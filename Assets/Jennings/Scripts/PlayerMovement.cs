using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jennings {

    [RequireComponent(typeof(AABB))]
    public class PlayerMovement : MonoBehaviour {

        /// <summary>
        /// The speed multiplier for horizontal movement.
        /// </summary>
        public float speed = 5;

        /// <summary>
        /// The acceleration due to gravity, measured in meters/seconds squared.
        /// </summary>
        public float gravity = 10;

        public float jumpImpulse = 5;

        /// <summary>
        /// Whether or not the player is currently standing on the ground.
        /// </summary>
        bool isGrounded = false;

        /// <summary>
        /// Whether or not the player is moving upwards on a jump arc (and holding the jump button).
        /// </summary>
        bool isJumping = false;

        /// <summary>
        /// The current velocity of the player, measured in meters/second.
        /// </summary>
        public Vector3 velocity = new Vector3();

        bool timeSlowed = false;
        

        void Start()
        {
            
        }

        // Updates PlayerMovement each frame; has timeSlowed originally intended for SlowTime script.
        public void Update()
        {
            DoPhysicsVertical();
            DoPhysicsHorizontal();

            if (timeSlowed == false)
            {
                // add velocity to position
                transform.position += velocity * Time.deltaTime;
            } else if (timeSlowed == true)
            {
                transform.position += velocity * Time.deltaTime * 3f;
            }

            isGrounded = false; 
        }

        /// <summary>
        ///  This portion is intended to make an invisible ground which the player cannot fall through. Intended for use for the SafeMode.
        /// </summary>
        public void ClampToGroundPlane()
        {
            // clamp to ground plane: (y=0)
            if (transform.position.y < 0)
            { // player is below the ground:
          
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
                // jump was pressed
                //transform.position += new Vector3(0, 3, 0);

                velocity.y = jumpImpulse;
                isJumping = true;
            }
            // if not holding jump, cancel jump:
            if (!Input.GetButton("Jump")) isJumping = false;

            // if past jump peak, cancel jump:
            if (velocity.y < 0) isJumping = false;


            // add acceleration to our velocity:
            float gravityMultiplier = (isJumping) ? 0.5f : 1; // if isJumping is true we'll use .5, if else then 1.
            if (isJumping) gravityMultiplier = 0.5f;

            velocity.y -= gravity * Time.deltaTime * gravityMultiplier;
        }

        public void ApplyFix(Vector3 fix)
        {
            if (fix.x != 0) velocity.x = 0;
            if (fix.y != 0) velocity.y = 0;
            if (fix.y > 0) isGrounded = true;
        }


        public void LaunchUpwards(float upwardVel)
        {
            velocity.y = upwardVel;
        }

        
    }
}
