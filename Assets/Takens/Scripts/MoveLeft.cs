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
        public static float speed = 5f;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            //transform.position -= new Vector3(speed, 0, 0) * Time.deltaTime;
            transform.position += speed * Vector3.left * Time.deltaTime;
        }
    }
}