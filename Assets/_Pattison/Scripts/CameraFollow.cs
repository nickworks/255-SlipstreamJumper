using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pattison
{
    public class CameraFollow : MonoBehaviour
    {

        public Transform target;
        public float easing = 2;

        void Start() {

        }

        
        void LateUpdate() {
            if(target != null) {

                Vector3 newPos = target.position;
                newPos.z = transform.position.z;

                //transform.position += (newPos - transform.position) * Time.deltaTime;
                transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * easing);

            }
        }
    }
}