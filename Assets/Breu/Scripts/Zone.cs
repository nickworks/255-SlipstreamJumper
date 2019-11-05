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

        public float platformSizeMin = 1;
        public float platformSizeMax = 10;

        public float gapSizeMin = 2;
        public float gapSizeMax = 10;
        Camera camera;

        void Awake()
        {
            camera = GetComponent<Camera>();
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

        private void SpawnPlatform()
        {
            //spawn new platform:

            float gapSize = Random.Range(gapSizeMin, gapSizeMax);
            float nextPlatformWidth = Random.Range(platformSizeMin, platformSizeMax);

            Vector3 pos = new Vector3();

            if (platforms.Count > 0)
            {
                BreuAABB lastPlatform = platforms[platforms.Count - 1];
                pos.x = lastPlatform.Max.x + gapSize + nextPlatformWidth/2;
            }

            GameObject newPlatform = Instantiate(platformPrefab, pos, Quaternion.identity);
            newPlatform.transform.localScale = new Vector3(nextPlatformWidth, 1, 1);

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