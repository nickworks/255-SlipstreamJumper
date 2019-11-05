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
        //public BreuAABB b; //used in old colision
        List<BreuAABB> platforms = new List<BreuAABB>();
        public GameObject platformPrefab;

        public float startingPlatformWidth = 10;
        public float startingPlatformHeight = 5;

        public float platformXSizeMin = 1;
        public float platformXSizeMax = 10;
        
        public float XGapSizeMin = 2;
        public float XGapSizeMax = 10;

        public float YGapSizeMin = 2;
        public float YGapSizeMax = 10;

        public Texture platformTexture;

        Camera camera;

        void Awake()
        {
            camera = GetComponent<Camera>();
        }

        void Start()
        {
            SpawnPlatform(true);
        }

        void Update()
        {
            if (platforms.Count < 5)
            {
                SpawnPlatform();
            }
            removeOffScreenPlatforms();
        }

        private void removeOffScreenPlatforms()
        {
            float limitX;
            limitX = FindScreenLeft();

            for (int i = platforms.Count - 1; i >= 0; i--)
            {
                if (platforms[i].Max.x < limitX)
                {
                    BreuAABB platform = platforms[i];
                    platforms.RemoveAt(i);

                    Destroy(platform.gameObject);
                }
            }
        }

        private float FindScreenLeft()
        {
            Plane xy = new Plane(Vector3.forward, Vector3.zero);
            Ray ray = camera.ScreenPointToRay(new Vector3(0, Screen.height / 2));

            if (xy.Raycast(ray, out float dis))
            {
                Vector3 pt = ray.GetPoint(dis);
                return pt.x;
                
                //Debug.DrawRay(ray.origin, ray.direction * dis, Color.red);
            }

            return -10;
        }

        private void SpawnPlatform(bool isStarting = false)
        {
            //spawn new platform:

            float XGapSize = Random.Range(XGapSizeMin, XGapSizeMax);
            float YGapSize = Random.Range(YGapSizeMin, YGapSizeMax);
            float nextPlatformWidth = Random.Range(platformXSizeMin, platformXSizeMax);
            float nextPlatformHeight = YGapSize;

            if (isStarting == true)
            {
                nextPlatformWidth = startingPlatformWidth;
                nextPlatformHeight = startingPlatformHeight;
            }


            Vector3 pos = new Vector3();

            if (platforms.Count > 0)
            {
                BreuAABB lastPlatform = platforms[platforms.Count - 1];
                pos.x = lastPlatform.Max.x + XGapSize + nextPlatformWidth/2;
                pos.y = -nextPlatformHeight/2;
            }

            GameObject newPlatform = Instantiate(platformPrefab, pos, Quaternion.identity);
            newPlatform.transform.localScale = new Vector3(nextPlatformWidth, nextPlatformHeight * 2, 1);

            MeshRenderer meshPlatform = newPlatform.GetComponent<MeshRenderer>();
            if (meshPlatform)
            {
                meshPlatform.material.SetTexture("_MainTex", platformTexture);
                meshPlatform.material.SetTextureScale("_MainTex", new Vector2 ( 1, .5f));
            }

            BreuAABB aabb = newPlatform.GetComponent<BreuAABB>();

            if (aabb != null)
            {
                platforms.Add(aabb);
                aabb.recalc();
            }
        }

        void LateUpdate()
        {

            //player AABB collision vs platform AABB check
            foreach(BreuAABB platorm in platforms)
            {
                if (player.collidesWith(platorm))
                {
                    Vector3 fix = player.findFix(platorm);
                    player.BroadcastMessage("applyFix", fix);
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