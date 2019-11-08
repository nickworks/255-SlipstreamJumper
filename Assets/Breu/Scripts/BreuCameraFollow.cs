using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Breu
{
    public class BreuCameraFollow : MonoBehaviour
    {
        public Transform Target;//what the camera will follow

        public float easing = 20;//determines how fast the camera will follow

        // Update is called once per frame
        void LateUpdate()
        {
            if (Target != null)
            {
                Vector3 newPos = new Vector3(0, Target.position.y, transform.position.z);//set the camera to follow only on the y axis

                transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * easing);//set camera to "lag" behind target
            }
        }
    }
}