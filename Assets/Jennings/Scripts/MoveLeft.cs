using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jennings {
    public class MoveLeft : MonoBehaviour {

        /// <summary>
        /// The speed to move all MoveLeft objects to the left (meters-per-second).
        /// </summary>
        public static float speed = 5;


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            transform.position -= new Vector3(speed, 0, 0) * Time.deltaTime;
        }
    }
}
