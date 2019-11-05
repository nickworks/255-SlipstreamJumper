using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Caughman
{
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
        bool isGrounded = false;

        /// <summary>
        /// Whether or not the player is moving upwards on a jump arc (and holding the jump button).
        /// </summary>
        bool isJumping = false;

        /// <summary>
        /// The current velocity of the player in meters per second.
        /// </summary>
        Vector3 veloctiy = new Vector3();



        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            DoPhysicsVertical();
            DoPhysicsHorizontal();

            //add velocity to position
            transform.position += veloctiy * Time.deltaTime;

            ClampToGroundPlane();
        }//end Update

        private void ClampToGroundPlane()
        {
            //clamp to ground plane: (y=0);
            if (transform.position.y < 0)//player is below the ground:
            {
                Vector3 pos = transform.position;
                pos.y = 0;
                transform.position = pos;

                veloctiy.y = 0;

                isGrounded = true;
            }
            else
            {
                isGrounded = false;
            }
        }//End ClampToGroundPlane

        private void DoPhysicsHorizontal()
        {
            float h = Input.GetAxis("Horizontal");

            veloctiy.x = h * speed;
        }

        private void DoPhysicsVertical()
        {
            if (Input.GetButtonDown("Jump") && isGrounded)//jump was just pressed
            {
                veloctiy.y += jumpImpulse;
                isJumping = true;
            }

            //if not holding jump, cancel jump
            if (!Input.GetButton("Jump")) isJumping = false;
            //if past jump peak, cancel jump
            if (veloctiy.y < 0) isJumping = false;


            //add acceleration to our velocity
            float gravityMultiplier = (isJumping) ? 0.5f : 1;


            veloctiy.y -= gravity * Time.deltaTime;
        }//End DoPhysicsVertical



    }//End Class

}//End Namespace