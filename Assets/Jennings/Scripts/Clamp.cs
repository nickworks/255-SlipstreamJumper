using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jennings {

    public class Clamp : MonoBehaviour {

        // Update is called once per frame
        void Update()
        {
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, -11f, 11f),
            Mathf.Clamp(transform.position.y, -12f, 12f), transform.position.z);
        }
    }
}
