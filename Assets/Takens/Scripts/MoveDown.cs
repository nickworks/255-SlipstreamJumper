using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Takens
{
    /// <summary>
    /// Moves objects down by a constant rate
    /// </summary>
    public class MoveDown : MonoBehaviour
    {
        /// <summary>
        ///How fast to move all moveLeft objects to the left (meters-per-second)
        /// </summary>
        public static float speed = 2f;

        // Update is called once per frame
        void LateUpdate()
        {
            if (!Game.isPaused)
            {
                //moves the object down by speed (meters) per second
                transform.position += speed * Vector3.down * Time.deltaTime;
            }
        }
    }
}