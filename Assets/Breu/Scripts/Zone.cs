using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Breu {
    public class Zone : Pattison.Zone {

        new static public ZoneInfo info = new ZoneInfo() {
            zoneName = "The Water Temple",
            creator = "Breu",
            level = "BreuScene"
        };

        public BreuAABB player;
        public BreuAABB b;


        void Start() { }

        void Update()
        {
            if (player.collidesWith(b))
            {
                player.GetComponent<MeshRenderer>().material.color = Color.red;
                b.GetComponent<MeshRenderer>().material.color = Color.red;

                Vector3 fix = player.findFix(b);
                player.applyFix(fix);
            }
            else
            {
                player.GetComponent<MeshRenderer>().material.color = Color.blue;
                b.GetComponent<MeshRenderer>().material.color = Color.blue;
            }
        }


    }
}