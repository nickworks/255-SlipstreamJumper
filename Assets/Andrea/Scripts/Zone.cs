using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Andrea {
    public class Zone : Pattison.Zone {

        new static public ZoneInfo info = new ZoneInfo() {
            zoneName = "The Water Temple",
            creator = "Student Lastname",
            level = "AndreaScene"
        };

        public AABB player;
        List<AABB> platforms = new List<AABB>();

        public GameObject prefabPlatform;

        /// <summary>
        /// The minimum 
        /// </summary>
        public float gapSizeMin = 2;
        public float gapSizeMax = 9;

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
            if (platforms.Count < 5)
            {
                SpawnPlatform();
            }
            RemoveOffScreenPlatforms();
        }

        private void RemoveOffScreenPlatforms()
        {
            float limitX = -20;
            limitX = FindScreenLeftX();
            for (int i = platforms.Count - 1; i >= 0; i--)
            {
                if (platforms[i].Max.x < limitX)
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
            Ray ray = cam.ScreenPointToRay(new Vector3(0, Screen.height / 2));

            Debug.DrawRay(ray.origin, ray.direction * 10, Color.red);
            if (xy.Raycast(ray, out float dis))
            {
                Vector3 pt = ray.GetPoint(dis);
                return pt.x;
            }

            return -20;
        }

        private void SpawnPlatform()
        {
            // spawn new platforms:

            float gapSize = Random.Range(gapSizeMin, gapSizeMax);
            float nextPlatformWidth = Random.Range(3,10);
            float nextPlatformHeight = Random.Range(1, 6);

            Vector3 pos = new Vector3();

            if (platforms.Count > 0)
            {
                AABB lastPlatform = platforms[platforms.Count - 1];
                pos.x = lastPlatform.Max.x + gapSize + (nextPlatformWidth / 2);
                pos.y = Random.Range(-2,3);
                //pos.y = lastPlatform.Min.y + (nextPlatformHeight / 2) + Random.Range(-2, 3);
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

        void LateUpdate() 
        {
            // check player AABB against every platform AABB:
            foreach (AABB platform in platforms)
            {
                if (player.CollidesWith(platform))
                {
                    Vector3 fix = player.FindFix(platform);
                    player.BroadcastMessage("ApplyFix", fix);
                }
            }
        }
    }
}