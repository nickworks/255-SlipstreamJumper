using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;

namespace Breu
{
    [RequireComponent(typeof(BreuAABB))]
    public class BreuPlayerMovement : MonoBehaviour
    {
        //Time.timeScale allows you to mess with how fast/slow time.deltatime is

        /// <summary>
        /// determines player horizontal (x) speed
        /// </summary>
        public float HorizontalSpeed = 5;

        /// <summary>
        /// player speed multiplyer for when the player is moving left
        /// </summary>
        public float LeftSpeedMult = 1.75f;

        /// <summary>
        /// Determines player vertical (y) jump
        /// </summary>
        public float jumpImpulse = 15;

        /// <summary>
        /// determines acceleration due to gravity
        /// </summary>
        public float Gravity = 30;

        /// <summary>
        /// Determines the amount of time that the pickup lasts
        /// </summary>
        public float PickUpTimer = 30;
        float PickUpTimeRemaining;

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

        private Vector2 ScreeBounds;
        
        void Start()
        {
            PickUpTimeRemaining = 0;
            isGroundPounding = false;
            ScreeBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        }

        void Update()
        {
            DoPhysicsVertical();//determines Vertical movement

            DoPhysicsHorizontal();//determines horizontal movement
            
            transform.position += PlayerVelocity * Time.deltaTime;//add velocity to position

            ClampToRightPlane();

            isGrounded = false;//assumes player is not on gorund

            if (PickUpTimeRemaining > 0)
            {
                isGrounded = true;
                PickUpTimeRemaining -= Time.deltaTime;
            }    
        }

        private void DoPhysicsHorizontal()
        {
            float h = Input.GetAxis("Horizontal");//get if player is pressing left/right on an axis between -1 & 1
            PlayerVelocity.x = h * HorizontalSpeed;//movs player left/right according to Horispeed
            if (h < 0)
            {
                PlayerVelocity.x = h * LeftSpeedMult * HorizontalSpeed;
            }
        }

        private void DoPhysicsVertical()
        {
            //if player presses groundpound button player "ground-pounds" until they hit ground
            if (Input.GetButtonDown("Fire3"))
            {
                isGroundPounding = true;                
            }

            //acceleration multiplier for ground-pound
            float groundPoundMult = 1;//defaults to 1 for no change

            //ground-pound button pressed
            if (isGroundPounding == true)
            {
                groundPoundMult = groundPoundSpeed;//change groundpoundmult to speed set in editor
                PlayerVelocity.x = 0;//keeps player from moving left/right when ground pounding. should be move to horizontal physics
            }
            else
            {
                groundPoundMult = 1;//reset groundpoundmult
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

        /// <summary>
        /// keep the player from going off screen to the right.
        /// </summary>
        private void ClampToRightPlane()
        {
                Vector3 pos = transform.position;
                pos.x = Mathf.Clamp(pos.x, ScreeBounds.x * -2, ScreeBounds.x);
                transform.position = pos;

                PlayerVelocity.x = 0;
        }


        /// <summary>
        /// moves player out of platforms determine by "fix" vector3
        /// </summary>
        /// <param name="fix"></param>
        public void applyFix(Vector3 fix)
        {
            if(fix.x != 0)//stop player from gaing infinite left/right velocity
            {
                PlayerVelocity.x = 0;
            }
            if (fix.y != 0 && PlayerVelocity.y < 0)//stops player from gaining infinite up/down velocity
            {
                PlayerVelocity.y = 0;
            }
            if (fix.y > 0)//set isgrounded to true if the player is placed ontop of a platform
            {
                isGrounded = true;
                isGroundPounding = false;
            }
        }

        /// <summary>
        /// called when player collides with a spring
        /// </summary>
        /// <param name="yVelocity"></param>
        public void launchUpwards(float yVelocity)
        {
            
            PlayerVelocity.y = yVelocity;//set players up/down velocity 
            isGroundPounding = false;//cancels ground-pound
        }
        
        /// <summary>
        /// sets the amount of time the player can use the pickup
        /// </summary>
        public void PickUpGet()
        {
            PickUpTimeRemaining = PickUpTimer;
        }

    }
}