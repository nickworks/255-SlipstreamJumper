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

        public AABB a;
        public AABB b;

        void Start() {

        }

        void Update() {

            if (a.CollidesWith(b)) {
                a.GetComponent<MeshRenderer>().material.color = Color.red;
                b.GetComponent<MeshRenderer>().material.color = Color.red;

                Vector3 fix = a.FindFix(b);
                a.ApplyFix(fix);

            } else {
                a.GetComponent<MeshRenderer>().material.color = Color.white;
                b.GetComponent<MeshRenderer>().material.color = Color.white;
            }


        }
    }
}