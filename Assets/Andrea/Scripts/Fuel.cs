using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Andrea
{
    [RequireComponent(typeof(AABB))]
    public class Fuel : MonoBehaviour
    {
        /// <summary>
        /// The amount of fuel the object provides when picked up.
        /// </summary>
        public float fuelValue = 20f; 
    }
}