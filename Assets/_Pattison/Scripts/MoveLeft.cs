using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pattison
{
    public class MoveLeft : MonoBehaviour
    {
        /// <summary>
        /// How fast to move all MoveLeft objects to the left (in meters-per-second).
        /// </summary>
        public static float speed = 5;


        void Start() {

        }

        
        void Update() {
            transform.position -= new Vector3(speed, 0, 0) * Time.deltaTime;
            //transform.position += speed * Vector3.left * Time.deltaTime;
        }
    }
}