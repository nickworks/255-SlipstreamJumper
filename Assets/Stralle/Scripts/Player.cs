using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    /// <summary>
    /// The speed multiplier for horizontal movement.
    /// </summary>
    public float speed = 5;

    /// <summary>
    /// The acceleration due to gravity,  measured in meters / second squared.
    /// </summary>
    public float gravity = 10;

    /// <summary>
    /// The amound of force to use when jumping.
    /// </summary>
    public float jumpImpulse = 5;

    /// <summary>
    /// Whether or now the player is currently standing on the ground.
    /// </summary>
    bool isGrounded = false;

    /// <summary>
    /// Whether or not the player is moving upwards on a jump arc (and holding the jump button.)
    /// </summary>
    bool isJumping = false;

    /// <summary>
    /// The current velocity of the player, measured in meters / second.
    /// </summary>
    Vector3 velocity = new Vector3();


    void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetButtonDown("Jump") && isGrounded)
        { // jump was just pressed:
            // transform.position += new Vector3(0, 3, 0);
            velocity.y += jumpImpulse;
            isJumping = true;
        }
        // if not holding jump, cancel jump:
        if (!Input.GetButton("Jump")) isJumping = false;
        // if past jump peak, cancel jump:
        if (velocity.y < 0) isJumping = false;


        // add acceleration to our velocity:
        float gravityMultiplier = (isJumping) ? 0.5f : 1;
        // if (isJumping) gravityMultiplier = 0.5f;

        velocity.y -= gravity * Time.deltaTime * gravityMultiplier;

        float h = Input.GetAxis("Horizontal");
        velocity.x = h * speed;

        // add velocity to position
        transform.position += velocity * Time.deltaTime;

        // clamp to ground plane: (y=0)
        if (transform.position.y < 0)
        { // player is below the ground:
            // transform.position = new Vector3(transform.position.x, 0, 0)
            Vector3 pos = transform.position;
            pos.y = 0;
            transform.position = pos;

            velocity.y = 0;

            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

    }
}
