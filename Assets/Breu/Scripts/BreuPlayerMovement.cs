using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Breu
{
    public class BreuPlayerMovement : MonoBehaviour
    {


        /// <summary>
        /// determines player horizontal (x) speed
        /// </summary>
        public float HoriSpeed = 5;

        /// <summary>
        /// Determines player vertical (y) jump
        /// </summary>
        public float jumpImpulse = 15;

        /// <summary>
        /// determines acceleration due to gravity
        /// </summary>
        public float Gravity = 30;

        /// <summary>
        /// determines if the player is standing on the "ground"
        /// </summary>
        private bool isGrounded = false;

        /// <summary>
        /// determines if the player is moving upwards during a jump
        /// </summary>
        bool isJumping = false;

        /// <summary>
        /// Current velocity of the player, meters per seconds (m/s)
        /// </summary>
        Vector3 PlayerVelocity = new Vector3();



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
            transform.position += PlayerVelocity * Time.deltaTime;

            ClampToGroundPlane();

        }

        private void DoPhysicsHorizontal()
        {
            float h = Input.GetAxis("Horizontal");
            PlayerVelocity.x = h * HoriSpeed;
        }

        private void DoPhysicsVertical()
        {
            if (Input.GetButtonDown("Jump") && isGrounded == true)//jump button pressed
            {
                PlayerVelocity.y = jumpImpulse;
                isJumping = true;
            }
            //if not holding jump buttons cancel jump
            if (!Input.GetButton("Jump"))
            {
                isJumping = false;
            }
            //if past jump peak, cancel jump
            if (PlayerVelocity.y < 0)
            {
                isJumping = false;
            }
            //add acceleration to out velocity
            float gravMultiplyer = 1;
            if (isJumping == true)
            {
                gravMultiplyer = 0.5f;
            }
            PlayerVelocity.y -= Gravity * Time.deltaTime * gravMultiplyer;
        }

        private void ClampToGroundPlane()
        {
            //clamp to ground plane: (y=0)
            if (transform.position.y < 0)
            {
                Vector3 pos = transform.position;
                pos.y = 0;
                transform.position = pos;

                PlayerVelocity.y = 0;

                isGrounded = true;
            }
            else
            {
                isGrounded = false;
            }
        }


    }
}