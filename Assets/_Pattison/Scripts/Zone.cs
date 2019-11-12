using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ZoneInfo {
    public string zoneName;
    public string creator;
    public string level;
}
namespace Pattison {
    public class Zone : MonoBehaviour {
        static public ZoneInfo info = new ZoneInfo() {
            zoneName = "",
            creator = "",
            level = ""
        };

        
    } // end class
} // end namespace