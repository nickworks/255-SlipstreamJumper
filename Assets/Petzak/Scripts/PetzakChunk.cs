using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Petzak
{
    /// <summary>
    /// A prefab object that contains platforms, springs, and spikes.
    /// Procedurally added to the game during runtime.
    /// </summary>
    public class PetzakChunk : MonoBehaviour
    {
        /// <summary>
        /// Empty game object that's put at the right edge of a chunk.
        /// Used to determine where to spawn next chunk.
        /// </summary>
        public Transform rightEdge;
    }
}