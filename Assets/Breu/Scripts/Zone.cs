using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Breu {
    public class Zone : Pattison.Zone {

        new static public ZoneInfo info = new ZoneInfo()//set level/creator names in menus
        {
            zoneName = "Unthemed Temple of Indeterminate Purpose",
            creator = "Breu",
            level = "BreuScene"
        };

        public BreuAABB player;//AABB for the player character

        public BreuAABB LeftScreenEdge;//AABB use to determin the left edge
        
        List<BreuChunks> chunks = new List<BreuChunks>();//current chinks on screen
        List<BreuAABB> platforms = new List<BreuAABB>();//current AABBs of all platforms in scene
        List<BreuAABB> springs = new List<BreuAABB>();//current AABBs of all springs in scene
        List<BreuAABB> hazards = new List<BreuAABB>();//current AABBs of all hazards in scene
        List<BreuAABB> Oneways = new List<BreuAABB>();//current AABBs of all OneWay platforms in scene
        List<BreuAABB> PickUps = new List<BreuAABB>();//curren AABBs of all PickUps in scene

        public BreuChunks[] prefabChunk;//level chunk that can be spawned

        public float SpringPower = 10;//how strong the push of a spring is
                
        public float XGapSizeMin = 2;//minimum distance between chunks
        public float XGapSizeMax = 10;//maximum distance between chunks
        
        Camera cam;//camera used to determine the screen space

        void Awake()
        {
            cam = GetComponent<Camera>();//find camera
        }

        void Update()
        {
            if (chunks.Count < 5)//spawns more chunks if nessasary
            {
                SpawnChunk();
            }
            RemoveOffscreenChunks();//deletes old chunks
        }

        /// <summary>
        /// deletes chunks as the move off the left side of the screen
        /// </summary>
        private void RemoveOffscreenChunks()
        {
            float limitX;
            limitX = FindScreenLeft();//set where the left edge of the screen is

            for (int i = chunks.Count - 1; i >= 0; i--)//goes through the list of all chunks
            {
                if (chunks[i].rightedge.position.x < limitX)//compares the right edge of a chunk to left edge of screen
                {//if chunk if off left side of screen
                    BreuChunks chunk = chunks[i];

                    BreuPlatform[] deadPlatforms = chunk.GetComponentsInChildren<BreuPlatform>();//collects list of all platforms in chunk
                    foreach(BreuPlatform platform in deadPlatforms)//deletes all platforms in chunk
                    {
                        platforms.Remove(platform.GetComponent<BreuAABB>());
                    }

                    BreuSpring[] deadSprings = chunk.GetComponentsInChildren<BreuSpring>();//collects list of all spring in chunk
                    foreach (BreuSpring spring in deadSprings)//deletes all springs in chunk
                    {
                        springs.Remove(spring.GetComponentInChildren<BreuAABB>());
                    }

                    BreuSpring[] deadHazards = chunk.GetComponentsInChildren<BreuSpring>();//collects list of all spring in chunk
                    foreach (BreuSpring hazard in deadHazards)//deletes all springs in chunk
                    {
                        hazards.Remove(hazard.GetComponentInChildren<BreuAABB>());
                    }

                    BreuSpring[] deadOneWays = chunk.GetComponentsInChildren<BreuSpring>();//collects list of all spring in chunk
                    foreach (BreuSpring oneway in deadOneWays)//deletes all springs in chunk
                    {
                        Oneways.Remove(oneway.GetComponentInChildren<BreuAABB>());
                    }

                    BreuSpring[] deadPickup = chunk.GetComponentsInChildren<BreuSpring>();//collects list of all spring in chunk
                    foreach (BreuSpring pickup in deadPickup)//deletes all springs in chunk
                    {
                        PickUps.Remove(pickup.GetComponentInChildren<BreuAABB>());
                    }


                    chunks.RemoveAt(i);//remove chunk from list of chunk

                    Destroy(chunk.gameObject);//deletes chunk object, which should be empty
                }
            }
        }

        /// <summary>
        /// find left side of screen
        /// </summary>
        /// <returns>left edge of camera or -10</returns>
        private float FindScreenLeft()
        {
            Plane xy = new Plane(Vector3.forward, Vector3.zero);
            Ray ray = cam.ScreenPointToRay(new Vector3(0, Screen.height / 2));

            if (xy.Raycast(ray, out float dis))
            {
                Vector3 pt = ray.GetPoint(dis);

                LeftScreenEdge.transform.position = pt - new Vector3 (1, 0, 0);

                return pt.x;
                
                //Debug.DrawRay(ray.origin, ray.direction * dis, Color.red);
            }

            return -10;
        }

        /// <summary>
        /// Spawns new chunk prefabs and add AABBs present in prefabs to lists
        /// </summary>
        /// <param name="isStarting"></param>
        private void SpawnChunk()
        {
            //spawn new chunk:

            float GapSize = Random.Range(XGapSizeMin, XGapSizeMax);//set distance between chunks
                        
            Vector3 pos = new Vector3();

            if (chunks.Count > 0)
            {
                pos.x = chunks[chunks.Count - 1].rightedge.position.x + GapSize;
                pos.y = chunks[chunks.Count - 1].rightedge.position.y;                
            }

            int index = Random.Range(0, prefabChunk.Length);//selects chunk at random from array

            BreuChunks chunk = Instantiate(prefabChunk[index], pos, Quaternion.identity);//spawns chunk
            chunks.Add(chunk);//adds ne chunk to chunk list

            ///<summary>
            ///adds platforms from new chunk to list of platforms
            ///</summary>
            BreuPlatform[] newplatforms = chunk.GetComponentsInChildren<BreuPlatform>();
            foreach (BreuPlatform p in newplatforms)
            {
                platforms.Add(p.GetComponent<BreuAABB>());//add AABB for platforms into a list of platform AABBs
            }

            ///<summary>
            ///adds springs from new chunk to list of springs
            ///</summary>
            BreuSpring[] newsprings = chunk.GetComponentsInChildren<BreuSpring>();
            foreach (BreuSpring s in newsprings)
            {
                springs.Add(s.GetComponent<BreuAABB>());//add AABB for springs into a list of spring AABBs
            }

            ///<summary>
            ///adds hazards from new chunk to list of springs
            ///</summary>
            BreuHazard[] newHazards = chunk.GetComponentsInChildren<BreuHazard>();
            foreach (BreuHazard h in newHazards)
            {
                hazards.Add(h.GetComponent<BreuAABB>());
            }

            ///<summary>
            ///adds OneWays from new chunk to list of springs
            ///</summary>
            BreuOneWay[] newOneWays = chunk.GetComponentsInChildren<BreuOneWay>();
            foreach (BreuOneWay o in newOneWays)
            {
                Oneways.Add(o.GetComponent<BreuAABB>());
            }

            ///<summary>
            ///adds hazards from new chunk to list of springs
            ///</summary>
            BreuPickUp[] newPickUps = chunk.GetComponentsInChildren<BreuPickUp>();
            foreach (BreuPickUp p in newPickUps)
            {
                PickUps.Add(p.GetComponent<BreuAABB>());
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
                {//collision - spring
                    BreuPlayerMovement mover = player.GetComponent<BreuPlayerMovement>();
                    if (mover != null)
                    {
                        mover.launchUpwards(SpringPower);
                    }
                }

            }

            //Player AABB collision vs Hazrad AABB check
            foreach (BreuAABB hazard in hazards)
            {
                if (player.collidesWith(hazard))
                {//collision - hazard
                    Game.GameOver();
                }

            }

            //Player AABB collision vs OneWay AABB check
            foreach (BreuAABB oneway in Oneways)
            {
                if (player.collidesWith(oneway))
                {//collision - OneWay
                    BreuPlayerMovement mover = player.GetComponent<BreuPlayerMovement>();
                    if (mover != null)
                    {
                        Vector3 fix = player.findFix(oneway);
                        if (fix.y > 0)
                        {
                            player.BroadcastMessage("applyFix", fix);
                        }
                    }
                }

            }
            
            //Player AABB collision vs pickup AABB check
            foreach (BreuAABB pickup in PickUps)
            {
                if (player.collidesWith(pickup))
                {//collision - pickup
                    BreuPlayerMovement mover = player.GetComponent<BreuPlayerMovement>();
                    if (mover != null)
                    {
                        
                        if (pickup != null)
                        {
                            pickup.GetComponent<BreuAABB>().gameObject.SetActive(false);
                        }
                        
                        mover.PickUpGet();
                        
                    }
                }

            }

            if (player.collidesWith(LeftScreenEdge))
            {
                Game.GameOver();
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