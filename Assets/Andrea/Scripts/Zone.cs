using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Andrea {
    public class Zone : Pattison.Zone {

        new static public ZoneInfo info = new ZoneInfo() {
            zoneName = "Hippity Hoppity",
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
        /// The current collection of hazard AABBs in the scene.
        /// </summary>
        List<AABB> hazards = new List<AABB>();

        /// <summary>
        /// The current collection of fuel AABBs in the scene.
        /// </summary>
        List<AABB> fuels = new List<AABB>();

        /// <summary>
        /// The collection of available chunks.
        /// </summary>
        public Chunk[] prefabChunks;


        /// <summary>
        /// The minimum distance between chunks.
        /// </summary>
        public float gapSizeMin = 2;

        /// <summary>
        /// The maximum distance between chunks.
        /// </summary>
        public float gapSizeMax = 9;


        /// <summary>
        /// Reference to the main camera.
        /// </summary>
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

            if (chunks.Count < 10)
            {
                // Less than 10 chunks are instantiated, instantiate a new random chunk.
                SpawnChunk();
            }
            RemoveOffscreenChunks(); 
        }

        /// <summary>
        /// Destroys chunks that are left of the camera view and removes their referenced AABBs from lists.
        /// </summary>
        private void RemoveOffscreenChunks()
        {
            float limitX = FindScreenLeftX();
            for (int i = chunks.Count - 1; i >= 0; i--)
            {
                if (chunks[i].rightEdge.position.x < limitX)
                {
                    // The chunk is to the left of the screen
                    Chunk chunk = chunks[i];

                    // Remove references to the platform AABBs from the list.
                    Platform[] deadPlatforms = chunk.GetComponentsInChildren<Platform>();
                    foreach (Platform platform in deadPlatforms)
                    {
                        platforms.Remove(platform.GetComponent<AABB>());
                    }

                    // Remove references to the spring AABBs from the list.
                    Spring[] deadSprings = chunk.GetComponentsInChildren<Spring>();
                    foreach (Spring spring in deadSprings)
                    {
                        springs.Remove(spring.GetComponent<AABB>());
                    }

                    // Remove references to the spike AABBs from the list.
                    Hazard[] deadHazards = chunk.GetComponentsInChildren<Hazard>();
                    foreach (Hazard h in deadHazards)
                    {
                        hazards.Remove(h.GetComponent<AABB>());
                    }

                    // Remove references to the fuel AABBs from the list.
                    Fuel[] deadFuel = chunk.GetComponentsInChildren<Fuel>();
                    foreach (Fuel f in deadFuel)
                    {
                        fuels.Remove(f.GetComponent<AABB>());
                    }

                    chunks.RemoveAt(i); // Remove the chunk from the list.
                    Destroy(chunk.gameObject); // Remove the chunk from the scene.
                }

            }
        }

        /// <summary>
        /// Returns the left boundary of the screen.
        /// </summary>
        /// <returns></returns>
        private float FindScreenLeftX()
        {
            Plane xy = new Plane(Vector3.forward, Vector3.zero);
            Ray ray = cam.ScreenPointToRay(new Vector3(0, Screen.height / 2));

            Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);
            if (xy.Raycast(ray, out float dis))
            {
                Vector3 pt = ray.GetPoint(dis);
                return pt.x;
            }

            return -20;
        }

        /// <summary>
        /// Instantiates a new chunk and adds the AABBs of child objects to lists.
        /// </summary>
        private void SpawnChunk()
        {
            float gapSize = Random.Range(gapSizeMin, gapSizeMax);

            Vector3 pos = new Vector3();

            if (chunks.Count > 0)
            {
                pos.x = chunks[chunks.Count - 1].rightEdge.position.x + gapSize;
                pos.y = chunks[chunks.Count - 1].rightEdge.position.y;
                //pos.y = lastPlatform.Min.y + (nextPlatformHeight / 2) + Random.Range(-2, 3);
            }

            // Select a random prefab chunk from the array and add it to the list of instantiated chunks.
            int index = Random.Range(0, prefabChunks.Length);
            Chunk chunk = Instantiate(prefabChunks[index], pos, Quaternion.identity);
            chunks.Add(chunk);
   
            // Add references to the platform AABBs to the list.
            Platform[] newPlatforms = chunk.GetComponentsInChildren<Platform>();
            foreach (Platform p in newPlatforms)
            {
                platforms.Add(p.GetComponent<AABB>());
            }

            // Add references to the spring AABBs to the list.
            Spring[] newSprings = chunk.GetComponentsInChildren<Spring>();
            foreach (Spring s in newSprings)
            {
                springs.Add(s.GetComponent<AABB>());
            }

            // Add references to the spike AABBs to the list.
            Hazard[] newHazards = chunk.GetComponentsInChildren<Hazard>();
            foreach (Hazard h in newHazards)
            {
                hazards.Add(h.GetComponent<AABB>());
            }

            // Add references to the fuel AABBs to the list.
            Fuel[] newFuel = chunk.GetComponentsInChildren<Fuel>();
            foreach (Fuel f in newFuel)
            {
                fuels.Add(f.GetComponent<AABB>());
            }

        }

        void LateUpdate() 
        {
            // check player AABB against every platform AABB:
            foreach (AABB platform in platforms)
            {
                if (player.CollidesWith(platform))
                {
                    PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();

                    Vector3 fix = player.FindFix(platform);

                    if (platform.currentType == AABB.CurrentType.PassThough && fix.y < 0)
                    {
                        //The player is colliding with the one way platform from below, the fix should be skipped.
                        continue;
                    }
                    else if (fix.y > 0 && playerMovement.velocity.y > 0)
                    {
                        //The player was allowed to pass through and is moving upward, but collision suggests a positive velocity to fix. No change in velocity needs to occur - fix is skipped.
                        continue;
                    }
                    else
                    {
                        player.BroadcastMessage("ApplyFix", fix);
                    }

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
                        playerMovement.LaunchUpwards(s.springiness); // Launch the player upward.
                    }
                }
            }

            // check player AABB against every fuel AABB:
            foreach (AABB fuel in fuels)
            {
                if (player.CollidesWith(fuel))
                {
                    PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
                    Fuel f = fuel.GetComponent<Fuel>();
                    if (playerMovement != null && f != null)
                    {
                        playerMovement.AddFuel(f.fuelValue); // Add the fuel to the player inventory.
                        f.fuelValue = 0;
                        f.gameObject.GetComponent<MeshRenderer>().enabled = false;
                    }
                }
            }

            // check player AABB against every spike AABB:
            foreach (AABB hazard in hazards)
            {
                if (player.CollidesWith(hazard))
                {
                    PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
                    Hazard h = hazard.GetComponent<Hazard>();
                    if (playerMovement != null && h != null)
                    {
                        Game.GameOver(); // End the game if collided with.
                    }
                }
            }
        }
    }
}