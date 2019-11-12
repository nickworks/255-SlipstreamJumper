using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Myles {
    public class Zone : Pattison.Zone {

        new static public ZoneInfo info = new ZoneInfo() {
            zoneName = "Square Dimension",
            creator = "Danny Myles",
            level = "MylesScene"
        };


        public AABB player;        
        List<AABB> platforms = new List<AABB>();
       
        public GameObject prefabPlatform;
        

        public float gapSizeMin = 5;
        public float gapSizeMax = 15;
        public float heightMin = -2;
        public float heightMax = 5;
        public float widthMin = 2;
        public float widthMax = 10;






        Camera camera;

        void Awake()
        {            
            camera = GetComponent<Camera>();
        }

        void Start()
        {
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

            float limitX = -10;
            limitX = FindScreenLeftX();

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
            //Debug.DrawRay(ray.origin, ray.direction * dis, Color.yellow);

            if (xy.Raycast(ray, out float dis))
            {
                Vector3 pt = ray.GetPoint(dis);
                return pt.x;
            }

            return -10;
        }

        private void SpawnPlatform()
        {
            //spawn new platforms:

            float gapSize = Random.Range(gapSizeMin, gapSizeMax);
            float nextPlatformWidth = Random.Range(widthMin, widthMax);
            float platformHeight = Random.Range(heightMin, heightMax);
            
            Vector3 pos = new Vector3();

            if (platforms.Count > 0)
            {
                AABB lastPlatform = platforms[platforms.Count - 1];
                pos.x = lastPlatform.max.x + gapSize + nextPlatformWidth/2;

            }

            GameObject newPlatform = Instantiate(prefabPlatform, pos, Quaternion.identity);
            newPlatform.transform.localScale = new Vector3(nextPlatformWidth, 1, 1);
            newPlatform.transform.position = new Vector3(pos.x, platformHeight, 1);
            

            AABB aabb = newPlatform.GetComponent<AABB>();
            if (aabb)
            {
                platforms.Add(aabb);
                aabb.Recalc();
            }
        }

        

        void LateUpdate()
        {
            // check player AABB against every platform AABB:
           foreach(AABB platform in platforms)
            {
                if(player.CollidesWith(platform))
                {
                    // collision!!!
                    Vector3 fix = player.FindFix(platform);
                    player.BroadcastMessage("ApplyFix", fix);

                }
            }
           
        }
        
    }
}