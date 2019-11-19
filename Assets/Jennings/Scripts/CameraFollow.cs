using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Jennings {
    public class CameraFollow : MonoBehaviour {
        //Makes it so a target can be assigned to camera as well as apply easing
        public Transform target;
        public float easing = 2;

        void Start()
        {

        }

        // Update is called once per frame
        void LateUpdate()
        {
            //Targets respective object
            if(target != null)
            {
                Vector3 newPos = target.position;
                newPos.z = transform.position.z;
                
                transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * easing);
            }
        }
    }
}
