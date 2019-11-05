using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Andrea {
    public class Zone : Pattison.Zone {

        new static public ZoneInfo info = new ZoneInfo() {
            zoneName = "Something Cool",
            creator = "Vincent Andrea",
            level = "AndreaScene"
        };


        /// <summary>
        /// The AABB of the player.
        /// </summary>
        public AABB player;

        /// <summary>
        /// The current collection of chunks in the scene.
        /// </summary>
        List<Chunk> chunks = new List<Chunk>();

        /// <summary>
        /// The current collection of platform AABBs in the scene.
        /// </summary>
        List<AABB> platforms = new List<AABB>();

        /// <summary>
        /// The current colleciton of spring AABBs in the scene.
        /// </summary>
        List<AABB> springs = new List<AABB>();

        /// <summary>
        /// The collection of available chunks.
        /// </summary>
        public Chunk[] prefabChunks;

        /// <summary>
        /// The minimum 
        /// </summary>
        public float gapSizeMin = 2;
        public float gapSizeMax = 9;

        Camera cam;

        void Awake()
        {
            cam = GetComponent<Camera>();
        }

        void Start()
        {
            
        }

        void Update()
        {
            if (chunks.Count < 5)
            {
                SpawnChunk();
            }
            RemoveOffscreenChunks();
        }

        private void RemoveOffscreenChunks()
        {
            float limitX = -20;
            limitX = FindScreenLeftX();
            for (int i = chunks.Count - 1; i >= 0; i--)
            {
                if (chunks[i].rightEdge.position.x < limitX)
                {
                    Chunk chunk = chunks[i];

                    Platform[] deadPlatforms = chunk.GetComponentsInChildren<Platform>();
                    foreach (Platform platform in deadPlatforms)
                    {
                        platforms.Remove(platform.GetComponent<AABB>());
                    }

                    Spring[] deadSprings = chunk.GetComponentsInChildren<Spring>();
                    foreach (Spring spring in deadSprings)
                    {
                        springs.Remove(spring.GetComponent<AABB>());
                    }

                    chunks.RemoveAt(i);
                    Destroy(chunk.gameObject);

                }

            }
        }

        private float FindScreenLeftX()
        {
            Plane xy = new Plane(Vector3.forward, Vector3.zero);
            Ray ray = cam.ScreenPointToRay(new Vector3(0, Screen.height / 2));

            Debug.DrawRay(ray.origin, ray.direction * 10, Color.red);
            if (xy.Raycast(ray, out float dis))
            {
                Vector3 pt = ray.GetPoint(dis);
                return pt.x;
            }

            return -20;
        }

        private void SpawnChunk()
        {
            // spawn new chunks:

            float gapSize = Random.Range(gapSizeMin, gapSizeMax);

            Vector3 pos = new Vector3();

            if (chunks.Count > 0)
            {
                pos.x = chunks[chunks.Count - 1].rightEdge.position.x + gapSize;
                pos.y = chunks[chunks.Count - 1].rightEdge.position.y;
                //pos.y = lastPlatform.Min.y + (nextPlatformHeight / 2) + Random.Range(-2, 3);
            }

            int index = Random.Range(0, prefabChunks.Length);
            Chunk chunk = Instantiate(prefabChunks[index], pos, Quaternion.identity);
            chunks.Add(chunk);

            Platform[] newPlatforms = chunk.GetComponentsInChildren<Platform>();
            foreach (Platform p in newPlatforms)
            {
                platforms.Add(p.GetComponent<AABB>());
            }

            Spring[] newSprings = chunk.GetComponentsInChildren<Spring>();
            foreach (Spring s in newSprings)
            {
                springs.Add(s.GetComponent<AABB>());
            }

        }

        void LateUpdate() 
        {
            // check player AABB against every platform AABB:
            foreach (AABB platform in platforms)
            {
                if (player.CollidesWith(platform))
                {
                    Vector3 fix = player.FindFix(platform);
                    player.BroadcastMessage("ApplyFix", fix);
                }
            }

            // check player AABB against every spring AABB:
            foreach (AABB spring in springs)
            {
                if (player.CollidesWith(spring))
                {
                    
                    PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
                    
                    Spring s = spring.GetComponent<Spring>();

                    if (playerMovement != null && s != null)
                    {
                        playerMovement.LaunchUpwards(s.springiness);
                    }
                }
            }
        }
    }
}