using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Andrea
{
    public class CameraFollow : MonoBehaviour
    {
        public Transform target;
        public float easing = 2.0f;

        void Start()
        {

        }

        void LateUpdate()
        {
            if (target != null)
            {
                Vector3 newPos = target.position;
                newPos.z = transform.position.z;
                                
                transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * easing);
            }
        }
    }
}