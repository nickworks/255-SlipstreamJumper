using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Breu
{
    [RequireComponent(typeof(BreuAABB))]
    public class BreuPlayerMovement : MonoBehaviour
    {
        //Time.timeScale allows you to mess with how fast/slow time.deltatime is

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
        /// determines acceleration due to groundpound
        /// </summary>
        public float groundPoundSpeed = 100;

        /// <summary>
        /// determines if player is groundpounding
        /// </summary>
        public static bool  isGroundPounding = false;

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

            //ClampToGroundPlane();
            isGrounded = false;

        }

        private void DoPhysicsHorizontal()
        {
            float h = Input.GetAxis("Horizontal");
            PlayerVelocity.x = h * HoriSpeed;
        }

        private void DoPhysicsVertical()
        {
            //if player presses groundpound button player "ground-pounds" until they hit ground
            if (Input.GetButtonDown("Fire3") && isGrounded == false)
            {
                isGroundPounding = true;                
            }
            else if (isGrounded == true)
            {
                isGroundPounding = false;
            }
            //add acceleration for ground-pound
            float groundPoundMult = 1;
            //ground-pound button pressed
            if (isGroundPounding == true)
            {
                groundPoundMult = groundPoundSpeed;
                PlayerVelocity.x = 0;
            }
            else
            {
                groundPoundMult = 1;
            }

            //jump button pressed
            if (Input.GetButtonDown("Jump") && isGrounded == true)
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
            
            PlayerVelocity.y -= Gravity * Time.deltaTime * gravMultiplyer * groundPoundMult;

            
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

        public void applyFix(Vector3 fix)
        {
            if(fix.x != 0)
            {
                PlayerVelocity.x = 0;
            }
            if (fix.y != 0 && PlayerVelocity.y < 0)
            {
                PlayerVelocity.y = 0;
            }
            if (fix.y > 0)
            {
                isGrounded = true;
            }
        }
    }
}