using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Andrea
{
    [RequireComponent(typeof(AABB))]
    public class Spring : MonoBehaviour
    {
        /// <summary>
        /// The amount of force this spring applies.
        /// </summary>
        public float springiness = 15;
    }
}