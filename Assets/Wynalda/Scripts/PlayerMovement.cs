using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// this script is used for everything concerning to the players movement!
/// </summary>

namespace Wynalda
{
    [RequireComponent(typeof(AABB))]
    public class PlayerMovement : MonoBehaviour
    {
        /// <summary>
        /// Speed multiplier for horizontal movement.
        /// </summary>
        public float speed = 7f;

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

        //things that happen at the start of the game
        void Start()
        {
            //sets up boundaries of the screen for clamping
            screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
            objectWidth = transform.GetComponent<SpriteRenderer>().bounds.size.x /2;
        }

        // Update is called once per frame
        void Update()
        {
            //calling in the voids that need to be updated and checked!
            DoPhysicsVertical();
            DoPhysicsHortizontal();
            GameOver();
            //print(speed); used for testing speed powerup!
            
            //add players velocity to the players position
            transform.position += velocity * Time.deltaTime;

            //is the player on the ground?
            isGrounded = false;
        }

        //this LateUpdate function was added to clamp the player to the right side of the screen. the -2 makes it so it only clamps right, as i want the player to die if they go off the right!
        void LateUpdate()
        {
            //view position
            Vector3 viewPos = transform.position;
            //This clamps the player to the right side of the screen.
            viewPos.x = Mathf.Clamp(viewPos.x, (screenBounds.x + objectWidth) * - 2, screenBounds.x - objectWidth); 
            //set position to view position.
            transform.position = viewPos;
        }


        /// <summary>
        /// This is used for all the situations where the Game would End!
        /// </summary>
        private void GameOver()
        {
            if (transform.position.x < screenBounds.x * -1)
            {
               //if player falls off screen he dies
                Game.GameOver();
              //  print("GAME OVER!!! XX"); // USED TO PROVE THIS WORKS!
            }

            //if player falls off screen he dies
            if (transform.position.y < screenBounds.y * -1)
            {
                Game.GameOver();
               // print("GAME OVER!!! YY"); // USED TO PROVE THIS WORKS!
            }
        }

        /// <summary>
        /// This handles the Horizontal physics of the game. Manages things such as movement speed.
        /// </summary>
        private void DoPhysicsHortizontal()
        {
            //horiztonal controls
            float h = Input.GetAxis("Horizontal");
            //horizontal speed
            velocity.x = h * speed;
           
        
        }

        /// <summary>
        /// This handles the Vertical Physics of the game. Manages things such as jumping and gravity.
        /// </summary>
        private void DoPhysicsVertical()
        {
            //if jump button is pressed when the player is grounded do the following
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                //jump!
                velocity.y = jumpImpulse;
                //declares u are jumping
                isJumping = true;
                //allows u to double jump
                doubleJumpAllowed = true;
                
            }
            //if not holding jump, cancel jump
            if (!Input.GetButton("Jump")) 
            {
                isJumping = false;
          
            }
            //If jump is pressed while u can doublejump and are not on the ground you can jump again!
            if(Input.GetButtonDown("Jump") && doubleJumpAllowed && !isGrounded)
            {
                velocity.y = jumpImpulse;
                doubleJumpAllowed = false;
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

        //Speed boost!
        public void GoFast(float speedIncrease)
        {
            //calls the IEumerator which sets the players speed to 30 then back to 7.
            StartCoroutine("SpeedBoost");
        }
        IEnumerator SpeedBoost()
        {
            //speed to 30.
            speed = 30f;
            //5 second timer.
            yield return new WaitForSeconds(5.0f);
            //speed back to 7.
            speed = 7f;
        }

        //This void is called when the player collides with a hazard. It ends the game!
        public void Death()
        {
            Game.GameOver();
            //print("YOU DIED!"); used for testing
        }



    }
}