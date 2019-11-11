using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Caughman
{
    public class MoveLeft : MonoBehaviour
    {
        /// <summary>
        /// how fast to move all MoveLeft Objects to the left in Meters per second
        /// </summary>
        public static float speed = 5;

        void Start()
        {

        }

        void Update()
        {
            transform.position -= new Vector3(speed, 0, 0) * Time.deltaTime;
        }
    }
}
