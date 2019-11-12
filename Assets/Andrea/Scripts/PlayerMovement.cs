using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Andrea
{
    [RequireComponent(typeof(AABB))]
    public class PlayerMovement : MonoBehaviour
    {
        /// <summary>
        /// The speed multiplier for horizontal movement.
        /// </summary>
        public float speed = 5;

        /// <summary>
        /// The amount of force applied when jumping.
        /// </summary>
        public float jumpImpulse = 12;

        /// <summary>
        /// The acceleration due to gravity (m/s^2)
        /// </summary>
        public float gravity = 25;

        /// <summary>
        /// Whether or not the player is currently standing on the ground.
        /// </summary>
        bool isGrounded = false;

        /// <summary>
        /// Whether or not the player is moving upwards on a jump arc (the spacebar is held down).
        /// </summary>
        bool isJumping = false;

        /// <summary>
        /// The current velocity of the player (m/s).
        /// </summary>
        public Vector3 velocity = new Vector3();

        /// <summary>
        /// The amount of force applied when using boost.
        /// </summary>
        public float boostImpulse = 3;

        /// <summary>
        /// Reference to the slider component for the player fuel.
        /// </summary>
        public Slider fuelBar;

        /// <summary>
        /// The current amount of fuel the player has.
        /// </summary>
        private float fuelAmount = 0;

        /// <summary>
        /// The maximum amount of fuel the player is allowed to have.
        /// </summary>
        private float maxFuel = 100;


        void Start()
        {
            fuelBar.value = CalculateFuel(); // Set the fuel slider to the current fuel.
        }
        void Update()
        {
            DoPhysicsVertical();
            DoPhysicsHorizontal();

            // add velocity to position
            transform.position += velocity * Time.deltaTime;

            //ClampToGroundPlane();
            isGrounded = false;
        }

        /// <summary>
        /// Manages physics and input on the x-axis.
        /// </summary>
        private void DoPhysicsHorizontal()
        {
            float h = Input.GetAxis("Horizontal");
            velocity.x = h * speed;

        }

        /// <summary>
        /// Manages physics and input on the y-axis.
        /// </summary>
        private void DoPhysicsVertical()
        {
            if (Input.GetButtonDown("Jump") && isGrounded)
            // jump was just pressed:
            {
                velocity.y = jumpImpulse;
                isJumping = true;
            }
            // if not holding jump, cancel jump:
            if (!Input.GetButton("Jump"))
            {
                isJumping = false;
            }
            // if past jump peak, cancel jump:
            if (velocity.y < 0)
            {
                isJumping = false;
            }

            // boost was just pressed:
            if (Input.GetButton("Fire3") && fuelAmount > 0)
            {
                ConsumeFuel(.5f); // The amount of fuel to be removed from the player each frame boost is held.
                velocity.y = boostImpulse; // Sets player velocity on the y-axis to the boost value.
                isJumping = false;
            }

            //add acceleration due to gravity:
            float gravityMultiplier = (isJumping) ? 0.5f : 1f;
            velocity.y -= gravity * Time.deltaTime * gravityMultiplier;
        }

        public void ApplyFix(Vector3 fix)
        {
            if (fix.x != 0) // The player needs to move left or right to resolve collision.
            {
                velocity.x = 0; // Zero out the players x-axis velocity before resolving collision.
            }
            if (fix.y > 0 && velocity.y < 0) // The player needs to move up to resolve collision, but has a negative y velocity.
            {
                velocity.y = 0; // Zero out the players y-axis velocity before resolving collision.
            }
            if (fix.y < 0 && velocity.y > 0) // The player needs to move down to resolve collision, but is has a positive y velocity.
            {
                velocity.y = 0; // Zero out the players y-axis velocity before resolving collision.
            }
            if (fix.y > 0) // The player is standing on the ground.
            {
                isGrounded = true;
            }
        }

        /// <summary>
        /// Sets player y-axis velocity to the provided value.
        /// </summary>
        /// <param name="upwardVelocity"></param>
        public void LaunchUpwards(float upwardVelocity)
        {
            velocity.y = upwardVelocity;
        }


        /// <summary>
        /// Adds fuel needed for boosting.
        /// </summary>
        /// <param name="fuelValue"></param>
        public void AddFuel(float fuelValue)
        {
            fuelAmount += fuelValue;
            if (fuelAmount > maxFuel)
            {
                fuelAmount = maxFuel;
            }
            fuelBar.value = CalculateFuel();
        }

        /// <summary>
        /// Consumes fuel from player.
        /// </summary>
        /// <param name="fuelCost"></param>
        public void ConsumeFuel(float fuelCost)
        {
            fuelAmount -= fuelCost;
            if (fuelAmount < 0)
            {
                fuelAmount = 0;
            }
            fuelBar.value = CalculateFuel();
        }

        /// <summary>
        /// The amount of fuel (%) the player has.
        /// </summary>
        /// <returns></returns>
        float CalculateFuel()
        {
            return fuelAmount / maxFuel;
        }

    }
}