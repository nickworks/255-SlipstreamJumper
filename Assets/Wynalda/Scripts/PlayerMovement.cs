using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Wynalda
{
    [RequireComponent(typeof(AABB))]
    public class PlayerMovement : MonoBehaviour
    {
        /// <summary>
        /// Speed multiplier for horizontal movement.
        /// </summary>
        public float speed = 5f;

        /// <summary>
        /// The acceleration due to gravity in meters per second square.
        /// </summary>
        public float gravity = 10f;

        /// <summary>
        /// The ammount of force used when jumping
        /// </summary>
        public float jumpImpulse = 5f;

        /// <summary>
        /// Wether or not the player is currently standing on the ground.
        /// </summary>
        bool isGrounded = false;

        /// <summary>
        /// Wether or not the player is moving upwards on a jump arc (and holding the jump button)
        /// </summary>
        bool isJumping = false;

        /// <summary>
        /// The current velocity of the player, measured in meters per second.
        /// </summary>
        Vector3 velocity = new Vector3();

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            DoPhysicsVertical();
            DoPhysicsHortizontal();
            //add players velocity to the players position
            transform.position += velocity * Time.deltaTime;


            //ClampToGroundPlane();
            isGrounded = false;
        }

        private void ClampToGroundPlane()
        {

            //clamp to ground play: (y=0)
            if (transform.position.y < 0)
            {//player is below the ground:
             //transform.position = new Vector3(transform.position.x, 0, transform.position.z);
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

        private void DoPhysicsHortizontal()
        {
            float h = Input.GetAxis("Horizontal");
            velocity.x = h * speed;
        }

        private void DoPhysicsVertical()
        {
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                velocity.y = jumpImpulse;
                isJumping = true;
            }
            //if not holding jump, cancel jump
            if (!Input.GetButton("Jump"))
            {
                isJumping = false;
            }
            //if past jump peak, cancel jump
            if (velocity.y < 0) isJumping = false;
            //add acceleration to our gravity
            float gravityMultiplier = (isJumping) ? 0.5f : 1;
            velocity.y -= gravity * gravityMultiplier * Time.deltaTime;
        }

        public void ApplyFix(Vector3 fix)
        {
            if (fix.x != 0) velocity.x = 0;
            if (fix.y > 0 && velocity.y < 0) velocity.y = 0;
            if (fix.y < 0 && velocity.y > 0) velocity.y = 0;
            if (fix.y > 0) isGrounded = true;
        }

    }
}