using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jennings {
    public class Zone : Pattison.Zone {

        new static public ZoneInfo info = new ZoneInfo() {
            zoneName = "Road Runner", //Name of my zone
            creator = "Student Lastname", //My Name
            level = "JenningsScene"
        };

        public AABB player;
        static public List<AABB> platforms = new List<AABB>();

        void Awake()
        {
            platforms.Clear();
        }

        void Start() {
            
        }

        void LateUpdate() {

            // check player AABB against every platform AABB
            foreach(AABB platform in platforms)
            {
                if (player.CollidesWith(platform))
                {
                    // COLLISION!
                    Vector3 fix = player.FindFix(platform);
                    player.BroadcastMessage("ApplyFix", fix);
                }
            }
            
        }


    }
}