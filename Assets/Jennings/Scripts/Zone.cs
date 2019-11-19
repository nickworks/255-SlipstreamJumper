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
        /// <summary>
        /// List of Time Slowing Powerups
        /// </summary>
        List<AABB> timeballs = new List<AABB>();
       
        

        // public float delayBetweenPlatforms = 1; CAN DELETE

        // float timerPlatforms = 0; CAN DELETE

        /// <summary>
        /// chunkcs we're allowed to spawn
        /// </summary>
        public Chunk[] prefabChunks;

        /// <summary>
        /// Minimum and maximum of the gaps between the chunks.
        /// </summary>
        public float gapSizeMin = 2;
        public float gapSizeMax = 10;
        Camera camera1;

        

        void Awake()
        {
            camera1 = GetComponent<Camera>();
        }

        void Start() {

        }

        /// <summary>
        /// Here we update the game per frame and each frame there is less than 10 chunks another will be spawned. It also triggers removal on the left side of the screen.
        /// </summary>
        void Update()
        {

            if (chunks.Count < 10)
            {
                SpawnChunk();
            }
                RemoveOffscreenChunks();
        }

        // Finds the left side of the screen (for the purpose of screen size being manipulated)
        private float FindScreenLeftX()
        {
            Plane xy = new Plane(Vector3.forward, Vector3.zero);
            Ray ray = camera1.ScreenPointToRay(new Vector3(0, Screen.height / 2));

            if (xy.Raycast(ray, out float dis))
            {
                Vector3 pt = ray.GetPoint(dis);
                return pt.x;

            }

            return -12;
        }

        /// <summary>
        /// This is a function meant ideally to remove chunks to keep the game truly infinite although it is failing to truly destroy them.
        /// </summary>
        private void RemoveOffscreenChunks()
        {
            // This is to find and determie the point at which is determined as leaving the screen on the left side.
            float limitX = FindScreenLeftX();

            for (int i = chunks.Count - 1; i >= 0; i--)
            {
                if (chunks[i].rightEdge.position.x < limitX)
                {
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

                    // Remove references to the timeball AABBs from the list.
                    SlowTime[] deadTimeballs = chunk.GetComponentsInChildren<SlowTime>();
                    foreach (SlowTime timeball in deadTimeballs)
                    {
                        timeballs.Remove(timeball.GetComponent<AABB>());
                    }

                    chunks.RemoveAt(i);
                    Destroy(chunk.gameObject);
                }
            }

        }

        
        /// <summary>
        /// This is for spawning in chunks to the scene.
        /// </summary>
        private void SpawnChunk()
        {
            
            float gapSize = Random.Range(gapSizeMin, gapSizeMax);

            Vector3 pos = new Vector3();

            if(chunks.Count > 0) 
            {
                pos.x = chunks[chunks.Count - 1].rightEdge.position.x + gapSize;
                pos.y = chunks[chunks.Count - 1].rightEdge.position.y;
            }
            // Begins to instantiate the Chunks 
            int Index = Random.Range(0, prefabChunks.Length);
            Chunk chunk = Instantiate(prefabChunks[Index], pos, Quaternion.identity);
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

            // Add references to the spring AABBs to the list.
            SlowTime[] newTimeballs = chunk.GetComponentsInChildren<SlowTime>();
            foreach (SlowTime t in newTimeballs)
            {
                timeballs.Add(t.GetComponent<AABB>());
            }
        }

        void LateUpdate()
        {

            // check player AABB against every platform AABB
            foreach (AABB platform in platforms)
            {
                if (player.CollidesWith(platform))
                {
                    // This is what detects the collision with the platform
                    Vector3 fix = player.FindFix(platform);
                    player.BroadcastMessage("ApplyFix", fix);
                }
            }

            // check player AABB against every spring AABB:
            foreach (AABB spring in springs)
            {


                if (player.CollidesWith(spring))
                {
                    // Seeking Player movement for collision with spring
                    PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();

                    Spring s = spring.GetComponent<Spring>();

                    if (playerMovement != null)
                    { // sends player flying upwards
                        playerMovement.LaunchUpwards(s.springiness);
                    }
                }
            }

            // check player AABB against every spring AABB:
            foreach (AABB timeball in timeballs)
            {

                // Would ideally trigger upon collision with timeball
                if (player.CollidesWith(timeball))
                {
                    // Seeking Player Movement
                    PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();

                    SlowTime t = timeball.GetComponent<SlowTime>();

                    if (playerMovement != null)
                    {
                        // Commented out due to lack of proper functioning
                       /* if (PlayerMovement.timeSlowed = false)
                        {
                            timeSlowed = true;
                        }*/
                    }
                }
            }

        }


    }
}