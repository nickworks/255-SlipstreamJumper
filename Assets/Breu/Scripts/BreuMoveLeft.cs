using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Breu
{
    public class BreuMoveLeft : MonoBehaviour
    {
        /// <summary>
        /// the speed VreuMoveLeft objects move left (meter/sec)
        /// </summary>
        public static float speed = 6;

        /// <summary>
        /// unused
        /// </summary>
        void Start()
        {

        }

        /// <summary>
        /// moves objects an amout based on speed per second
        /// </summary>
        void Update()
        {
            transform.position -= new Vector3(speed, 0, 0) * Time.deltaTime;
        }
    }
}