using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Takens {
    /// <summary>
    /// Main game mechanics class, spawns and removes chunks, handles collisions
    /// Added to main camera of scene
    /// </summary>
    public class Zone : Pattison.Zone {

        new static public ZoneInfo info = new ZoneInfo() {
            zoneName = "Tower",
            creator = "Keith Takens",
            level = "TakensScene"
        };

        /// <summary>
        /// Reference to the player
        /// </summary>
        public AABB player;

        /// <summary>
        /// The current chunks in our scene
        /// </summary>
        List<Chunk> chunks = new List<Chunk>();

        /// <summary>
        /// The current AABBs of all platforms in our scene.
        /// </summary>
        public List<AABB> platforms = new List<AABB>();

        /// <summary>
        /// The current AABBs of all springs in our scene.
        /// </summary>
        public List<AABB> springs = new List<AABB>();

        /// <summary>
        /// The current AABBs of all powerUps in our scene.
        /// </summary>
        public List<AABB> powerUps = new List<AABB>();

        /// <summary>
        /// The current AABBs of all spikes in our scene.
        /// </summary>
        public List<AABB> spikes = new List<AABB>();

        /// <summary>
        /// Reference to player's PlayerMovement component
        /// </summary>
        [HideInInspector]
        public PlayerMovement PM;

        /// <summary>
        /// The chunks we are allowed to spawn.
        /// </summary>
        public Chunk[] prefabChunks;

        /// <summary>
        /// Minimum distance between chunks in meters
        /// </summary>
        public float gapSizeMin = 0;

        /// <summary>
        /// Maximum distance between chunks in meters
        /// </summary>
        public float gapSizeMax = 0;

        /// <summary>
        /// Reference to main camera
        /// </summary>
        Camera cam;

        /// <summary>
        /// Called on start, sets reference to camera
        /// </summary>
        void Awake()
        {
            cam = GetComponent<Camera>();
        }

        /// <summary>
        /// Called on start, sets reference to player's PlayerMovement component
        /// </summary>
        void Start() {
            PM = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();   
        }

        /// <summary>
        /// Called every frame if game is not paused,
        /// Calls methods to spawn and load chunks
        /// </summary>
        void Update()
        {
            if (!Game.isPaused)
            {
                if (chunks.Count < 4)
                {
                    SpawnChunk();
                }

                RemoveOffScreenChunks();
            }
        }
        /// <summary>
        /// Removes Chunks that go off of the bottom of the screen
        /// </summary>
        private void RemoveOffScreenChunks()
        {
            float limitY = FindScreenLeftBottom();//sets a threshold to the coord of the bottom of the screen

            for (int i = chunks.Count - 1; i >= 0; i--)//counts backwards to avoid skipping bug
            {
                if (chunks[i].topEdge.position.y < limitY)
                {
                    Chunk chunk = chunks[i];
                    Platform[] deadPlatforms = chunk.GetComponentsInChildren<Platform>();
                    foreach(Platform platform in deadPlatforms)//removes all platforms in dead chunk
                    {
                        platforms.Remove(platform.GetComponent<AABB>());
                    }
                    Spring[] deadSprings = chunk.GetComponentsInChildren<Spring>();
                    foreach (Spring spring in deadSprings)//removes all springs in dead chunk
                    {
                        springs.Remove(spring.GetComponent<AABB>());
                    }
                    PowerUp[] deadPowerUps = chunk.GetComponentsInChildren<PowerUp>();
                    foreach (PowerUp pup in deadPowerUps)//removes all powerUps in dead chunk
                    {
                        powerUps.Remove(pup.GetComponent<AABB>());
                    }
                    Spike[] deadSpikes = chunk.GetComponentsInChildren<Spike>();
                    foreach (Spike s in deadSpikes)//removes all spikes in dead chunk
                    {
                        spikes.Remove(s.GetComponent<AABB>());
                    }
                    chunks.RemoveAt(i);
                    Destroy(chunk.gameObject);
                }
                if(player.transform.position.y < limitY)//If the player is below this threshold, call Game.GameOver();
                {
                    Game.GameOver();
                    Debug.Log("Game Over!!");
                }
            }
        }

        /// <summary>
        /// Finds the coord of the bottom of the screen using a ray cast
        /// </summary>
        /// <returns>y coord of bottom of screen</returns>
        private float FindScreenLeftBottom()
        {
            Plane xy = new Plane(Vector3.forward, Vector3.zero);
            Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width/2, 0));

             Debug.DrawRay(ray.origin,ray.direction * 13, Color.yellow);//shows ray in viewport

            if (xy.Raycast(ray, out float dis))
            {
                Vector3 pt = ray.GetPoint(dis);
                return pt.y;
            }
            else return -10;

         //   return limitY;
        }

        /// <summary>
        /// Spawns random chunk from list
        /// </summary>
        private void SpawnChunk()
        {
            //spawn new platforms:

            float gapSize = Random.Range(gapSizeMin,gapSizeMax);//randomizes gap size

            Vector3 pos = new Vector3();
            

            if (chunks.Count > 0)
            {
                pos.y = chunks[chunks.Count - 1].topEdge.position.y + gapSize;
                pos.x = chunks[chunks.Count - 1].topEdge.position.x;
            }

            int index = Random.Range(0, prefabChunks.Length);

            Chunk chunk = Instantiate(prefabChunks[index], pos, Quaternion.identity);
            chunks.Add(chunk);

            //50% of chunks being flipped in the horizontal axis
            int flip = Random.Range(0, 10);
            if (flip >= 5)
            {
                chunk.transform.localScale = new Vector3(-1f, 1f, 1f);
            }

            Platform[] newPlatforms = chunk.GetComponentsInChildren<Platform>();//adds platforms in chunk to platforms array
            foreach(Platform p in newPlatforms){
                platforms.Add(p.GetComponent<AABB>());
            }

            Spring[] newSprings = chunk.GetComponentsInChildren<Spring>();//adds springs in chunk to springs array
            foreach (Spring p in newSprings)
            {
                springs.Add(p.GetComponent<AABB>());
            }
            PowerUp[] ps = chunk.GetComponentsInChildren<PowerUp>();//adds powerups in chunk to powerUps array
            foreach (PowerUp p in ps)
            {
                powerUps.Add(p.GetComponent<AABB>());
            }
            Spike[] newSpikes = chunk.GetComponentsInChildren<Spike>();//adds spikes in chunk to spikes array
            foreach (Spike s in newSpikes)
            {
                spikes.Add(s.GetComponent<AABB>());
            }
        }
        
        /// <summary>
        /// Checks for collisions with each type of object in the scene
        /// </summary>
        void LateUpdate() {
            //check player AABB against every platform AABB:
            foreach (AABB platform in platforms) {
                if (player.CollidesWith(platform))
                {
                    Vector3 fix = player.FindFix(platform);

                    //cancels applying of the fix if you are passing through a passthrough platform

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
            foreach (AABB spring in springs) //checks for collisions with all the spring AABBs
            {
                if (player.CollidesWith(spring))
                {
                    PM.LaunchUpwards();
                }
            }
            foreach (AABB powerUp in powerUps)//checks for collisions with all  the powerup AABBs
            {
                if (player.CollidesWith(powerUp))
                {
                    PM.doubleJumpsLeft += 1;
                    if (PM.doubleJumpsLeft > 3) PM.doubleJumpsLeft = 3;

                    GameObject pu = powerUp.gameObject;
                    powerUps.Remove(powerUp);
                    Destroy(pu);
                }
            }
            foreach (AABB spike in spikes) //checks for collisions with all the sike AABBs
            {
                if (player.CollidesWith(spike))
                {
                    Game.GameOver();
                    Debug.Log("Game Over!!");
                }
            }

        }

    }
}
