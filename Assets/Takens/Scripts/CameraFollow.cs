using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Takens
{
    /// <summary>
    /// Makes the Camera follow the player when the player gets to high on the stage
    /// </summary>
    public class CameraFollow : MonoBehaviour
    {
        /// <summary>
        /// The transform of the player to follow with the camera
        /// </summary>
        public Transform target;

        // Update is called once per frame
        void Update()
        {
            if (target != null)
            {

                Vector3 newPos = transform.position; 
                if (target.position.y > transform.position.y + 5f)

                //if players y pos is above the cameras y pos, move it up by a factor of 80% the difference per second
                newPos.y += (target.position.y- transform.position.y)* .8f * Time.deltaTime;

                //set new pos
                transform.position = newPos;
            }
        }
    }
}