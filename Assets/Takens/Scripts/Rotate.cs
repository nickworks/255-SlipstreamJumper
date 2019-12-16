using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Takens
{
    public class Rotate : MonoBehaviour
    {

        public float speed = 1.5f;
        // Update is called once per frame
        void Update()
        {
            gameObject.transform.RotateAroundLocal(Vector3.up, speed * Time.deltaTime);
        }
    }
}