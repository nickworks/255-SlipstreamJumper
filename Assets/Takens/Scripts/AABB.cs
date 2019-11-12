using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Takens
{
    /// <summary>
    /// Adds a AABB collision box to the object
    /// </summary>
    public class AABB : MonoBehaviour
    {
        /// <summary>
        /// Enum to define the type of AABB
        /// Determines how the interaction should be handeled
        /// </summary>
        public enum ObjectType {Solid, Passthrough, Player}

        /// <summary>
        /// The current type of AABB object, set in the inspector
        /// </summary>
        public ObjectType currentType = ObjectType.Solid;

        /// <summary>
        /// Size of AABB box
        /// </summary>
        public Vector3 size;

        /// <summary>
        /// Setting for disabling the auto resizing
        /// </summary>
        public bool manual = false;

        /// <summary>
        /// Minimum values for x y z
        /// </summary>
        public Vector3 min { get; private set; }

        /// <summary>
        /// Maximum values for x y z
        /// </summary>
        public Vector3 max { get; private set; }

        /// <summary>
        /// Reference to the player, set by the Start() function
        /// </summary>
        private GameObject player;

        //Called once at start, gets the player
        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        // Update is called once per frame
        void Update()
        {
            Recalc();
        }

        /// <summary>
        /// Checks for collision with another AABB
        /// </summary>
        /// <param name="other"></param>
        /// <returns>wether or not the AABB is colliding with 'other'</returns>
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
            //get the distances for each direction
            float moveRight = other.max.x - this.min.x;
            float moveLeft = other.min.x - this.max.x;
            float moveUp = other.max.y - this.min.y;
            float moveDown = other.min.y - this.max.y;


            //check which direction is the shortest

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
        /// <summary>
        /// Updates the min and max values each frame
        /// </summary>
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
        /// <summary>
        /// Add the calculated translation values when a collision is detected
        /// Then Recalc's
        /// </summary>
        /// <param name="fix"></param>
        public void ApplyFix(Vector3 fix)
        {
                transform.position += fix;
                Recalc();
            
        }
        /// <summary>
        /// Draws the box of the AABB in the editor
        /// </summary>
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