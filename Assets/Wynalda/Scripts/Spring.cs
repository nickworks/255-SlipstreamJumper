using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wynalda
{
    /// <summary>
    /// This requires an AABB come along anytime the spring script is applied to an object.
    /// </summary>
    [RequireComponent(typeof(AABB))]
    public class Spring : MonoBehaviour
    {
        /// <summary>
        /// This is the amount of which the spring "springs". The number is changed in the inspector.
        /// </summary>
        public float springiness = 50;
    }
}
