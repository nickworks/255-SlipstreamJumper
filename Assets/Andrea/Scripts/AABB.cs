using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Andrea
{
    public class AABB : MonoBehaviour
    {
        public Vector3 size;

        public Vector3 Min { get; private set; }
        public Vector3 Max { get; private set; }

        void Start()
        {

        }

        void Update()
        {
            Recalc();
        }


        //void LateUpdate()
        //{
            //Recalc();
        //}

        public bool CollidesWith(AABB other)
        {
            // check for gap to the left:
            if (other.Max.x < this.Min.x)
            {
                return false; // no collision
            }

            // check for gap to the right
            if (other.Min.x > this.Max.x)
            {
                return false; // no collision
            }

            // check for gap above
            if (other.Min.y > this.Max.y)
            {
                return false; // no collision
            }

            // check for gap below
            if (other.Max.y < this.Min.y)
            {
                return false; // no collision
            }

            // if no gaps are found, return true:
            return true;
        }

        /// <summary>
        /// This function returns how far to move this AABB so that it no longer overlaps
        /// another AABB. This function assumes that the two are overlapping.
        /// This function only solves for the X or Y axies. Z is ignored.
        /// </summary>
        /// <param name="other"></param>
        /// <returns>How far to move this box (in meters).</returns>
        public Vector3 FindFix(AABB other)
        {
            float moveRight = other.Max.x - this.Min.x;
            float moveLeft = other.Min.x - this.Max.x;
            float moveUp = other.Max.y - this.Min.y;
            float moveDown = other.Min.y - this.Max.y;

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

        public void Recalc()
        {
            Vector3 halfSize = size / 2;

            halfSize.x *= transform.localScale.x;
            halfSize.y *= transform.localScale.y;
            halfSize.z *= transform.localScale.z;

            Min = transform.position - halfSize;
            Max = transform.position + halfSize;

        }

        void OnDrawGizmos()
        {
            Vector3 scaledSize = size;
            scaledSize.x *= transform.localScale.x;
            scaledSize.y *= transform.localScale.y;
            scaledSize.z *= transform.localScale.z;

            Gizmos.DrawWireCube(transform.position, scaledSize);
        }
        
        public void ApplyFix(Vector3 fix)
        {
            transform.position += fix;
            Recalc();
        }
    }
}