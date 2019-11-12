using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Caughman
{ 
public class Chunks : MonoBehaviour
    {

        public Transform rightEdge;
        public float edgeX;

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            edgeX = rightEdge.position.x;
        }
    }
}
