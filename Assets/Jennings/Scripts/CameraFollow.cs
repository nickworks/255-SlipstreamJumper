using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Jennings {
    public class CameraFollow : MonoBehaviour {

        public Transform target;
        public float easing = 2;

        void Start()
        {

        }

        // Update is called once per frame
        void LateUpdate()
        {
            if(target != null)
            {
                Vector3 newPos = target.position;
                newPos.z = transform.position.z;

                //transform.position += (newPos - transform.position) * Time.deltaTime;

                //Does the exact same thing as the line above
                transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * easing);
            }
        }
    }
}
