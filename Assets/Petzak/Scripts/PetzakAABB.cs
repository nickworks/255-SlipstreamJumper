using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Petzak
{
    /// <summary>
    /// Axis-Aligned Bounding Box used for collisions of all game objects.
    /// </summary>
    public class PetzakAABB : MonoBehaviour
    {
        /// <summary>
        /// Size of the object.
        /// </summary>
        public Vector3 size;
        /// <summary>
        /// Minimum x and y values.
        /// </summary>
        public Vector3 min;
        /// <summary>
        /// Maximum x and y values.
        /// </summary>
        public Vector3 max;

        /// <summary>
        /// Recalc min and max every frame.
        /// </summary>
        void Update()
        {
            Recalc();
        }

        /// <summary>
        /// Determines if there are any gaps between one AABB and another.
        /// Only accounts for x and y axis.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool CollidesWith(PetzakAABB other)
        {
            // LightPlatform collision against player
            if (other.name.StartsWith("Light") && !this.name.StartsWith("Life"))
            {
                Recalc(); // make sure both objects have the correct min/max
                other.Recalc();
                PetzakPlayerMovement p = this.GetComponentInParent<PetzakPlayerMovement>();
                bool closeEnough = Math.Abs(other.max.y - this.min.y) < .2; // objects are very close to one another
                bool withinHorizontalRange = other.min.x <= this.max.x && other.max.x >= this.min.x;
                bool isNotMovingUp = p.velocity.y <= 0;
                return closeEnough && withinHorizontalRange && isNotMovingUp && !p.isJumping && !p.isGrounded;
            }

            if (other.max.x < this.min.x // check for gap to left
             || other.min.x > this.max.x // check for gap to right
             || other.min.y > this.max.y // check for gap above
             || other.max.y < this.min.y) // check for gap below
                return false;

            return true; // no gaps found
        }

        /// <summary>
        /// This function returns how far to move this AABB so that it no longer overlaps
        /// another AABB. This function assumes that the two are overlapping.
        /// This function only solves for the X or Y axies. Z is ignored.
        /// </summary>
        /// <param name="other"></param>
        /// <returns>How far to move this box (in meters).</returns>
        public Vector3 FindFix(PetzakAABB other)
        {
            float moveRight = other.max.x - this.min.x;
            float moveLeft = other.min.x - this.max.x;
            float moveUp = other.max.y - this.min.y;
            float moveDown = other.min.y - this.max.y;

            Vector3 fix = new Vector3();

            fix.x = (Mathf.Abs(moveLeft) < Mathf.Abs(moveRight)) ? moveLeft : moveRight;
            fix.y = (Mathf.Abs(moveUp) < Mathf.Abs(moveDown)) ? moveUp : moveDown;

            if (Mathf.Abs(fix.x) < Mathf.Abs(fix.y))
                fix.y = 0;
            else
                fix.x = 0;

            return fix;
        }

        /// <summary>
        /// Sets the minimum and maximum bounds of an AABB based on it's size.
        /// </summary>
        public void Recalc()
        {
            Vector3 halfSize = size / 2;

            halfSize.x *= transform.localScale.x;
            halfSize.y *= transform.localScale.y;
            halfSize.z *= transform.localScale.z;

            min = transform.position - halfSize;
            max = transform.position + halfSize;
        }

        /// <summary>
        /// Adjusts the position of the AABB so that it's not overlapping other AABB's.
        /// Then recalculates the min/max.
        /// </summary>
        /// <param name="fix"></param>
        public void ApplyFix(Vector3 fix)
        {
            transform.position += fix;
            Recalc();
        }
    }
}