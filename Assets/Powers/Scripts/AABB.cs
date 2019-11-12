using UnityEngine;

namespace Powers
{
    public class AABB : MonoBehaviour
    {

        public Vector3 size;

        public Vector3 Min { get; private set; }
        public Vector3 Max { get; private set; }

        [HideInInspector]
        public bool collison = false;

        void Update()
        {
            Recalc();
        }

        public bool CollidesWith(AABB other)
        {
            
            // check for gap to the left
            if (other.Max.x < Min.x) return false; // no collision
            // check for gap to the right
            else if (other.Min.x > Max.x) return false; // no collision
            // check for gap above
            else if (other.Min.y > Max.y) return false; // no collision
            // check for gap below
            else if (other.Max.y < Min.y) return false; // no collision
            // if no gaps are found, return true
            else return true; //collision!
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
            float moveRight = other.Max.x - Min.x;
            float moveLeft = other.Min.x - Max.x;
            float moveUp = other.Max.y - Min.y;
            float moveDowm = other.Min.y - Max.y;

            Vector3 fix = new Vector3
            {
                x = (Mathf.Abs(moveLeft) < Mathf.Abs(moveRight)) ? moveLeft : moveRight,
                y = (Mathf.Abs(moveUp) < Mathf.Abs(moveDowm)) ? moveUp : moveDowm
            };

            if (Mathf.Abs(fix.x) < Mathf.Abs(fix.y)) fix.y = 0;
            else fix.x = 0;
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

        public void ApplyFix(Vector3 fix)
        {
            transform.position += fix;
            Recalc();
        }

        void OnDrawGizmos()
        {
            Vector3 scaledSize = size;

            scaledSize.x *= transform.localScale.x;
            scaledSize.y *= transform.localScale.y;
            scaledSize.z *= transform.localScale.z;

            Gizmos.DrawWireCube(transform.position, scaledSize);
        }
    }
}
