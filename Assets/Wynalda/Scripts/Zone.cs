using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wynalda
{
    public class Zone : Pattison.Zone {

        new static public ZoneInfo info = new ZoneInfo() {
            zoneName = "The Water Temple",
            creator = "Takens",
            level = "TakensScene"
        };


        public AABB player;
        List<AABB> platforms = new List<AABB>();

        public GameObject prefabPlatform;

        public float gapSizeMin = 2;
        public float gapSizeMax = 10;

        Camera camera;

        void Awake()
        {
            camera = GetComponent<Camera>();
        }

        void Start() {
            
        }

        void Update()
        {
            if (platforms.Count < 5)
            {
                SpawnPlatform();
            }

            RemoveOffscreenPlatforms();
        }

        private void RemoveOffscreenPlatforms()
        {
            float limitX = FindScreenLeftX();
            //limitX = FindScreenLeftX();

            for (int i = platforms.Count - 1; i >= 0; i--)
            {
                if (platforms[i].max.x < limitX)
                {
                    AABB platform = platforms[i];

                    platforms.RemoveAt(i);
                    Destroy(platform.gameObject);
                }
            }
        }

        private float FindScreenLeftX()
        {
            Plane xy = new Plane(Vector3.forward, Vector3.zero);
            Ray ray = camera.ScreenPointToRay(new Vector3(0, Screen.height / 2));

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

        private void SpawnPlatform()
        {
            //spawn new platforms:

            float gapSize = Random.Range(gapSizeMin,gapSizeMax);
            float nextPlatformWidth = 10;

            Vector3 pos = new Vector3();

            if (platforms.Count > 0)
            {
                AABB lastPlatform = platforms[platforms.Count - 1];
                pos.x = lastPlatform.max.x + gapSize + nextPlatformWidth/2;
            }

            GameObject newPlatform = Instantiate(prefabPlatform, pos, Quaternion.identity);
            newPlatform.transform.localScale = new Vector3(nextPlatformWidth, 1, 1);

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
                    //collision!!!
                    Vector3 fix = player.FindFix(platform);
                    player.BroadcastMessage("ApplyFix", fix);
                }

            }
        }

    }
}
