using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Petzak
{
    /// <summary>
    /// A pill that randomly spawns above the player and falls down.
    /// When picked up by player it gives an extra life. 
    /// Protects from spike and fall damage.
    /// </summary>
    [RequireComponent(typeof(PetzakAABB))]
    public class PetzakLifePill : MonoBehaviour
    {
        /// <summary>
        /// Whether or not the pill is falling
        /// </summary>
        public bool isFalling = true;
        /// <summary>
        /// Whether or not the pill is visible.
        /// </summary>
        public bool isVisible = false;
        /// <summary>
        /// Time (in frames) for the next pill to spawn. Spawns once it reaches 0.
        /// </summary>
        public int spawnTime;
        /// <summary>
        /// The acceleration due to gravity, measured in meters / second squared.
        /// </summary>
        public float gravity = 2;
        /// <summary>
        /// The velocity at which the pill is moving
        /// </summary>
        public Vector3 velocity = new Vector3();

        /// <summary>
        /// Spawn the pill above the player and reset properties.
        /// </summary>
        /// <param name="pos"></param>
        public void Spawn(Vector3 pos)
        {
            spawnTime = Random.Range(10, 20) * 60; // 10 to 20 seconds
            int r = Random.Range(15, 30); // distance ahead of player
            velocity = new Vector3();
            transform.position = new Vector3(pos.x + r, pos.y + 20, pos.z);
            var mr = GetComponentInParent<MeshRenderer>();
            isFalling = isVisible = mr.enabled = true; // disable mesh renderer
        }

        /// <summary>
        /// Hide the pill and reset velocity when it's touched by player.
        /// </summary>
        public void Pickup()
        {
            if (!isVisible)
                return;
            var mr = GetComponentInParent<MeshRenderer>();
            isVisible = mr.enabled = false;
            velocity = new Vector3();
        }

        /// <summary>
        /// Reduce spawn time and drop pill every frame
        /// </summary>
        void Update()
        {
            // reduce spawn counter after pill is picked up
            if (!isVisible)
                spawnTime--;

            // drop pill
            if (isFalling && isVisible)
            {
                // add acceleration to falling velocity
                float gravityMultiplier = (isFalling) ? 0.5f : 1;
                velocity.y -= gravity * Time.deltaTime * gravityMultiplier;
                transform.position += velocity * Time.deltaTime;
            }
        }
    }
}