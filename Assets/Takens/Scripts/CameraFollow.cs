using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Takens
{
    public class CameraFollow : MonoBehaviour
    {
        public Transform target;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (target != null)
            {

                Vector3 newPos = transform.position;
                if(target.position.y > transform.position.y)
                newPos.y += (target.position.y- transform.position.y)*1f * Time.deltaTime;


                transform.position = newPos;
            }
        }
    }
}