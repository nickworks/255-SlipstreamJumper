using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jennings {
    [RequireComponent(typeof(AABB))]
    public class Spring : MonoBehaviour {
        // The force being applied from the spring.
        public float springiness = 15;
    }
}
