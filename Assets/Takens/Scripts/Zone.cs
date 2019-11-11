using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Takens {
    public class Zone : Pattison.Zone {

        new static public ZoneInfo info = new ZoneInfo() {
            zoneName = "Tower",
            creator = "Keith Takens",
            level = "TakensScene"
        };


        public AABB player;


        /// <summary>
        /// The current chunks in our scene
        /// </summary>
        List<Chunk> chunks = new List<Chunk>();
        /// <summary>
        /// The current AABBs of all platforms in our scene.
        /// </summary>
        static public List<AABB> platforms = new List<AABB>();
        /// <summary>
        /// The current AABBs of all springs in our scene.
        /// </summary>
        static public List<AABB> springs = new List<AABB>();

        [HideInInspector]
        public PlayerMovement PM;

        /// <summary>
        /// The chunks we are allowed to spawn.
        /// </summary>
        public Chunk[] prefabChunks;


        public float gapSizeMin = 0;
        public float gapSizeMax = 0;

        Camera cam;

        void Awake()
        {
            cam = GetComponent<Camera>();
        }

        void Start() {
            PM = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();   
        }

        void Update()
        {
            if (chunks.Count < 4)
            {
                SpawnChunk();
            }

            RemoveOffScreenChunks();
        }

        private void RemoveOffScreenChunks()
        {
            float limitY = FindScreenLeftBottom();
            //limitX = FindScreenLeftX();

            for (int i = chunks.Count - 1; i >= 0; i--)
            {
                if (chunks[i].topEdge.position.y < limitY)
                {
                    Chunk chunk = chunks[i];
                    Platform[] deadPlatforms = chunk.GetComponentsInChildren<Platform>();
                    foreach(Platform platform in deadPlatforms)
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
                if(player.transform.position.y < limitY)
                {
                    Game.GameOver();
                    Debug.Log("Game Over!!");
                }
            }
        }

        private float FindScreenLeftBottom()
        {
            Plane xy = new Plane(Vector3.forward, Vector3.zero);
            Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width/2, 0));

             Debug.DrawRay(ray.origin,ray.direction * 13, Color.yellow);

            if (xy.Raycast(ray, out float dis))
            {
                Vector3 pt = ray.GetPoint(dis);
                // limitX = pt.x;
                return pt.y;
            }
            else return -10;

         //   return limitX;
        }

        private void SpawnChunk()
        {
            //spawn new platforms:

            float gapSize = Random.Range(gapSizeMin,gapSizeMax);

            Vector3 pos = new Vector3();
            

            if (chunks.Count > 0)
            {
                pos.y = chunks[chunks.Count - 1].topEdge.position.y + gapSize;
                pos.x = chunks[chunks.Count - 1].topEdge.position.x;
            }

            int index = Random.Range(0, prefabChunks.Length);

            Chunk chunk = Instantiate(prefabChunks[index], pos, Quaternion.identity);
            chunks.Add(chunk);
            int flip = Random.Range(0, 10);
            if (flip >= 5)
            {
                chunk.transform.localScale = new Vector3(-1f, 1f, 1f);
            }
            Platform[] newPlatforms = chunk.GetComponentsInChildren<Platform>();
            foreach(Platform p in newPlatforms){
                platforms.Add(p.GetComponent<AABB>());
            }
            Spring[] newSprings = chunk.GetComponentsInChildren<Spring>();
            foreach (Spring p in newSprings)
            {
                springs.Add(p.GetComponent<AABB>());
            }
        }

        void LateUpdate() {
            //check player AABB against every platform AABB:
            foreach (AABB platform in platforms) {
                if (player.CollidesWith(platform))
                {
                    Vector3 fix = player.FindFix(platform);


                    if (platform.currentType == AABB.ObjectType.Passthrough && !(fix.y > 0))
                    {
                        // Debug.Log("Passing Through!");
                    }
                    else
                    {
                        if ((fix.y > 0) && (PM.velocity.y > 0))
                        {
                            return;
                        }

                        //collision!!!
                        player.BroadcastMessage("ApplyFix", fix);
                    }

                }

            }
            foreach (AABB spring in springs)
            {
                if (player.CollidesWith(spring))
                {
                    PM.LaunchUpwards();
                }
            }

        }

    }
}
