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
        List<AABB> platforms = new List<AABB>();
        /// <summary>
        /// AABBs of all springs
        /// </summary>
        List<AABB> springs = new List<AABB>();

        //GameObject prefabSpring;
        public GameObject prefabPlatform;

        // public float delayBetweenPlatforms = 1; CAN DELETE

        // float timerPlatforms = 0; CAN DELETE

        public float gapSizeMin = 2;
        public float gapSizeMax = 10;

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

        void Update()
        {
            // timerPlatforms -= Time.deltaTime; CAN DELETE WHEN TIMER PLATFORMS IS DELETED

            if (platforms.Count < 5)
            {
                SpawnPlatform();
            }

            for(int i = platforms.Count - 1; i < platforms.Count; i--)
            {
                if (platforms[i].max.x < -13)
                {
                    AABB platform = platforms[i];

                    platforms.RemoveAt(i);
                    Destroy(platform.gameObject);
                }
            }
        }

        private void SpawnPlatform()
        {
            // Spawn new platforms:
            // Vector3 places the given platform in a specified location, try using Random.Range(##.#f, ##.#f) instead of a number for some randomness

            float gapSize = Random.Range(gapSizeMin, gapSizeMax);
            float nextPlatformWidth = 10;

            Vector3 pos = new Vector3();

            if (platforms.Count > 0)
            {
                AABB lastPlatform = platforms[platforms.Count - 1];
                pos.x = lastPlatform.max.x + gapSize + nextPlatformWidth/2;

            }

            GameObject newPlatform = Instantiate(prefabPlatform, pos, Quaternion.identity);
            // Scale (for width) can't be changed via Instantiate (above) therefore this changes it immeditately after instantiation
            newPlatform.transform.localScale = new Vector3(nextPlatformWidth, 1, 1);
            //Original version relating to comment under Spawn New Platforms
            //GameObject newPlatform = Instantiate(prefabPlatform, new Vector3(5, 0, 0), Quaternion.identity);

            AABB aabb = newPlatform.GetComponent<AABB>();
            if (aabb) {
                platforms.Add(aabb);
                aabb.Recalc();
            }
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