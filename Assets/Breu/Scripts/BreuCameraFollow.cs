using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Breu
{
    public class BreuCameraFollow : MonoBehaviour
    {
        public Transform Target;

        public float easing = 20;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void LateUpdate()
        {
            if (Target != null)
            {

                Vector3 newPos = Target.position;
                newPos.z = transform.position.z;

                //transform.position += (newPos -transform.position) * Time.deltaTime ;

                transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * easing);
            }
        }
    }
}