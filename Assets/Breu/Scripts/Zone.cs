using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Breu {
    public class Zone : Pattison.Zone {

        new static public ZoneInfo info = new ZoneInfo()
        {
            zoneName = "The Water Temple",
            creator = "Breu",
            level = "BreuScene"
        };

        public BreuAABB player;
        
        List<BreuChunks> chunks = new List<BreuChunks>();//current chinks on screen
        List<BreuAABB> platforms = new List<BreuAABB>();//current AABBs of all platforms in scene
        List<BreuAABB> springs = new List<BreuAABB>();//current AABBs of all springs in scene

        public BreuChunks[] prefabChunk;//level chunk that can be spawned

        public float SpringPower = 10;//how strong the push of a spring is
                
        public float XGapSizeMin = 2;
        public float XGapSizeMax = 10;
        

        public Texture platformTexture;

        Camera cam;

        void Awake()
        {
            cam = GetComponent<Camera>();
        }

        void Start()
        {
            SpawnChunk(true);
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
            float limitX;
            limitX = FindScreenLeft();

            for (int i = chunks.Count - 1; i >= 0; i--)
            {
                if (chunks[i].rightedge.position.x < limitX)
                {
                    BreuChunks chunk = chunks[i];
                    BreuPlatform[] deadPlatforms = chunk.GetComponentsInChildren<BreuPlatform>();
                    foreach(BreuPlatform platform in deadPlatforms)
                    {
                        platforms.Remove(platform.GetComponent<BreuAABB>());
                    }
                    BreuSpring[] deadSprings = chunk.GetComponentsInChildren<BreuSpring>();
                    foreach (BreuSpring spring in deadSprings)
                    {
                        springs.Remove(spring.GetComponentInChildren<BreuAABB>());
                    }


                    chunks.RemoveAt(i);

                    Destroy(chunk.gameObject);
                }
            }
        }

        private float FindScreenLeft()
        {
            Plane xy = new Plane(Vector3.forward, Vector3.zero);
            Ray ray = cam.ScreenPointToRay(new Vector3(0, Screen.height / 2));

            if (xy.Raycast(ray, out float dis))
            {
                Vector3 pt = ray.GetPoint(dis);
                return pt.x;
                
                //Debug.DrawRay(ray.origin, ray.direction * dis, Color.red);
            }

            return -10;
        }

        private void SpawnChunk(bool isStarting = false)
        {
            //spawn new chunk:

            float GapSize = Random.Range(XGapSizeMin, XGapSizeMax);
                        
            Vector3 pos = new Vector3();

            if (chunks.Count > 0)
            {
                pos.x = chunks[chunks.Count - 1].rightedge.position.x + GapSize;
                pos.y = chunks[chunks.Count - 1].rightedge.position.y;
            }

            int index = Random.Range(0, prefabChunk.Length);

            BreuChunks chunk = Instantiate(prefabChunk[index], pos, Quaternion.identity);
            chunks.Add(chunk);

            BreuPlatform[] newplatforms = chunk.GetComponentsInChildren<BreuPlatform>();
            foreach (BreuPlatform p in newplatforms)
            {
                platforms.Add(p.GetComponent<BreuAABB>());
            }

            BreuSpring[] newsprings = chunk.GetComponentsInChildren<BreuSpring>();
            foreach (BreuSpring s in newsprings)
            {
                springs.Add(s.GetComponent<BreuAABB>());
            }

        }

        void LateUpdate()
        {

            //player AABB collision vs platform AABB check
            foreach(BreuAABB platorm in platforms)
            {
                if (player.collidesWith(platorm))
                {//collision - platform
                    Vector3 fix = player.findFix(platorm);
                    player.BroadcastMessage("applyFix", fix);
                }
            }


            //Player AABB collision vs Spring AABB check
            foreach (BreuAABB spring in springs)
            {
                if (player.collidesWith(spring))
                {//collision - plaform
                    BreuPlayerMovement mover = player.GetComponent<BreuPlayerMovement>();
                    if (mover != null)
                    {
                        mover.launchUpwards(SpringPower);
                    }
                }

            }

            /* using in old collision
            if (player.collidesWith(b))
            {
                player.GetComponent<MeshRenderer>().material.color = Color.red;
                b.GetComponent<MeshRenderer>().material.color = Color.red;

                Vector3 fix = player.findFix(b);
                //player.applyFix(fix);

                player.BroadcastMessage("applyFix", fix);

            }
            else
            {
                player.GetComponent<MeshRenderer>().material.color = Color.blue;
                b.GetComponent<MeshRenderer>().material.color = Color.blue;
            }
            */
        }


    }
}