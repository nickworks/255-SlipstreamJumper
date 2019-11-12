using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Petzak
{
    /// <summary>
    /// Controls movement of player (speed, gravity, jumping, double jumps)
    /// </summary>
    [RequireComponent(typeof(PetzakAABB))]
    public class PetzakPlayerMovement : MonoBehaviour
    {
        /// <summary>
        /// The speed multiplier for horizontal movement.
        /// </summary>
        public float speed = 8;
        /// <summary>
        /// The acceleration due to gravity, measured in meters / second squared.
        /// </summary>
        public float gravity = 10;
        /// <summary>
        /// The amount of force to use when jumping.
        /// </summary>
        public float jumpImpulse = 7;
        /// <summary>
        /// Whether or not the player is currently standing on the ground.
        /// </summary>
        public bool isGrounded = false;
        /// <summary>
        /// Whether or not the player is moving upwards on a jump arc (and holding the jump button).
        /// </summary>
        public bool isJumping = false;
        /// <summary>
        /// Whether or not the player can double jump.
        /// </summary>
        public bool canDoubleJump = false;
        /// <summary>
        /// Whether or not the double jump bar should be reset.
        /// </summary>
        public bool resetBar = false;
        /// <summary>
        /// The current velocity of the player, measured in meters / second.
        /// </summary>
        public Vector3 velocity = new Vector3();

        /// <summary>
        /// Double the timescale on startup
        /// </summary>
        void Start()
        {
            Time.timeScale = 2.0f;
        }

        /// <summary>
        /// Do horizontal/vertical physics and move player each frame
        /// </summary>
        void Update()
        {
            DoPhysicsVertical();
            DoPhysicsHorizontal();           
            transform.position += velocity * Time.deltaTime; // add velocity to position
            isGrounded = false;
        }

        /// <summary>
        /// Sets horizontal velocity.
        /// </summary>
        private void DoPhysicsHorizontal()
        {
            float h = Input.GetAxis("Horizontal");
            velocity.x = h * speed;
        }

        /// <summary>
        /// Sets vertical velocity. Handles player jumping and double jumping.
        /// </summary>
        private void DoPhysicsVertical()
        {
            if (Input.GetButtonDown("Jump"))
            {
                if (isGrounded) // jump 1
                {
                    velocity.y = jumpImpulse;
                    isJumping = true;
                }
                else if (canDoubleJump) // jump 2
                {
                    velocity.y = jumpImpulse + 3; // a little higher
                    canDoubleJump = false;
                    resetBar = true; // checked in the Zone to reset the double jump bar
                }
            }

            // if not holding jump or past jump peak, cancel jump
            if (!Input.GetButton("Jump") || velocity.y < 0)
                isJumping = false;

            // add acceleration to our velocity
            float gravityMultiplier = isJumping ? 0.5f : 1;
            velocity.y -= gravity * Time.deltaTime * gravityMultiplier;
        }

        /// <summary>
        /// Clears horizontal/vertical velocity if objects are touching and grounds objects
        /// </summary>
        /// <param name="fix"></param>
        public void ApplyFix(Vector3 fix)
        {
            if (fix.x != 0)
                velocity.x = 0;
            if (fix.y != 0 && !isJumping)
                velocity.y = 0;
            if (fix.y > 0)
                isGrounded = true;
        }

        /// <summary>
        /// Sets vertical velocity upwards
        /// </summary>
        /// <param name="upwardVel"></param>
        public void LaunchUpwards(float upwardVel)
        {
            velocity.y = upwardVel;
        }
    }
}