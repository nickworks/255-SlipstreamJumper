using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Takens
{

    public class MoveLeft : MonoBehaviour
    {
        /// <summary>
        ///How fast to move all moveLeft objects to the left (meters-per-second)
        /// </summary>
        public static float speed = 2f;

        // Start is called before the first frame update
        void Start() { 
        }

        // Update is called once per frame
        void Update()
        {
            //for moving left
            //transform.position += speed * Vector3.left * Time.deltaTime;

            transform.position += speed * Vector3.down * Time.deltaTime;

        }
    }
}