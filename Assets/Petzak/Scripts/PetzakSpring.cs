using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Petzak
{
    /// <summary>
    /// Holds the springiness of a spring
    /// </summary>
    [RequireComponent(typeof(PetzakAABB))]
    public class PetzakSpring : MonoBehaviour
    {
        /// <summary>
        /// Level of force applied when player touches spring.
        /// </summary>
        public float springiness = 10;
    }
}