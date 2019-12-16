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
        /// The current collection of Level Chunks in the scene
        /// </summary>
        List<Chunks> chunks = new List<Chunks>();
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
        /// This is our array of health pickups
        /// </summary>
        List<AABB> healthUp = new List<AABB>();

        /// <summary>
        /// Collection of available level chunks
        /// </summary>
        public Chunks[] prefabChunks;

        /// <summary>
        /// Minimum gap in meters on the X axis between spawned objects
        /// </summary>
        public float gapSizeMin = 4;
        /// <summary>
        /// Maximum gap in meters on the X axis between spawned objects
        /// </summary>
        public float gapSizeMax = 7;

        /// <summary>
        /// Current health player has until Game Over
        /// </summary>
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
            if (chunks.Count < 5)
            {
               SpawnChunk();
            }

            RemoveOffscreenChunks();
           if (health == 0) Game.GameOver();

        }//End Update

        //Removes level chunks that go off the left side of the screen
        private void RemoveOffscreenChunks()
        {
            //Point on screen where objects will despawn
            float limitX = -10;

            for (int i = chunks.Count - 1; i >= 0; i--)
            {
                if (chunks[i].rightEdge.position.x < limitX)
                {
                    Chunks chunk = chunks[i];

                    // Remove references to the platform AABBs from the list.
                    Platform[] deadPlatforms = chunk.GetComponentsInChildren<Platform>();
                    foreach (Platform platform in deadPlatforms)
                    {
                        platforms.Remove(platform.GetComponent<AABB>());
                    }

                    // Remove references to the spring AABBs from the list.
                    Springs[] deadSprings = chunk.GetComponentsInChildren<Springs>();
                    foreach (Springs spring in deadSprings)
                    {
                        springs.Remove(spring.GetComponent<AABB>());
                    }

                    // Remove references to the spike AABBs from the list.
                    Spikes[] deadSpikes = chunk.GetComponentsInChildren<Spikes>();
                    foreach (Spikes spike in deadSpikes)
                    {
                        spikes.Remove(spike.GetComponent<AABB>());
                    }

                    // Remove references to the spike AABBs from the list.
                   HealthUp[] deadHealth = chunk.GetComponentsInChildren<HealthUp>();
                    foreach (HealthUp hp in deadHealth)
                    {
                        healthUp.Remove(hp.GetComponent<AABB>());
                    }

                    chunks.RemoveAt(i); // Remove the chunk from the list.
                    Destroy(chunk.gameObject); // Remove the chunk from the scene.
                }
            }
        }

        /// <summary>
        /// Instantiates a new chunk and adds the AABBs of child objects to lists.
        /// </summary>
        private void SpawnChunk()
        {
            float gapSize = Random.Range(gapSizeMin, gapSizeMax);

            Vector3 pos = new Vector3();

            if (chunks.Count > 0)
            {
                pos.x = chunks[chunks.Count - 1].rightEdge.position.x + gapSize;
                pos.y = chunks[chunks.Count - 1].rightEdge.position.y;

            }

            // Select a random prefab chunk from the array and add it to the list of instantiated chunks.
            int index = Random.Range(0, prefabChunks.Length);
            Chunks chunk = Instantiate(prefabChunks[index], pos, Quaternion.identity);
            chunks.Add(chunk);

            // Add references to the platform AABBs to the list.
            Platform[] newPlatforms = chunk.GetComponentsInChildren<Platform>();
            foreach (Platform p in newPlatforms)
            {
                platforms.Add(p.GetComponent<AABB>());
            }

            // Add references to the spring AABBs to the list.
            Springs[] newSprings = chunk.GetComponentsInChildren<Springs>();
            foreach (Springs s in newSprings)
            {
                springs.Add(s.GetComponent<AABB>());
            }

            // Add references to the spike AABBs to the list.
            Spikes[] newSpikes = chunk.GetComponentsInChildren<Spikes>();
            foreach (Spikes sp in newSpikes)
            {
                spikes.Add(sp.GetComponent<AABB>());
            }

            // Add references to the healthup AABBs to the list.
            HealthUp[] newHealthUp = chunk.GetComponentsInChildren<HealthUp>();
            foreach (HealthUp hp in newHealthUp)
            {
                healthUp.Add(hp.GetComponent<AABB>());
            }
        }


        public void LateUpdate()
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

                    PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();

                    playerMovement.SpringUp(10);

                }
            }

            foreach (AABB spike in spikes)
            {
                //Check Player AABB against every spike AABB:
                if (player.CollidesWith(spike))
                {
                    Vector3 fix = player.FindFix(spike);
                    player.BroadcastMessage("ApplyFix", fix);
                    //There is a collision!
                    Debug.Log("Player loses 1 health "+ health);
                    health--;

                    //TODO: move players position to pos.x-4 pos.y5
                    PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();

                    playerMovement.SpikeHit();

                }
            }

            foreach (AABB hp in healthUp)
            {
                //Check Player AABB against every platform AABB:
                if (player.CollidesWith(hp))
                {
                    //There is a collision!

                    /*HealthUp[] deadHealth = chunk.GetComponentsInChildren<HealthUp>();
                    foreach (HealthUp hp2 in deadHealth)
                    {
                        healthUp.Remove(hp2.GetComponent<AABB>());
                    }*/

                    if (health <= 3)
                    {
                        health = 3;
                    }
                    else
                    {
                        health++;
                    }
                    Debug.Log("Player gained health " + health);

                }
            }

        }//End LateUpdate

    }

}