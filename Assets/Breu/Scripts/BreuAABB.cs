using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Breu
{
    public class BreuAABB : MonoBehaviour
    {


        public Vector3 size;

        public Vector3 Max { get; private set; }

        public Vector3 Min { get; private set; }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per framea
        void Update()
        {
            recalc();
        }

        public bool collidesWith(BreuAABB other)
        {

            //check for gap to the left
            if (other.Max.x < this.Min.x)
            {
                return false;//no collision
            }
            //check for gap to the right
            if (other.Min.x > this.Max.x)
            {
                return false;//no collision
            }
            //check for gap above
            if (other.Min.y > this.Max.y)
            {
                return false;//no collision
            }
            //check for gap below
            if (other.Max.y < this.Min.y)
            {
                return false;//no collision
            }
            //if no gaps are found, return true
            return true;
        }


        /// <summary>
        /// this function returns how far to move THIS aadd so that it no longer overlaps another.
        /// assumed that the two overlap.
        /// only solves overlap in 2D, X-axis & Y-axis.
        /// </summary>
        /// <param name="other"></param>
        /// <returns>how far to move this object in meters</returns>
        public Vector3 findFix(BreuAABB other)
        {
            float moveRight = other.Max.x - this.Min.x;
            float moveLeft = other.Min.x - this.Max.x;
            float moveUp = other.Max.y - this.Min.y;
            float moveDowm = other.Min.y - this.Max.y;

            Vector3 fix = new Vector3();

            fix.x = (Mathf.Abs(moveLeft) < Mathf.Abs(moveRight)) ? moveLeft : moveRight;
            fix.y = (Mathf.Abs(moveUp) < Mathf.Abs(moveDowm)) ? moveUp : moveDowm;
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

        /// <summary>
        /// 
        /// </summary>
        public void recalc()
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

        public void applyFix(Vector3 fix)
        {            
            transform.position += fix;

            recalc();
        }
    }
}