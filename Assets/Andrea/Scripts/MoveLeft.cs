using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Andrea
{
    public class MoveLeft : MonoBehaviour
    {
        /// <summary>
        /// How fast all MoveLeft objects are to move to the left (m/s).
        /// </summary>
        public static float speed = 5;
        void Start()
        {

        }

        void Update()
        {
            transform.position += speed * Vector3.left * Time.deltaTime;
        }
    }
}