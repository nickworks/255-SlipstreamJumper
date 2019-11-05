using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jennings {

    [RequireComponent(typeof(AABB))]
    public class Platform : MonoBehaviour {
        

        void Start()
        {
            // register self:
            AABB aabb = GetComponent<AABB>();
            Zone.platforms.Add(aabb);
        }
        void OnDestroy()
        {
            AABB aabb = GetComponent<AABB>();
            Zone.platforms.Remove(aabb);
        }

    }
}
