using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jennings {
    public class SafeMode : MonoBehaviour {

        public static bool isSafeModeOff = true;
        // Start is called before the first frame update
        void Start()
        {
            //This will cause the open space between platforms to act as a platform temporarily.
            if (isSafeModeOff)
            {
                
            }
            else
            {
                
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
