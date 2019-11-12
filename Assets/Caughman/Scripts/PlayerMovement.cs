using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Caughman
{
    [RequireComponent(typeof(AABB))]
    public class PlayerMovement : MonoBehaviour
    {
        /// <summary>
        /// The speed multiplier for horizontal movement.
        /// </summary>
        public float speed = 10;

        /// <summary>
        /// The acceleration due to gravity in meters per seconds squared.
        /// </summary>
        public float gravity = 10;

        /// <summary>
        /// the amount of force to use when jumping
        /// </summary>
        public float jumpImpulse = 5;


        /// <summary>
        /// whether or not the player is currently standing on the ground.
        /// </summary>
        public bool isGrounded = false;

        /// <summary>
        /// Whether or not the player is moving upwards on a jump arc (and holding the jump button).
        /// </summary>
        public bool isJumping = false;

        /// <summary>
        /// The current velocity of the player in meters per second.
        /// </summary>
       public  Vector3 velocity = new Vector3();



        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void LateUpdate()
        {
            DoPhysicsVertical();
            DoPhysicsHorizontal();

            //add velocity to position
            transform.position += velocity * Time.deltaTime;

            ClampToGroundPlane();
            ClampToSide();
        }//end Update

        private void ClampToGroundPlane()
        {
            //clamp to ground plane: (y=0);
            if (transform.position.y < -5)//player is below the ground:
            {
                Vector3 pos = transform.position;
                pos.y = 5;
                pos.x = -4;
                transform.position = pos;

                velocity.y = 0;

                isGrounded = true;


                
            }
            else
            {
                isGrounded = false;
            }
        }//End ClampToGroundPlane

        private void ClampToSide()
        {
            //clamp to sides of play field: (x=-8);
            if (transform.position.x < -8)//player is too far left:
            {
                Vector3 pos = transform.position;
                pos.x = -8;
                transform.position = pos;

                velocity.x = 0;
            }

            //clamp to sides of play field: (x=8);
            if (transform.position.x >8)//player is too far right:
            {
                Vector3 pos = transform.position;
                pos.x = 8;
                transform.position = pos;

                velocity.x = 0;
            }
        }//End ClampToSide

        private void DoPhysicsHorizontal()
        {
            float h = Input.GetAxis("Horizontal");

            velocity.x = h * speed;
        }

        private void DoPhysicsVertical()
        {
            if (Input.GetButtonDown("Jump") && isGrounded)//jump was just pressed
            {
                velocity.y += jumpImpulse;
                isJumping = true;
            }

            //if not holding jump, cancel jump
            if (!Input.GetButton("Jump")) isJumping = false;
            //if past jump peak, cancel jump
            if (velocity.y < 0) isJumping = false;


            //add acceleration to our velocity
            float gravityMultiplier = (isJumping) ? 0.5f : 1;


            velocity.y -= gravity * Time.deltaTime;
        }//End DoPhysicsVertical


        public void ApplyFix(Vector3 fix)
        {
            if (fix.x != 0) velocity.x = 0;
            if (fix.y != 0) velocity.y = 0;
            if (fix.y > 0) isGrounded = true;
        }//End ApplyFix

        public void SpringUp(float velocityY)
        {
            velocity.y = velocityY;
        }//End Spring Up

        public void SpikeHit()
        {
            Vector3 pos = transform.position;
            pos.y = 5;
            pos.x = -4;
            transform.position = pos;

            velocity.y = 0;
        }
        
    }//End Class

}//End Namespace