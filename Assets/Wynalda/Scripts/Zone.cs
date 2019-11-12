using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wynalda
{
    public class Zone : Pattison.Zone {

        new static public ZoneInfo info = new ZoneInfo() {
            zoneName = "Penguin Hops",
            creator = "Wynalda",
            level = "WynaldaScene"
        };

        //Player!
        public AABB player;
        /// <summary>
        /// The current chunks in our scene
        /// </summary>
        List<Chunk> chunks = new List<Chunk>();
        /// <summary>
        /// The AABBs of all platforms in our scene
        /// </summary>
        public List<AABB> platforms = new List<AABB>();
        /// <summary>
        /// the AABBs of all springs in our scene
        /// </summary>
        public List<AABB> springs = new List<AABB>();
        /// <summary>
        /// the AABBs of all powerups in our scene
        /// </summary>
        public List<AABB> powers = new List<AABB>();
        /// <summary>
        /// the AABBS of all hazards in our scene
        /// </summary>
        public List<AABB> hazards = new List<AABB>();
        /// <summary>
        /// The chunks we are allowed to spawn
        /// </summary>
        public Chunk[] prefabChunks;

        
        //minimum amount of space between platforms
        public float gapSizeMin = 12;
        //minimum amount of space between platforms
        public float gapSizeMax = 20;

        //Camera!
        Camera cam;

        /// <summary>
        /// Awake!
        /// </summary>
        void Awake()
        {
            cam = GetComponent<Camera>();
        }
        
        //Start!
        void Start() {
            SpawnChunk(true);
            
        }

        //Things that happen often!
        void Update()
        {
            //Less than 3 chunks? Spawn a new one!
            if (chunks.Count < 3)
            {
                SpawnChunk(); // Spawn chunk!
            }

            //Goodbye chunks we cant see anymore!
            RemoveOffScreenChunks();
        }

        //Removes chunks that are off the screen to prevent memory leak!
        private void RemoveOffScreenChunks()
        {
            //Fine left of screen to determine where things should die off!
            float limitX = FindScreenLeftX();
            
            //Removes dead offscreen chunks from arrays.
            for (int i = chunks.Count - 1; i >= 0; i--)
            {
                if (chunks[i].rightEdge.position.x < limitX)
                {
                    Chunk chunk = chunks[i];

                    //Goodbye platforms!
                    Platform[] deadPlatforms = chunk.GetComponentsInChildren<Platform>();
                    foreach(Platform platform in deadPlatforms)
                    {
                        platforms.Remove(platform.GetComponent<AABB>());
                    }
                    //Goodbye springs!
                    Spring[] deadSprings = chunk.GetComponentsInChildren<Spring>();
                    foreach (Spring spring in deadSprings)
                    {
                        springs.Remove(spring.GetComponent<AABB>());
                    }
                    //Goodbye powerups!
                    Power[] deadPowers = chunk.GetComponentsInChildren<Power>();
                    foreach (Power power in deadPowers)
                    {
                        powers.Remove(power.GetComponent<AABB>());
                    }
                    //Goodbye hazards!
                    Hazard[] deadHazards = chunk.GetComponentsInChildren<Hazard>();
                    foreach (Hazard hazard in deadHazards)
                    {
                        hazards.Remove(hazard.GetComponent<AABB>());
                    }
              
                    //removing the gameObjects.
                    chunks.RemoveAt(i);
                    Destroy(chunk.gameObject);
                }
            }
        }

        //Finds left side of screen to determine where chunks should begin to be removed.
        private float FindScreenLeftX()
        {
            Plane xy = new Plane(Vector3.forward, Vector3.zero);
            Ray ray = cam.ScreenPointToRay(new Vector3(0, Screen.height / 2));

            if (xy.Raycast(ray, out float dis))
            {
                Vector3 pt = ray.GetPoint(dis);
                return pt.x;
            }
            else return -10;

        }

        //spawn new chunks!
        private void SpawnChunk(bool isStartingPlatform = false)
        {
           
            //random gap size:
            float gapSize = Random.Range(gapSizeMin,gapSizeMax);

            //position of new chunk in scene
            Vector3 pos = new Vector3(0, -4.2f, 0);

            //if there are more than zero chunks 
            if (chunks.Count > 0)
            {

                pos.x = chunks[chunks.Count - 1].rightEdge.position.x + gapSize;
                pos.y = chunks[chunks.Count - 1].rightEdge.position.y;
                //  pos.y = -5;
            }

            //random chunk spawning
            int index = Random.Range(0, prefabChunks.Length);

            //chunks!
            Chunk chunk = Instantiate(prefabChunks[index], pos, Quaternion.identity);
            chunks.Add(chunk);

            //spawns in platforms and adds them to array.
            Platform[] newplatforms = chunk.GetComponentsInChildren<Platform>();
            foreach(Platform p in newplatforms)
            {
                platforms.Add(p.GetComponent<AABB>());
            }

            //spawns in springs and adds them to array.
            Spring[] newsprings = chunk.GetComponentsInChildren<Spring>();
            foreach(Spring s in newsprings)
            {
                springs.Add(s.GetComponent<AABB>());
            }
            //spawns in powers and adds them to array.
            Power[] newpowers = chunk.GetComponentsInChildren<Power>();
            foreach (Power u in newpowers)
            {
                powers.Add(u.GetComponent<AABB>());
            }
            //spawns in hazards and adds them to array.
            Hazard[] newhazards = chunk.GetComponentsInChildren<Hazard>();  
            foreach (Hazard h in newhazards)
            {
                hazards.Add(h.GetComponent<AABB>());
            }
            
            

        }

        void LateUpdate() {
            //check player AABB against every platform AABB:
            foreach (AABB platform in platforms)
            {
                if (player.CollidesWith(platform))
                {
                    Vector3 fix = player.FindFix(platform);
                    player.BroadcastMessage("ApplyFix", fix);
                    //print("Hi"); was used to test if this was working

                }

            }

            foreach (AABB spring in springs)
            {

                //check player AABB against every spring AABB:
                if (player.CollidesWith(spring))
                {
                    PlayerMovement mover = player.GetComponent<PlayerMovement>();
                    Spring s = spring.GetComponent<Spring>();


                    if (mover != null)
                    {
                       // print($"springiness is {s.springiness}"); USED FOR TESTING!
                        mover.LaunchUpwards(s.springiness);
                    }
                }
            }



            //check player AABB against every powerup AABB:
            foreach (AABB power in powers)
            {
                if (player.CollidesWith(power))
                {
                    Power u = power.GetComponent<Power>();
                    PlayerMovement mover = player.GetComponent<PlayerMovement>();
                    if (mover != null)
                    {
                        mover.GoFast(u.speed);
                        power.GetComponent<MeshRenderer>().enabled = false;
                    }

                   

                }

            }

             foreach (AABB hazard in hazards)
            {
                if (player.CollidesWith(hazard))
                {
                    Hazard h = hazard.GetComponent<Hazard>();
                    PlayerMovement mover = player.GetComponent<PlayerMovement>();
                    if (mover != null)
                    {
                        mover.Death();            
                    }

                }

            }

        } // end LateUpdate()


    }// end class
}// end namespace
