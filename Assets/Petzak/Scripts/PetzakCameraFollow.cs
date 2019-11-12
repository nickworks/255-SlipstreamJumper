using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Petzak
{
    /// <summary>
    /// Allows the main camera to follow the player.
    /// </summary>
    public class PetzakCameraFollow : MonoBehaviour
    {
        /// <summary>
        /// The object to follow (player)
        /// </summary>
        public Transform target;
        /// <summary>
        /// The speed at which the camera moves to the target.
        /// </summary>
        public float easing = 2;

        /// <summary>
        /// Moves the position of the camera towards the target.
        /// </summary>
        void LateUpdate()
        {
            if (target == null)
                return;
            Vector3 newPos = target.position;
            newPos.z = transform.position.z;
            transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * easing);
        }
    }
}