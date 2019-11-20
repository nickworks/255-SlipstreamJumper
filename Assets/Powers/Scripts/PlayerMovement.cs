﻿using UnityEngine;

namespace Powers
{
    [RequireComponent(typeof(AABB))]
    public class PlayerMovement : MonoBehaviour
    {
        //The speed multiplier for horizontal movement
        public float speed = 5;
        //The amount of force applied when jumping
        public float jumpImpulse = 3;
        //The acceleration due to gravity (m/s^2)
        public float gravity = 9.81f;
        // Whether or not the player is currently standing on the ground
        [HideInInspector]
        public bool isGrounded = false;
        // A double checker to ensure stability and polish
        [HideInInspector]
        public bool isGroundedPrev = false;
        //detects if the player has died
        [HideInInspector]
        public bool isDead = false;
        // Used to ensure no one off frames occur where the player cant jump due to minor collison errors
        [HideInInspector]
        public bool hasJumped = false;
        //this controls whether or not the player has the float power enabled
        [HideInInspector]
        public bool hitSpring = false;
        //The current velocity of the player (m/s)
        Vector3 velocity = new Vector3();

        [Space(10)]
        //detects if the player has a slow time powerup
        [HideInInspector]
        public int slowTimePowerup = 0;
        //this is used to multiply the speed once the slow time powerup starts
        [HideInInspector]
        public float slowTimeMultiplier = 1;
        //detects if has a shield powerup
        [HideInInspector]
        public bool shieldPowerup = false;
        //this refers to the shield effect
        public GameObject shieldObject;

        [Space(10)]
        //used to play the jump sfx:
        public AudioSource audioPlayer;
        public AudioSource musicPlayer;
        public AudioClip jumpSFX;

        private AABB playerAABB;

        private void Start()
        {
            playerAABB = gameObject.GetComponent<AABB>();
        }

        void Update()
        {
            //update player if not dead
            if (!isDead)
            {
                DoPhysicsVertical();
                DoPhysicsHorizontal();

                // add velocity to position
                transform.position += velocity * Time.deltaTime;

                isGroundedPrev = isGrounded;

                //if shield powerup is grabbed, activate shield effect
                if (shieldPowerup) shieldObject.SetActive(true);
                else if (!shieldPowerup) shieldObject.SetActive(false);
            }
        }

        private void FixedUpdate()
        {
            //only do the fixed update actions if the player is not dead
            if (!isDead)
            {
                //if slow time powerup obtained or unobtained, change relevant variables
                if (slowTimePowerup != 0)
                {
                    if (Time.timeScale >= 0.5)
                    {
                        Time.timeScale -= 0.01f;
                        musicPlayer.pitch -= 0.01f;
                        audioPlayer.pitch -= 0.01f;
                        slowTimeMultiplier += 0.02f;
                    }
                    slowTimePowerup--;
                }
                else if (slowTimePowerup == 0 && Time.timeScale <= 1)
                {
                    Time.timeScale += 0.01f;
                    musicPlayer.pitch += 0.01f;
                    audioPlayer.pitch += 0.01f;
                    slowTimeMultiplier -= 0.02f;
                }
            }
        }

        private void OnDestroy()
        {
            Time.timeScale = 1;
        }

        private void DoPhysicsHorizontal()
        {
            float h = Input.GetAxis("Horizontal");

            //if time is slowed down, double speed so player can move as normal
            velocity.x = h * speed * slowTimeMultiplier;
        }

        private void DoPhysicsVertical()
        {   
            // jump was just pressed 
            if (Input.GetButtonDown("Jump") && isGrounded || Input.GetButtonDown("Jump") && !isGrounded && !hasJumped || hitSpring)
            {
                //if time is slowed down, multiply by 2 to move normally
                velocity.y = jumpImpulse;

                //play sfx:
                audioPlayer.PlayOneShot(jumpSFX, 0.6f);

                //set variables appropriately
                hitSpring = false;
                hasJumped = true;
                isGrounded = false;
                isGroundedPrev = false;
            }
            //add acceleration due to gravity if player is in air. if time is slowed, multiply by 2 to move normally
            else velocity.y -= gravity * Time.deltaTime;

            //allow player to jump again once they landed on the ground
            if (isGrounded && isGroundedPrev) hasJumped = false;
        }

        public void ApplyFix(Vector3 fix)
        {
            if (fix.x != 0) velocity.x = 0;
            if (fix.y > 0 && velocity.y < 0) velocity.y = 0;
            if (fix.y < 0 && velocity.y > 0) velocity.y = 0;
            if (fix.y > 0) isGrounded = true;
        }

    }
}