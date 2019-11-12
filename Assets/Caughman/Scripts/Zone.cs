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
        /// This is our array of spikes AABB
        /// </summary>
        List<AABB> spikes = new List<AABB>();
        /// <summary>
        /// This is our array of springs AABB
        /// </summary>
        List<AABB> springs = new List<AABB>();
        /// <summary>
        /// This is our Prefab Platform object
        /// </summary>
        public GameObject prefabPlatform;
        /// <summary>
        /// This is our Prefab Spike object
        /// </summary>
        public GameObject prefabSpike;
        /// <summary>
        /// This is our Prefab Spring object
        /// </summary>
        public GameObject prefabSpring;
        /// <summary>
        /// Minimum gap in meters on the X axis between spawned objects
        /// </summary>
        public float gapSizeMin = 2;
        /// <summary>
        /// Maximum gap in meters on the X axis between spawned objects
        /// </summary>
        public float gapSizeMax = 10;
        /// <summary>
        /// Minimum platform size in meters
        /// </summary>
        public float platformSizeMin = 4;
        /// <summary>
        /// Maximum platform size in meters
        /// </summary>
        public float platformSizeMax = 10;

        public float health = 3;


        void Awake()
        { 
        }//End Awake

        void Start()
        {
        }//End Start

        void Update()
        {
            //if there are less than 5 platforms, spawn a platform
            if (platforms.Count < 5)
            {
                SpawnPlatform();
            }
            //if there are less than 5 spikes, spawn a spike
            if (spikes.Count < 5)
            {
                SpawnSpike();
            }
            //if there are less than 5 spring, spawn a spring
            if (springs.Count < 5)
            {
                SpawnSpring();
            }
            RemoveOffscreenPlatforms();
            RemoveOffscreenSpikes();
            RemoveOffscreenSprings();

           if (health == 0) Game.gameOver();

        }//End Update

        //Removes spikes that go off the left side of the screen
        private void RemoveOffscreenSpikes()
        {
            //Point off camera where objects will despawn
            float limitX = -10;

            for (int i = spikes.Count - 1; i >= 0; i--)
            {
                if (spikes[i].max.x < limitX)
                {
                    AABB spike = spikes[i];

                    spikes.RemoveAt(i);
                    Destroy(spike.gameObject);
                }
            }
        }

            //Spawn New Spikes;
        private void SpawnSpike()
        {

            // The gap between spawned spikes in a random range of gapSizeMin and gapSizeMax
            float gapSize = Random.Range(gapSizeMin, gapSizeMax);
            float spikeGap = Random.Range(3, 5);
            //the position of the spikes spawned
            Vector3 pos = new Vector3();

            if (spikes.Count > 0)
            {
                AABB lastSpike = spikes[spikes.Count - 1];
                pos.x = lastSpike.max.x + gapSize+ spikeGap;
                pos.y = Random.Range(1, 3);
            }

            GameObject newSpike = Instantiate(prefabSpike, pos, Quaternion.identity);
            newSpike.transform.localScale = new Vector3(1, 1, 1);

            AABB aabb = newSpike.GetComponent<AABB>();
            if (aabb)
            {
                spikes.Add(aabb);
                aabb.Recalc();
            }
        }



            //Spawn New Springs;
        private void SpawnSpring()
        {

            // The gap between spawned springs in a random range of gapSizeMin and gapSizeMax
            float gapSize = Random.Range(gapSizeMin, gapSizeMax);
            float springGap = Random.Range(3, 5);
            //the position of the springs spawned
            Vector3 pos = new Vector3();

            if (springs.Count > 0)
            {
                AABB lastSpring = springs[springs.Count - 1];
                pos.x = lastSpring.max.x + gapSize + springGap;
                pos.y = Random.Range(-1, 3);
            }

            GameObject newSpring = Instantiate(prefabSpring, pos, Quaternion.identity);
            newSpring.transform.localScale = new Vector3(1, 1, 1);

            AABB aabb = newSpring.GetComponent<AABB>();
            if (aabb)
            {
                springs.Add(aabb);
                aabb.Recalc();
            }
        }

        //Removes springs that go off the left side of the screen
        private void RemoveOffscreenSprings()
        {
            //Point off camera where objects will despawn
            float limitX = -10;

            for (int i = springs.Count - 1; i >= 0; i--)
            {
                if (springs[i].max.x < limitX)
                {
                    AABB spring = springs[i];

                    springs.RemoveAt(i);
                    Destroy(spring.gameObject);
                }
            }
        }


            //Spawn New Platforms;
        private void SpawnPlatform()
        {

            // The gap between spawned platforms in a random range of gapSizeMin and gapSizeMax
            float gapSize = Random.Range(gapSizeMin,gapSizeMax);
            //the Width of the next spawned platform
            float nextPlatformWidth = Random.Range(platformSizeMin,platformSizeMax);
            //the position of the platform spawned
            Vector3 pos = new Vector3();

            if (platforms.Count > 0)
            {
                AABB lastPlatform = platforms[platforms.Count - 1];
                pos.x = lastPlatform.max.x + gapSize + nextPlatformWidth/2;
                pos.y = 0;
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

        //Removes platforms that go off the left side of the screen
        private void RemoveOffscreenPlatforms()
        {
            //Point off camera where objects will despawn
            float limitX = -10;

            for (int i = platforms.Count -1; i >= 0; i--)
            {
                if (platforms[i].max.x < limitX)
                {
                    AABB platform = platforms[i];

                    platforms.RemoveAt(i);
                    Destroy(platform.gameObject);
                }
            }
        }//End RemoveOffscreenPlatforms

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

            foreach (AABB spring in springs)
            {
                //Check Player AABB against every spring AABB:
                if (player.CollidesWith(spring))
                {
                    //There is a collision!
                    Vector3 fix = player.FindFix(spring);
                    player.BroadcastMessage("ApplyFix", fix);
                }
            }

            foreach (AABB spike in spikes)
            {
                //Check Player AABB against every spike AABB:
                if (player.CollidesWith(spike))
                {
                    //There is a collision!
                    Debug.Log("Player loses 1 health "+ health);

                    //TODO: move players position to pos.x-4 pos.y5
                    //TODO: remove 1 health
                    health--;
                

                }
            }

        }//End LateUpdate

    }

}