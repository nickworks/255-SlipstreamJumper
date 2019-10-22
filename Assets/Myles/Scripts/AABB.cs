using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Myles
{
    public class AABB : MonoBehaviour
    {

        public Vector3 size;

        public Vector3 min { get; private set; }
        public Vector3 max { get; private set; }

        void Start()
        {

        }


        void LateUpdate()
        {
            Recalc();
        }

        public bool CollidesWith(AABB other)
        {
            //checked for gap to left:
            if (other.max.x < this.min.x) return false; // no collision!
            //checked for gap to right:
            if (other.min.x > this.max.x) return false; // no collision!
            //checked for gap above:
            if (other.min.y > this.max.y) return false; // no collision!
            //checked for gap below:
            if (other.max.y < this.min.y) return false; // no collision!
            



            // no gaps found, return true:
            return true;
        }

        /// <summary>
        /// This function returns how far to move THIS AABB so that it no longer overlaps
        /// another AABB. This function assumes that the two are overlapping.
        /// This function only solves for the X or Y axis. Z axis is ignored!!!
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
            if(Mathf.Abs(fix.x) < Mathf.Abs(fix.y))
            {
                fix.y = 0;

            } else
            {
                fix.x = 0;
            }
        }


        void Recalc()
        {
            Vector3 halfSize = size / 2;

            min = transform.position - halfSize;
            max = transform.position + halfSize;

        }

        public void ApplyFix(Vector3 fix)
        {
            transform.position += fix;

        }

        void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position, size);
        }
    }
}
