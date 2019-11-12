using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Takens
{
    /// <summary>
    /// Class for doing player motion/physics, added to player object
    /// </summary>
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
        public float gravity = 12f;

        /// <summary>
        /// The ammount of force used when jumping
        /// </summary>
        public float jumpImpulse = 10f;

        /// <summary>
        /// Wether or not the player is currently standing on the ground.
        /// </summary>
        bool isGrounded = false;

        /// <summary>
        /// Whether or not the player is moving upwards on a jump arc (and holding the jump button)
        /// </summary>
        bool isJumping = false;

        /// <summary>
        /// Is the player able to double jump if they did have enough jumps left
        /// </summary>
        bool canDoubleJump = true;

        /// <summary>
        /// Ammount of double jumps left
        /// </summary>
        [HideInInspector]
        public int doubleJumpsLeft = 3;

        /// <summary>
        /// The current velocity of the player, measured in meters per second.
        /// </summary>
        [HideInInspector]
        public Vector3 velocity = new Vector3();

        /// <summary>
        /// The reference to the text that displays how many jumps are left
        /// </summary>
        public Text jumpsLeft;

        // Update is called once per frame
        void Update()
        {
            if (!Game.isPaused)
            {
                DoPhysicsVertical();
                DoPhysicsHortizontal();
                //add players velocity to the players position
                transform.position += velocity * Time.deltaTime;

                //update display text
             //   jumpsLeft.text = ("Double Jumps Left: " + doubleJumpsLeft);

                //ClampToGroundPlane();
                isGrounded = false;
            }
        }
        /// <summary>
        /// Sets horiztonal movement of player base on input axis
        /// </summary>
        private void DoPhysicsHortizontal()
        {
            float h = Input.GetAxis("Horizontal");
            velocity.x = h * speed;
        }

        /// <summary>
        /// Handles jumping, grounded, and gravity
        /// </summary>
        private void DoPhysicsVertical()
        {
            //if the jump button is pressed
            if (Input.GetButtonDown("Jump") )
            {
                if (isGrounded)//initial jump
                {
                    velocity.y = jumpImpulse;
                    isJumping = true;
                    
                }
                else if(canDoubleJump && doubleJumpsLeft > 0)//double jump
                {
                    velocity.y = jumpImpulse * .9f;
                    isJumping = true;
                    canDoubleJump = false;
                    doubleJumpsLeft -= 1;
                }
            }
            //if not holding jump, cancel jump
            if (!Input.GetButton("Jump"))
            {
                isJumping = false;
            }
            //if past jump peak, cancel jump
            if (velocity.y < 0) isJumping = false;
            //add acceleration to our gravity
            float gravityMultiplier = (isJumping) ? 0.8f : 1;
            velocity.y -= gravity * gravityMultiplier * Time.deltaTime;
        }
        /// <summary>
        /// Neutralizes the players velocity if they are running into a wall
        /// Moves player down with platforms if they are grounded to prevent hopping
        /// </summary>
        /// <param name="fix"></param>
        public void ApplyFix(Vector3 fix)
        {
            if (fix.x != 0) velocity.x = 0;
            if (fix.y > 0 && velocity.y < 0) velocity.y = 0;
            if (fix.y < 0 && velocity.y > 0) velocity.y = 0;
            if (fix.y > 0)//for collisions with the player resting on top of the platform
            {
                isGrounded = true;
                canDoubleJump = true;
                transform.position += 2 * Vector3.down * Time.deltaTime;
            }
        }
        

        /// <summary>
        /// Function called by collision with springs to launch the player upwwards
        /// </summary>
        public void LaunchUpwards()
        {
            if (velocity.y < 0)//if the player is falling onto the spring, have them bounce higher
            {
                velocity.y *= -.3f;
                velocity.y += 20f;
            }
            else
            {
                velocity.y = 20f;//other wise set their vertical velocity to 20 m/s
            }
        }

    }
}