using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Takens
{
    /// <summary>
    /// Chunk added to chunks parent
    /// </summary>
    public class Chunk : MonoBehaviour
    {
        /// <summary>
        /// Defines the top limit of the chunk to determine when to delete it,
        /// as well as where to spawn another one from
        /// </summary>
        public Transform topEdge;
    }
}
