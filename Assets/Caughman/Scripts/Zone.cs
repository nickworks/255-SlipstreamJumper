using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Caughman
{
    public class Zone : Pattison.Zone
    {

        new static public ZoneInfo info = new ZoneInfo()
        {
            zoneName = "Spikeville",
            creator = "Caughman",
            level = "CaughmanScene"
        };

        /// <summary>
        /// This is our players AABB
        /// </summary>
        public AABB player;
        /// <summary>
        ///This is our array of platforms AABB
        /// </summary>
        List<AABB> platforms = new List<AABB>();

        /// <summary>
        /// This is our Prefab Platform object
        /// </summary>
        public GameObject prefabPlatform;
        /// <summary>
        /// 
        /// </summary>
        public float gapSizeMin = 2;
        /// <summary>
        /// 
        /// </summary>
        public float gapSizeMax = 10;
        /// <summary>
        /// 
        /// </summary>
        Camera camera;
        /// <summary>
        /// 
        /// </summary>
        public float delayBetweenPlatforms = 1;
        /// <summary>
        /// 
        /// </summary>
        float timerPlatforms = 0;

        void Awake()
        { 
            camera = GetComponent<Camera>();
        }//End Awake

        void Start()
        {
        }//End Start

        void Update()
        {
            if (platforms.Count < 5)
            {
                SpawnPlatform();
            }

            RemoveOffscreenPlatforms();
        }//End Update

        private void RemoveOffscreenPlatforms()
        {

            float limitX = FindScreenLeftX();

            for (int i = platforms.Count; i >= 0; i--)
            {
                if (platforms[i].max.x < limitX)
                {
                    AABB platform = platforms[i];

                    platforms.RemoveAt(i);
                    Destroy(platforms[i].gameObject);
                }
            }
        }//End RemoveOffscreenPlatforms

        private float FindScreenLeftX()
        {
            Plane xy = new Plane(Vector3.forward, Vector3.zero);
            Ray ray = camera.ScreenPointToRay(new Vector3(0, Screen.height / 2));

            if (xy.Raycast(ray, out float dis))
            {
                Vector3 pt = ray.GetPoint(dis);
                return pt.x;
            }

            return -10;
        }//End FindSCreenLeftX

        private void SpawnPlatform()
        {
            //Spawn New Platforms;

            // The gap between spawned platforms in a random range of gapSizeMin and gapSizeMax
            float gapSize = Random.Range(gapSizeMin,gapSizeMax);
            //the Width of the next spawned platform
            float nextPlatformWidth = 10;
            //the position of the platform spawned
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
        }//End SpawnPlatform

        void LateUpdate()
        {
         foreach(AABB platform in platforms)
            {
                //Check Player AABB against every platform AABB:
                if (player.CollidesWith(platform))
                {
                    //There is a collision!
                    Vector3 fix = player.FindFix(platform);
                    player.BroadcastMessage("ApplyFix", fix);
                }
            }
        }//End LateUpdate

    }

}