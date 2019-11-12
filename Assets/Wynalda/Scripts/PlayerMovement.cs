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
        public float speed = 15f;

        /// <summary>
        /// The acceleration due to gravity in meters per second square.
        /// </summary>
        public float gravity = 10f;

        /// <summary>
        /// The ammount of force used when jumping
        /// </summary>
        public float jumpImpulse = 5f;

        /// <summary>
        /// The Width of the screen clamp
        /// </summary>
        private float objectWidth;
        
        /// <summary>
        /// Whether or not the player is currently standing on the ground.
        /// </summary>
        bool isGrounded = false;

        /// <summary>
        /// Whether or not the player is moving upwards on a jump arc (and holding the jump button)
        /// </summary>
        bool isJumping = false;

        /// <summary>
        /// Whether or not the player is allowed to perform a double jump.
        /// </summary>
        bool doubleJumpAllowed = false; 

        /// <summary>
        /// The current velocity of the player, measured in meters per second.
        /// </summary>
        Vector3 velocity = new Vector3();

        /// <summary>
        /// Clamping the player from leaving the left and right sides of the screen.
        /// </summary>
        private Vector2 screenBounds;

        void Start()
        {
            screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
            objectWidth = transform.GetComponent<SpriteRenderer>().bounds.size.x /2;
        }

        // Update is called once per frame
        void Update()
        {
            DoPhysicsVertical();
            DoPhysicsHortizontal();
            GameOver();
            
            //add players velocity to the players position
            transform.position += velocity * Time.deltaTime;


            //ClampToGroundPlane();
            isGrounded = false;
        }

        void LateUpdate()
        {
            Vector3 viewPos = transform.position;
            viewPos.x = Mathf.Clamp(viewPos.x, (screenBounds.x + objectWidth) * - 1, screenBounds.x - objectWidth); //This clamps the player to the left and right side of the screen.
            transform.position = viewPos;
        }


        /// <summary>
        /// This is used for all the situations where the Game would End!
        /// </summary>
        private void GameOver()
        {
            if (transform.position.x < screenBounds.x * -1)
            {
                Game.GameOver();
               // print("GAME OVER!!!"); // USED TO PROVE THIS WORKS!
            }

            if (transform.position.y < screenBounds.y * -1)
            {
                Game.GameOver();
                print("GAME OVER!!!"); // USED TO PROVE THIS WORKS!
            }
        }

     /*   private void DoubleJump()
        {
            if()
            {
                
        
            }

            else
            {
           
            }
        } */
        

        /// <summary>
        /// This handles the Horiztonal Physics of the game. The speed at which the player moves.
        /// </summary>
        private void DoPhysicsHortizontal()
        {
            float h = Input.GetAxis("Horizontal");
            velocity.x = h * speed;
           
        
        }

        /// <summary>
        /// This handles the Vertical Physics of the game. Manages things such as jumping and gravity.
        /// </summary>
        private void DoPhysicsVertical()
        {
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                velocity.y = jumpImpulse;
                isJumping = true;
                doubleJumpAllowed = true;
                
            }
            //if not holding jump, cancel jump
            if (!Input.GetButton("Jump")) 
            {
                isJumping = false;
          
            }
            if(Input.GetButtonDown("Jump") && doubleJumpAllowed && !isGrounded)
            {
                velocity.y = jumpImpulse;
                doubleJumpAllowed = false;
               // isJumping = true; do i need this here?
            }

            //if past jump peak, cancel jump
            if (velocity.y < 0) isJumping = false;
            //add acceleration to our gravity
            float gravityMultiplier = (isJumping) ? 0.5f : 1;
            velocity.y -= gravity * gravityMultiplier * Time.deltaTime;
        }

        /// <summary>
        /// This applies several needed fixes for the game to function. zeroing out velocity and working with clamp to object. 
        /// </summary>
        /// <param name="fix"></param>
        public void ApplyFix(Vector3 fix)
        {
            if (fix.x != 0) velocity.x = 0;
            if (fix.y > 0 && velocity.y < 0) velocity.y = 0;
            if (fix.y < 0 && velocity.y > 0) velocity.y = 0;
            if (fix.y > 0) isGrounded = true; 
        }

        /// <summary>
        /// This us used to launch the player upward.
        /// </summary>
        public void LaunchUpwards(float upwardVel)
        {
            //print($"launching upwards at a velocity of {upwardVel}"); // USED TO TEST IF MY VALUES I INTEND ARE BEING CORRECTLY USED!
            velocity.y = upwardVel;
        }

    }
}