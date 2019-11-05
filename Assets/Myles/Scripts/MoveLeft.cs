using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Myles
{
    public class MoveLeft : MonoBehaviour
    {
        /// <summary>
        /// How fast to move all MoveLeft objects to the left (in meters/second).
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
