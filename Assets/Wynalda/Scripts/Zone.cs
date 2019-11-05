using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wynalda
{
    public class Zone : Pattison.Zone {

        new static public ZoneInfo info = new ZoneInfo() {
            zoneName = "40 Yard Dash",
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
        static public List<AABB> platforms = new List<AABB>();
        /// <summary>
        /// the AABBs of all springs in our scene
        /// </summary>
        static public List<AABB> springs = new List<AABB>();
        /// <summary>
        /// The chunks we are allowed to spawn
        /// </summary>
        public Chunk[] prefabChunks;
        
        //minimum amount of space between platforms
        public float gapSizeMin = 2;
        //minimum amount of space between platforms
        public float gapSizeMax = 10;

        Camera cam;

        public Texture[] textures;
        public Texture startingTexture;

        void Awake()
        {
            cam = GetComponent<Camera>();
        }

        void Start() {
            SpawnChunk(true);
            
        }

        void Update()
        {
            if (chunks.Count < 2)
            {
                SpawnChunk();
            }

            RemoveOffScreenChunks();
        }

        private void RemoveOffScreenChunks()
        {
            float limitX = FindScreenLeftX();

            for (int i = chunks.Count - 1; i >= 0; i--)
            {
                if (chunks[i].rightEdge.position.x < limitX)
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
            }
        }

        private float FindScreenLeftX()
        {
            Plane xy = new Plane(Vector3.forward, Vector3.zero);
            Ray ray = cam.ScreenPointToRay(new Vector3(0, Screen.height / 2));

            //  Debug.DrawRay(ray.origin,ray.direction * 10, Color.yellow);

            if (xy.Raycast(ray, out float dis))
            {
                Vector3 pt = ray.GetPoint(dis);
                // limitX = pt.x;
                return pt.x;
            }
            else return -10;

         //   return limitX;
        }

        private void SpawnChunk(bool isStartingPlatform = false)
        {
            //spawn new chunk:

            float gapSize = Random.Range(gapSizeMin,gapSizeMax);


           // if (isStartingPlatform)
           // {
           //     nextPlatformWidth = 50;
           // }

            Vector3 pos = new Vector3(0, -3 , 0);

            if (chunks.Count > 0)
            {

                pos.x = chunks[chunks.Count - 1].rightEdge.position.x + gapSize;
                pos.y = chunks[chunks.Count - 1].rightEdge.position.y;
                //  pos.y = -5;
            }

            //  if (isStartingPlatform)
            //  {
            //     pos.y += -2;
            //     nextPlatformHeight = 10;
            //   }

            int index = Random.Range(0, prefabChunks.Length);

            Chunk chunk = Instantiate(prefabChunks[index], pos, Quaternion.identity);
            chunks.Add(chunk);

            Platform[] newplatforms = chunk.GetComponentsInChildren<Platform>();
            foreach(Platform p in newplatforms)
            {
                platforms.Add(p.GetComponent<AABB>());
            }

            Spring[] newsprings = chunk.GetComponentsInChildren<Spring>();
            foreach(Spring s in newsprings)
            {
                springs.Add(s.GetComponent<AABB>());
            }

        }

        void LateUpdate() {
            //check player AABB against every platform AABB:
            foreach (AABB platform in platforms)
            {
                if (player.CollidesWith(platform))
                {
                    //collision!!!
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
                    //boing!!!
                    PlayerMovement mover = player.GetComponent<PlayerMovement>();
                    Spring s = spring.GetComponent<Spring>();


                    if (mover != null)
                    {
                        print($"springiness is {s.springiness}");
                        mover.LaunchUpwards(s.springiness);
                    }
                }
            }
                   
        } // end LateUpdate()


    }// end class
}// end namespace
