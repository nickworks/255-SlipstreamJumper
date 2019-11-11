using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Caughman
{
    public class AABB : MonoBehaviour
    {

        public Vector3 size;

        public Vector3 min { get; private set; }
        public Vector3 max { get; private set; }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            Recalc();
        }

        
        /// <summary>
        /// checks for gaps in the distance between the min and max of each x and y edges of AABB objects
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool CollidesWith(AABB other)
        {
            //check for gap to left
            if (other.max.x < this.min.x) return false;
            //check for gap to right
            if (other.min.x > this.max.x) return false;
            //check for gap above
            if (other.min.y > this.max.y) return false;
            //check for gap below
            if (other.max.y < this.min.y) return false;

            //no gaps found, return true
            return true;
        }

        /// <summary>
        /// This function returns how far to move THIS aabb so that it no longer overlaps
        /// another AABB.  This function assumes that the two are overlapping.  Only solves for X and Y for Axis.
        /// </summary>
        /// <param name="other"></param>
        /// <returns>How far to move this box in meters</returns>
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

        public void Recalc()
        {
            Vector3 halfSize = size / 2;

            halfSize.x *= transform.localScale.x;
            halfSize.y *= transform.localScale.y;
            halfSize.z *= transform.localScale.z;

            min = transform.position - halfSize;
            max = transform.position + halfSize;
        }

        public void ApplyFix(Vector3 fix)
        {
            transform.position += fix;
            Recalc();

        }//End ApplyFix

        private void OnDrawGizmos()
        {
            Vector3 scaledSize = size;

            scaledSize.x *= transform.localScale.x;
            scaledSize.y *= transform.localScale.y;
            scaledSize.z *= transform.localScale.z;

            Gizmos.DrawWireCube(transform.position, scaledSize);
        }//End OnDrawGizmos

    }
}
