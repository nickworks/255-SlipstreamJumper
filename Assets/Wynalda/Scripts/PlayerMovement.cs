using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wynalda
{
    public class PlayerMovement : MonoBehaviour
    {
        /// <summary>
        /// The speec multiplier for horizontal movement.
        /// </summary>
        public float speed = 5;

        /// <summary>
        /// The aceceleration due to gravity.
        /// </summary>
        public float gravity = 20;

        /// <summary>
        /// The amount of force to use when jumping.
        /// </summary>
        public float jumpImpulse = 8;

        /// <summary>
        /// Whether or not the player is currently standing on the ground.
        /// </summary>
        bool isGrounded = false;

        /// <summary>
        /// Whether or not the player is moving upwards on a jump arc (and holding the jump button).
        /// </summary>
        bool isJumping = false;

        /// <summary>
        /// The current velocity of the player, measured in meters / second. 
        /// </summary>
        Vector3 velocity = new Vector3();


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {


            DoPhysicsHorizontal();
            DoPhysicsVertical();
            ClampToGround();

        }// end update

        private void DoPhysicsVertical()
        {
            if (Input.GetButtonDown("Jump") && isGrounded) // jump was just pressed
            {
                velocity.y = jumpImpulse;
                isJumping = true;
            }
            if (!Input.GetButton("Jump"))
            {
                isJumping = false;
            }
            if (velocity.y < 0)
            {
                isJumping = false;
            }

            //add acceleration to our velocity
            float gravityMultiplier = (isJumping) ? 0.5f : 1;
            velocity.y -= gravity * Time.deltaTime * gravityMultiplier;
            //add players velocity to players position
            transform.position += velocity * Time.deltaTime;
        }

        private void DoPhysicsHorizontal()
        {
            float h = Input.GetAxis("Horizontal");
            velocity.x = h * speed;
        }

        private void ClampToGround()
        {
            //clamp to ground plane: (y = 0)
            if (transform.position.y < 0)//below the ground
            {
                //transform.position = new Vector3(transform.position.x, 0, 0); 
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

        public void ApplyFix(Vector3 fix)
        {
            transform.position += fix;
        }

    } // end class
}
