using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jennings {
    public class Chunk : MonoBehaviour {
        // Establishes where the right edge is so the next Chunk's spawn location can be accurately determined.
        public Transform rightEdge;
        public float edgeX;

        private void Start()
        {
           
        }

        private void Update()
        {
            edgeX = rightEdge.position.x;
        }
    }
}
