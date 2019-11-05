using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Takens {
    public class Zone : Pattison.Zone {

        new static public ZoneInfo info = new ZoneInfo() {
            zoneName = "The Water Temple",
            creator = "Takens",
            level = "TakensScene"
        };


        public AABB player;
        List<AABB> platforms = new List<AABB>();

        public PlayerMovement PM;
        public GameObject prefabPlatform;

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
            if (platforms.Count < 20)
            {
                SpawnPlatform();
            }

            RemoveOffscreenPlatforms();
        }

        private void RemoveOffscreenPlatforms()
        {
            float limitY = FindScreenLeftBottom();
            //limitX = FindScreenLeftX();

            for (int i = platforms.Count - 1; i >= 0; i--)
            {
                if (platforms[i].max.y < limitY)
                {
                    AABB platform = platforms[i];

                    platforms.RemoveAt(i);
                    Destroy(platform.gameObject);
                }
            }
        }

        private float FindScreenLeftBottom()
        {
            Plane xy = new Plane(Vector3.forward, Vector3.zero);
            Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width/2, 0));

             Debug.DrawRay(ray.origin,ray.direction * 10, Color.yellow);

            if (xy.Raycast(ray, out float dis))
            {
                Vector3 pt = ray.GetPoint(dis);
                // limitX = pt.x;
                return pt.y;
            }
            else return -10;

         //   return limitX;
        }

        private void SpawnPlatform()
        {
            //spawn new platforms:

            float gapSize = Random.Range(gapSizeMin,gapSizeMax);
            float nextPlatformWidth = Random.Range(7, 10);

            Vector3 pos = new Vector3();
            

            if (platforms.Count > 0)
            {
                AABB lastPlatform = platforms[platforms.Count - 1];
                //pos.x = lastPlatform.max.x + gapSize + nextPlatformWidth/2;
                pos.y = lastPlatform.max.y + gapSize;
                pos.x += Random.Range(-15f, 15f);
            }

            GameObject newPlatform = Instantiate(prefabPlatform, pos, Quaternion.identity);
            float random = Random.Range(0f, 10f);
            if (random > 8)
            {
                //platform is passthrough!
                newPlatform.GetComponent<AABB>().currentType = AABB.ObjectType.Passthrough;
                newPlatform.transform.localScale = new Vector3(nextPlatformWidth, .2f, 1);
            }
            else
            {
                newPlatform.GetComponent<AABB>().currentType = AABB.ObjectType.Solid;
                newPlatform.transform.localScale = new Vector3(nextPlatformWidth, 1, 1);
            }

            

            AABB aabb = newPlatform.GetComponent<AABB>();
            if (aabb)
            {
                platforms.Add(aabb);
                aabb.Recalc();
            }
        }

        void LateUpdate() {
            //check player AABB against every platform AABB:
            foreach(AABB platform in platforms){
                if (player.CollidesWith(platform))
                {
                    Vector3 fix = player.FindFix(platform);


                    if (platform.currentType == AABB.ObjectType.Passthrough && !(fix.y > 0))
                    {
                       // Debug.Log("Passing Through!");
                    }
                    else
                    {
                        if((fix.y > 0) &&(PM.velocity.y > 0))
                        {
                            return;
                        }
                       
                        //collision!!!
                        player.BroadcastMessage("ApplyFix", fix);
                    }
                   
                    
                    
                }

            }
        }

    }
}
