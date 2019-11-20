﻿using System.Collections.Generic;
using UnityEngine;

namespace Powers
{
    public class Zone : Pattison.Zone
    {

        new static public ZoneInfo info = new ZoneInfo()
        {
            zoneName = "Super Fast Pup",
            creator = "Aaron Powers",
            level = "PowersScene"
        };

        public AABB player;
        public PlayerMovement playerMove;
        public AudioSource audioPlayer;

        [Space(10)]
        //these are used to refer to audio clips played:
        public AudioClip powerupSFX;
        public AudioClip gameOverSFX;

        [Space(10)]

        //these gameobjects are used to interact with the ui, showing what powerups are obtained
        public GameObject slowTimeUI;
        public GameObject shieldUI;

        //these lists are used to refer to different object types
        List<AABB> Platforms = new List<AABB>();
        List<AABB> Edges = new List<AABB>();
        List<AABB> Bottom = new List<AABB>();
        List<AABB> Hazards = new List<AABB>();
        List<AABB> Springs = new List<AABB>();
        List<AABB> Shields = new List<AABB>();
        List<AABB> SlowTimes = new List<AABB>();

        [Space(10)]

        //these variables refer to the objects spawned for each object type
        public GameObject screenBounds;
        public GameObject bottomBounds;
        public GameObject buildingPlatform;
        public GameObject elevatorPlatform;
        public GameObject hazardObject;
        public GameObject springObject;
        public GameObject shieldObject;
        public GameObject slowTimeObject;

        [Space(10)]

        public float gapSizeMin = 2;
        public float gapSizeMax = 5;

        private GameObject newPlatform;
        private bool firstPlatform = true;

        Camera gameCam;

        void Awake()
        {
            gameCam = GetComponent<Camera>();
        }

        void FixedUpdate()
        {
            CheckCollisons();
        }

        void Update()
        {
            //if player is not dead, continue playing
            if (!playerMove.isDead)
            {
                if (Platforms.Count < 5) SpawnPlatform();

                //remove off screen objects by going through the different list of objects
                RemoveOffScreenObjects(Platforms);
                RemoveOffScreenObjects(Hazards);
                RemoveOffScreenObjects(Springs);
                RemoveOffScreenObjects(Shields);
                RemoveOffScreenObjects(SlowTimes);

                //check if powerups are enabled
                if (playerMove.slowTimePowerup == 0) slowTimeUI.SetActive(false);
                else slowTimeUI.SetActive(true);

                if (playerMove.shieldPowerup == false) shieldUI.SetActive(false);
                else shieldUI.SetActive(true);
            }
            //otherwise, end game
            else
            {
                Time.timeScale = 1;
                Game.GameOver();
            }
        }

        private void RemoveOffScreenObjects(List<AABB> objectList)
        {
            float limitX = -20;
            limitX = FindScreenLeftX();

            //remove objects if they are offscreen
            for (int i = objectList.Count - 1; i >= 0; i--)
            {
                if (objectList[i].Max.x < limitX)
                {
                    AABB offscreenObject = objectList[i];

                    objectList.RemoveAt(i);
                    Destroy(offscreenObject.gameObject);
                }

            }
        }

        private float FindScreenLeftX()
        {
            Plane xy = new Plane(Vector3.forward, Vector3.zero);
            Ray ray = gameCam.ScreenPointToRay(new Vector3(0, Screen.height / 2));

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

            Vector3 pos = new Vector3();

            int platformType;

            //checks if it's the first platform. if so, spawn building so player drops safely onto playing field
            if (firstPlatform)
            {
                platformType = 0;

                newPlatform = Instantiate(screenBounds, new Vector3(10.75f, 0, 0), Quaternion.identity);
                AABB boundsAABB = newPlatform.GetComponent<AABB>();
                Edges.Add(boundsAABB);
                boundsAABB.Recalc();

                newPlatform = Instantiate(screenBounds, new Vector3(-10.75f, 0, 0), Quaternion.identity);
                boundsAABB = newPlatform.GetComponent<AABB>();
                Edges.Add(boundsAABB);
                boundsAABB.Recalc();

                newPlatform = Instantiate(bottomBounds, new Vector3(0, -7.5f, 0), Quaternion.identity);
                boundsAABB = newPlatform.GetComponent<AABB>();
                Bottom.Add(boundsAABB);
                boundsAABB.Recalc();

            }
            else platformType = Random.Range(0, 2);

            if (Platforms.Count > 0)
            {
                AABB lastPlatform = Platforms[Platforms.Count - 1];

                //change value at end (prev obstacle length) depending on what the next obstacle will be.
                if (platformType == 0) pos.x = lastPlatform.Max.x + gapSize + 2.5f;
                else if (platformType == 1) pos.x = lastPlatform.Max.x + gapSize + 0.5f;
            }

            //change what height the next obstacle will be at depending on what type it is.
            if (platformType == 0) pos.y = Random.Range(-8, -4);
            else if (platformType == 1) pos.y = Random.Range(-1, 0.5f);
            pos.z = -0.01f;
            
            //create the new platform, depending on what type it is.
            if (platformType == 0) newPlatform = Instantiate(buildingPlatform, pos, Quaternion.identity);
            else if (platformType == 1) newPlatform = Instantiate(elevatorPlatform, pos, Quaternion.identity);

            AABB aabb = newPlatform.GetComponent<AABB>();

            if (aabb)
            {
                Platforms.Add(aabb);
                aabb.Recalc();
            }

            //if regular building, go through process of possibly spawning hazards or springs
            if(platformType == 0 && !firstPlatform)
            {
                //see if a hazard or spring will be added:
                int specialObjectChance = Random.Range(0, 12);
                //if 1, spawn hazard. if 2, spawn spring. if 3, spawn slow time powerup. if 4, spawn shield powerup. otherwise, spawn nothing
                if (specialObjectChance == 1 || specialObjectChance == 0)
                {
                    newPlatform = Instantiate(hazardObject, new Vector3(pos.x + Random.Range(-1.75f, 1.75f), pos.y + 5.25f, pos.z), Quaternion.identity);
                    aabb = newPlatform.GetComponent<AABB>();
                    Hazards.Add(aabb);
                    aabb.Recalc();
                }
                else if (specialObjectChance == 2 || specialObjectChance == 3)
                {
                    newPlatform = Instantiate(springObject, new Vector3(pos.x + Random.Range(1.5f, 1.75f), pos.y + 5.25f, pos.z), Quaternion.identity);
                    aabb = newPlatform.GetComponent<AABB>();
                    Springs.Add(aabb);
                    aabb.Recalc();
                }
            }

            //see if a powerup will be added:
            int powerupObjectChance = Random.Range(0, 7);
            if (powerupObjectChance <= 1 && !firstPlatform)
            {
                if(powerupObjectChance == 0)
                {
                    //spawn slow time powerup and add to collection
                    if (platformType == 0) newPlatform = Instantiate(shieldObject, new Vector3(pos.x + Random.Range(-1.75f, 1.75f), pos.y + Random.Range(5.5f, 8.5f), pos.z), Quaternion.identity);
                    else newPlatform = Instantiate(shieldObject, new Vector3(pos.x + Random.Range(-1.25f, 1.25f), pos.y + Random.Range(3f, 5f), pos.z), Quaternion.identity);
                    aabb = newPlatform.GetComponent<AABB>();
                    Shields.Add(aabb);
                    aabb.Recalc();
                }
                else
                {
                    //spawn shield powerup and add to collection
                    if (platformType == 0) newPlatform = Instantiate(slowTimeObject, new Vector3(pos.x + (Random.Range(-1.75f, 1.75f)), pos.y + Random.Range(5.5f, 8.5f), pos.z), Quaternion.identity);
                    else newPlatform = Instantiate(slowTimeObject, new Vector3(pos.x + Random.Range(-1.25f, 1.25f), pos.y + Random.Range(3f, 5f), pos.z), Quaternion.identity);
                    aabb = newPlatform.GetComponent<AABB>();
                    SlowTimes.Add(aabb);
                    aabb.Recalc();
                }
            }

            //deactivate first platform var so powerups and hazards can spawn
            firstPlatform = false;
        }

        void CheckCollisons()
        {
            bool collide = false;

            // check player AABB against every platform AABB:
            foreach (AABB platform in Platforms)
            {
                if (player.CollidesWith(platform))
                {
                    Vector3 fix = player.FindFix(platform);
                    player.BroadcastMessage("ApplyFix", fix);
                    collide = true;
                }
            }

            //if player has not collided at all, player is not grounded
            if (!collide) playerMove.isGrounded = false;

            // check player AABB against edge bounds:
            foreach (AABB edge in Edges)
            {
                //if player collides, stop them
                if (player.CollidesWith(edge))
                {
                    Vector3 fix = player.FindFix(edge);
                    player.BroadcastMessage("ApplyFix", fix);
                    collide = true;
                }
            }

            // check if player AABB has fallen out of bounds
            foreach (AABB bottom in Bottom)
            {
                //if player collides, stop them
                if (player.CollidesWith(bottom))
                {
                    playerMove.isDead = true;
                }
            }

            // check player AABB against every hazard AABB:
            for (int i = Hazards.Count - 1; i >= 0; i--)
            {
                AABB hazard = Hazards[i];

                //if player collides, they are dead
                if (player.CollidesWith(hazard))
                {

                    //check to see if player has shield powerup. if so, deactivate it. if not, end game.
                    if (playerMove.shieldPowerup == true) playerMove.shieldPowerup = false;
                    else if (playerMove.shieldPowerup == false)
                    {
                        playerMove.isDead = true;

                        //play sfx:
                        audioPlayer.PlayOneShot(gameOverSFX, 1f);
                    }

                    Hazards.RemoveAt(i);
                    Destroy(hazard.gameObject);
                }

            }

            // check player AABB against every spring AABB:
            for (int i = Springs.Count - 1; i >= 0; i--)
            {
                AABB spring = Springs[i];

                //if player collides, they will bounce
                if (player.CollidesWith(spring))
                {
                    Vector3 fix = player.FindFix(spring);
                    player.BroadcastMessage("ApplyFix", fix);

                    //bounce player
                    playerMove.hitSpring = true;

                    Springs.RemoveAt(i);
                    Destroy(spring.gameObject);
                }
            }

            // check player AABB against every slow time AABB:
            for (int i = SlowTimes.Count - 1; i >= 0; i--)
            {
                AABB slowTime = SlowTimes[i];

                //if player collides, activate time slow powerup
                if (player.CollidesWith(slowTime))
                {
                    //set slow time:
                    playerMove.slowTimePowerup = 300;

                    //play sfx:
                    audioPlayer.PlayOneShot(powerupSFX, 0.75f);

                    SlowTimes.RemoveAt(i);
                    Destroy(slowTime.gameObject);
                }
            }

            // check player AABB against every shield AABB:
            for (int i = Shields.Count - 1; i >= 0; i--)
            {
                AABB shield = Shields[i];

                //if player collides, activate time slow powerup
                if (player.CollidesWith(shield))
                {
                    //active shield powerup:
                    playerMove.shieldPowerup = true;

                    //play sfx:
                    audioPlayer.PlayOneShot(powerupSFX, 0.75f);

                    Shields.RemoveAt(i);
                    Destroy(shield.gameObject);
                }
            }
        }
    }
}