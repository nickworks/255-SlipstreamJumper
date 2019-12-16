﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jennings {

    public class AABB : MonoBehaviour {
        // size of the AABB box
        public Vector3 size;

        public Vector3 min { get; private set; }
        public Vector3 max { get; private set; }



        void Start()
        {

        }


        void Update()
        {
            Recalc(); // Recalculates to find a gap
        }

        // Finds collisions by searching for a gap between the two objects. If one is found they are not touching.
        public bool CollidesWith(AABB other)
        {

            // check for gap to left:
            if (other.max.x < this.min.x) return false; // no collision!
                                                        // check for gap to right:
            if (other.min.x > this.max.x) return false; // no collision!
                                                        // check for gap above:
            if (other.min.y > this.max.y) return false; // no collision!
                                                        // check for gap below:
            if (other.max.y < this.min.y) return false; // no collision!



            // no gaps found
            return true;
        }
        /// <summary>
        /// This function returns how far to move THIS aabb so that it no longer overlaps
        /// another aabb. This function assumes that the two are overlapping.
        /// This function only solves for the X or Y axis. Z axis is ignored!!
        /// </summary>
        /// <param name="other"></param>
        /// <returns>How far to move this box (in meters).</returns>
        public Vector3 FindFix(AABB other)
        {
            float moveRight = other.max.x - this.min.x;
            float moveLeft = other.min.x - this.max.x;
            float moveUp = other.max.y - this.min.y;
            float moveDown = other.min.y - this.max.y;

            Vector3 fix = new Vector3();

            fix.x = (Mathf.Abs(moveLeft) < Mathf.Abs(moveRight)) ? moveLeft : moveRight;
            fix.y = (Mathf.Abs(moveUp) < Mathf.Abs(moveDown)) ? moveUp : moveDown;

            if (Mathf.Abs(fix.x) < Mathf.Abs(fix.y))
            {
                fix.y = 0;
            }
            else
            {
                fix.x = 0;
            }

            return fix;
        }
        // Recalculate to get the size of te respective object
        public void Recalc()
        {
            Vector3 halfSize = size / 2;

            halfSize.x *= transform.localScale.x;
            halfSize.y *= transform.localScale.y;
            halfSize.z *= transform.localScale.z;

            min = transform.position - halfSize;
            max = transform.position + halfSize;


        }

        // Creates the little box to be seen in viewport
        void OnDrawGizmos()
        {

            Vector3 scaledSize = size;

            scaledSize.x *= transform.localScale.x;
            scaledSize.y *= transform.localScale.y;
            scaledSize.z *= transform.localScale.z;

            Gizmos.DrawWireCube(transform.position, scaledSize);
        }
        // Applies fix to transformations moved and triggers recalculation.
        public void ApplyFix(Vector3 fix)
        {
            transform.position += fix;
            Recalc();

        }
    }
}
