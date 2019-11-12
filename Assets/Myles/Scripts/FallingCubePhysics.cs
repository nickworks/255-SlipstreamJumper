using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Myles
{
    [RequireComponent(typeof(AABB))]
    public class FallingCubePhysics : MonoBehaviour
    {
        List<GameObject> cubes = new List<GameObject>();

        public GameObject fallingCube;

        /// <summary>
        /// The acceleration due to gravity, measured in meters / second squared.
        /// </summary>
        public float gravity = 10;

        /// <summary>
        /// The current velocity of the cube, measured in meters / second.
        /// </summary>
        Vector3 velocity = new Vector3();


        void Start()
        {
            //Time.timeScale = 0.5f;
        }


        void Update()
        {
            DoPhysics();
            
            if (transform.position.y < -10)
            {
                Destroy(fallingCube);
            }

            
        }

        
        private void DoPhysics()
        {

            // add acceleration to our velocity:
            float gravityMultiplier = 0.5f * 1;


            velocity.y -= gravity * Time.deltaTime * gravityMultiplier;

            // add velocity to position
            transform.position += velocity * Time.deltaTime;
        }

        
    }
}
