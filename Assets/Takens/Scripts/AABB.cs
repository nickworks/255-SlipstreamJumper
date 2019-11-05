using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Takens
{
    public class AABB : MonoBehaviour
    {
        public enum ObjectType {Solid, Passthrough, Spring, Fall, Player}
        public ObjectType currentType = ObjectType.Solid;
        public Vector3 size;
        public bool manual = false;

        public Vector3 min { get; private set; }
        public Vector3 max { get; private set; }
        private GameObject player;

        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        // Update is called once per frame
        void Update()
        {
            Recalc();
        }

        public bool CollidesWith(AABB other)
        {

                                                       //check for gap to left
            if (other.max.x < this.min.x) return false;//no collision
                                                       //check for gap to right
            if (other.min.x > this.max.x) return false;//no collision
                                                       //check for gap to above
            if (other.max.y < this.min.y) return false;//no collision
                                                       //check for gap to below
            if (other.min.y > this.max.y) return false;//no collision

         
           


            //no gaps found, return true
            return true;
        }

        /// <summary>
        /// This function returns how far to move THIS aabb so that it no longer overlaps
        /// another aabb. This function assumes that the two are overlapping.
        /// This function only solves for the X and Y axis. Z axis is ignored!!
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

        public void Recalc()
        {
            Vector3 halfSize = size / 2;

            if (!manual)
            {
                halfSize.x *= transform.localScale.x;
                halfSize.y *= transform.localScale.y;
                halfSize.z *= transform.localScale.z;
            }

            min = transform.position - halfSize;
            max = transform.position + halfSize;
        }

        public void ApplyFix(Vector3 fix)
        {
          
                transform.position += fix;
                Recalc();
           
            
        }

        void OnDrawGizmos()
        {

            if (!manual)
            {
                Vector3 scaledSize = size;
                scaledSize.x *= transform.localScale.x;
                scaledSize.y *= transform.localScale.y;
                scaledSize.z *= transform.localScale.z;
                Gizmos.DrawWireCube(transform.position, scaledSize);
            }
            else
            {
                Gizmos.DrawWireCube(transform.position, size);
            }
        }
    }
}