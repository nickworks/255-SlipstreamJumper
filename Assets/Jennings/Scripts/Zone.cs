using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jennings {
    public class Zone : Pattison.Zone {

        new static public ZoneInfo info = new ZoneInfo() {
            zoneName = "Road Runner", //Name of my zone
            creator = "Jaylen J.", //My Name
            level = "JenningsScene"
        };

        public AABB player;

        /// <summary>
        /// Current chunks in scene
        /// </summary>
        List<Chunk> chunks = new List<Chunk>();
        /// <summary>
        /// AABBs of all platforms in scene
        /// </summary>
        static public List<AABB> platforms = new List<AABB>();
        /// <summary>
        /// AABBs of all springs
        /// </summary>
        List<AABB> springs = new List<AABB>();

        //GameObject prefabSpring;
        //GameObject prefabPlatform;

            /// <summary>
            /// chunkcs we're allowed to spawn
            /// </summary>
        public Chunk[] prefabChunks;

        void Awake()
        {
            platforms.Clear();
        }

        void Start() {

            /*
            GameObject spring = Instantiate(prefabSpring, pos + Vector3.up, Quaternion.identity, newPlatform.transform);
            AABB aabb2 = spring.GetComponent<AABB>();
            if (aabb2)
            {
                springs.Add(aabb2);
                aabb2.Recalc();
            }
            */
        }

        private void SpawnChunk()
        {
            /*
            float gapSize = Random.Range(gapSizeMin, gapSizeMax);

            Vector3 pos = new Vector3();

            if(chunks.Count > 0)
            {
                pos.x = chunks[chunks.Count - 1].rightEdge.position.x + gapSize;
                pos.y = chunks[chunks.Count - 1].rightEdge.position.7
            }

            int Index // continue this!
            */
        }

        private void RemoveOffscreenChunks()
        {
            /*
            float limitX = FindScreenLeftX();

            for(int i = chunks.Count - 1; i >= 0, i--)
            {

                AABB platform = Chunks[i]; // this line needs fixing!
                chunks.RemoveAt(i);
                Destroy(platform.gameObject);
            }
            */
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

            // check player AABB against every spring AABB:
            foreach (AABB spring in springs)
            {


                if (player.CollidesWith(spring))
                {
                    // boing! 
                    PlayerMovement mover = player.GetComponent<PlayerMovement>();
                    if (mover != null)
                    {
                        mover.LaunchUpwards(8);
                    }
                }
            }
            
        }


    }
}