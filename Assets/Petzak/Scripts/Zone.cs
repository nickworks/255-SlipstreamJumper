using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Petzak
{
    /// <summary>
    /// Contains all game objects. Controls AABB spawning/despawning. Handles collisions. Updates HUD elements.
    /// </summary>
    public class Zone : Pattison.Zone
    {
        /// <summary>
        /// Stores the zoneName, creator, and level
        /// </summary>
        static public ZoneInfo info = new ZoneInfo()
        {
            zoneName = "Cube Jumper",
            creator = "Alec Petzak",
            level = "PetzakScene"
        };
        /// <summary>
        /// The AABB of the player
        /// </summary>
        public PetzakAABB _player;
        /// <summary>
        /// The PlayerMovement of the player
        /// </summary>
        PetzakPlayerMovement mover;
        /// <summary>
        /// The current chunks in the scene.
        /// </summary>
        List<PetzakChunk> _chunks = new List<PetzakChunk>();
        /// <summary>
        /// The AABBs of all platforms in the scene.
        /// </summary>
        List<PetzakAABB> _platforms = new List<PetzakAABB>();
        /// <summary>
        /// The AABBs of all springs in the scene.
        /// </summary>
        List<PetzakAABB> _springs = new List<PetzakAABB>();
        /// <summary>
        /// The AABBs of all spikes in the scene.
        /// </summary>
        List<PetzakAABB> _spikes = new List<PetzakAABB>();
        /// <summary>
        /// The life pill. Continuously respawns ahead of the player at random intervals.
        /// Picking it up does nothing if you're already green.
        /// </summary>
        PetzakLifePill pill;
        /// <summary>
        /// The AABB of the life pill.
        /// </summary>
        public PetzakAABB lifePill;
        /// <summary>
        /// Contains the chunks that will spawn one after another.
        /// </summary>
        public PetzakChunk[] prefabs;
        /// <summary>
        /// The hud element to indicate double jump reset.
        /// </summary>
        public GameObject bar;
        /// <summary>
        /// The hud element to show the amount of chunks cleared.
        /// </summary>
        public GameObject scoreText;
        /// <summary>
        /// Used to reset player color once they take damage.
        /// </summary>
        public Material playerColor;
        /// <summary>
        /// Turns true when life pill is picked up.
        /// When true it will save the player from spike damage or falling.
        /// </summary>
        bool hasExtraLife = false;
        /// <summary>
        /// Has player left the spikes range after taking damage.
        /// </summary>
        bool touchingSpikes = false;
        /// <summary>
        /// Amount of chunks destroyed
        /// </summary>
        int score = 0;
        /// <summary>
        /// Amount of chunks spawned
        /// </summary>
        int chunkCount = 0;
        /// <summary>
        /// Range that chunks will despawn at
        /// </summary>
        float minLimit = -100;
        /// <summary>
        /// Next chunk to be spawned. Goes from 0-5 then randomly from 1-5.
        /// </summary>
        int chunkIndex = 0;
        /// <summary>
        /// Main camera. Follows the player.
        /// </summary>
        Camera _cam;

        /// <summary>
        /// Instantiates _cam, mover, and pill objects on startup.
        /// </summary>
        void Awake()
        {
            _cam = GetComponent<Camera>();
            mover = _player.GetComponent<PetzakPlayerMovement>();
            pill = lifePill.GetComponentInParent<PetzakLifePill>();
        }

        /// <summary>
        /// Called every frame. Spawns and removes chunks.
        /// </summary>
        void Update()
        {
            if (_chunks.Count < 3)
                SpawnChunk();
            RemoveOffScreenChunks();
        }

        /// <summary>
        /// Returns the x location of the screens left edge
        /// </summary>
        /// <returns></returns>
        private float FindScreenLeftX()
        {
            Plane xy = new Plane(Vector3.forward, Vector3.zero);
            Ray ray = _cam.ScreenPointToRay(new Vector3(0, Screen.height / 2));
            if (xy.Raycast(ray, out float dis))
            {
                Vector3 pt = ray.GetPoint(dis);
                return pt.x;
            }
            return -10;
        }

        /// <summary>
        /// Spawns new chunk, then adds the chunk's children to the game.
        /// </summary>
        private void SpawnChunk()
        {

            float gapSizeMin = 1;
            float gapSizeMax = 2;


            float gapSize = Random.Range(gapSizeMin, gapSizeMax);
            Vector3 pos = new Vector3(-5, -3, 0);

            if (_chunks.Count > 0)
            {
                if (chunkCount < 6) // spawn chunks in order
                    chunkIndex++;
                else // then randomly select them
                    chunkIndex = Random.Range(1, prefabs.Length);

                pos.x = _chunks[_chunks.Count - 1].rightEdge.position.x + gapSize;
                pos.y = _chunks[_chunks.Count - 1].rightEdge.position.y;
                minLimit = pos.y - 110;
            }
            else
            {
                // spawn first pill
                pill.Spawn(_player.transform.position);
            }

            #region Add Chunk and it's children

            PetzakChunk chunk = Instantiate(prefabs[chunkIndex], pos, Quaternion.identity);
            _chunks.Add(chunk);

            PetzakPlatform[] newplatforms = chunk.GetComponentsInChildren<PetzakPlatform>();
            foreach (PetzakPlatform p in newplatforms)
                _platforms.Add(p.GetComponent<PetzakAABB>());

            PetzakSpring[] newsprings = chunk.GetComponentsInChildren<PetzakSpring>();
            foreach (PetzakSpring s in newsprings)
                _springs.Add(s.GetComponent<PetzakAABB>());

            PetzakSpikes[] newspikes = chunk.GetComponentsInChildren<PetzakSpikes>();
            foreach (PetzakSpikes s in newspikes)
                _spikes.Add(s.GetComponent<PetzakAABB>());

            #endregion

            chunkCount++;
        }

        /// <summary>
        /// Removes chunks (and respawns lifepill) if they are off the left side of the screen.
        /// </summary>
        private void RemoveOffScreenChunks()
        {
            float limitX = FindScreenLeftX();

            for (int i = _chunks.Count - 1; i >= 0; i--)
            {
                if (_chunks[i].rightEdge.position.x < limitX)
                {
                    PetzakChunk chunk = _chunks[i];

                    // remove this chunk's platforms from the game
                    PetzakPlatform[] deadPlatforms = chunk.GetComponentsInChildren<PetzakPlatform>();
                    foreach (PetzakPlatform platform in deadPlatforms)
                        _platforms.Remove(platform.GetComponent<PetzakAABB>());

                    // remove this chunk's springs from the game
                    PetzakSpring[] deadSprings = chunk.GetComponentsInChildren<PetzakSpring>();
                    foreach (PetzakSpring spring in deadSprings)
                        _springs.Remove(spring.GetComponent<PetzakAABB>());

                    // remove this chunk's spikes from the game
                    PetzakSpikes[] deadSpikes = chunk.GetComponentsInChildren<PetzakSpikes>();
                    foreach (PetzakSpikes spike in deadSpikes)
                        _spikes.Remove(spike.GetComponent<PetzakAABB>());

                    // remove the chunk from the game
                    _chunks.RemoveAt(i);
                    Destroy(chunk.gameObject);

                    // update score on hud
                    var mesh = scoreText.GetComponent<TextMesh>();
                    mesh.text = score++.ToString();
                }
            }

            // respawn pill if beyond limit
            bool pillIsBehindPlayer = lifePill.transform.position.x > lifePill.transform.position.x;
            if (lifePill.transform.position.x < limitX && pillIsBehindPlayer)
            {
                Debug.Log(lifePill.transform.position.x + " " + limitX);
                pill.Spawn(_player.transform.position);
            }
        }

        /// <summary>
        /// Stop pill from falling when it collides with an AABB
        /// </summary>
        /// <param name="other"></param>
        public void LifePillCollision(PetzakAABB other)
        {
            if (lifePill.CollidesWith(other))
            {
                pill.isFalling = false;
                Vector3 fix = lifePill.FindFix(other);
                lifePill.BroadcastMessage("ApplyFix", fix);
            }
        }

        /// <summary>
        /// Runs the collision detection for all objects. 
        /// Updates various properties of the hud and pill.
        /// Handles player falling (to their death).
        /// </summary>
        void LateUpdate()
        {
            if (PlayerFell())
                return;

            PlatformCollision();
            SpringCollision();
            SpikeCollision();
            UpdatePill();
            UpdateHud();
        }

        /// <summary>
        /// Determines when player falls, then restarts game or launches player upward (consuming the pill).
        /// </summary>
        /// <returns></returns>
        bool PlayerFell()
        {
            // check if above limit
            if (_player.transform.position.y >= minLimit)
                return false;

            if (hasExtraLife)
            {
                mover.LaunchUpwards(50);
                SwitchPlayerColor(false);
            }
            else
            {
                // restart scene
                UnityEngine.SceneManagement.SceneManager.LoadScene("PetzakScene");
            }
            return true;
        }

        /// <summary>
        /// Checks player (and pill) AABB against every platform AABB.
        /// </summary>
        void PlatformCollision()
        {
            foreach (PetzakAABB platform in _platforms)
            {
                if (_player.CollidesWith(platform))
                {
                    Vector3 fix = _player.FindFix(platform);
                    _player.BroadcastMessage("ApplyFix", fix);
                }
                LifePillCollision(platform);
            }
        }

        /// <summary>
        /// Check player (and pill) AABB against every spring AABB
        /// </summary>
        void SpringCollision()
        {          
            foreach (PetzakAABB spring in _springs)
            {
                if (_player.CollidesWith(spring))
                {
                    PetzakSpring s = spring.GetComponent<PetzakSpring>();
                    if (s != null)
                        mover.LaunchUpwards(s.springiness);
                    break;
                }
                LifePillCollision(spring);
            }
        }

        /// <summary>
        /// Check player (and pill) AABB against every spring AABB.
        /// Restarts game if player touches spikes without the pill.
        /// </summary>
        void SpikeCollision()
        {
            bool spikeCollision = false; // has there been a spike collision this frame
            foreach (PetzakAABB spike in _spikes)
            {
                if (_player.CollidesWith(spike))
                {
                    PetzakSpikes s = spike.GetComponent<PetzakSpikes>();

                    if (mover == null || s == null)
                        continue;

                    spikeCollision = true;
                    if (hasExtraLife) // reset player color after taking damage
                    {
                        touchingSpikes = true;
                        SwitchPlayerColor(false);
                    }
                    else if (!touchingSpikes) // let player leave spikes before taking damage again
                    {
                        UnityEngine.SceneManagement.SceneManager.LoadScene("PetzakScene");
                    }
                }
                LifePillCollision(spike);
            }

            // reset once player takes damage and leaves the spikes range
            if (!spikeCollision)
                touchingSpikes = false;
        }

        /// <summary>
        /// Switches player color to red (default) or green (if the player has the lifepill)
        /// </summary>
        /// <param name="green"></param>
        void SwitchPlayerColor(bool green = true)
        {
            hasExtraLife = green;
            var mrp = _player.GetComponent<MeshRenderer>();
            var mrl = pill.GetComponent<MeshRenderer>();
            mrp.material = green ? mrl.material : playerColor;
        }

        /// <summary>
        /// Spawns a new pill when and detects player collision with pill
        /// </summary>
        void UpdatePill()
        {
            if (pill.spawnTime <= 0 || pill.velocity.y < -30) // check if pill is endlessly falling
            {
                // time to spawn
                pill.Spawn(_player.transform.position);
            }
            else if (_player.CollidesWith(lifePill))
            {
                // give lifepill to player
                SwitchPlayerColor();
                pill.Pickup();
            }
        }

        /// <summary>
        /// Updates various properties of the double jump bar and score text 
        /// </summary>
        void UpdateHud()
        {
            if (!mover.canDoubleJump) // don't update bar if double jump is ready
            {
                Vector3 size = bar.transform.localScale;
                size.x -= .05f;
                if (size.x > 0) 
                {
                    // shrink bar
                    bar.transform.localScale = size;
                }                 
                else if (!mover.resetBar)
                {
                    // hide bar once it's below 0
                    var mr = bar.GetComponent<MeshRenderer>();
                    mr.enabled = false;
                    mover.canDoubleJump = true;
                }
                else
                {
                    // reset bar after double jumping
                    size.x = 8f;
                    bar.transform.localScale = size;
                    var mr = bar.GetComponent<MeshRenderer>();
                    mr.enabled = true;
                    mover.resetBar = false;
                }
            }
            Vector3 barPos = new Vector3(_cam.transform.position.x, _cam.transform.position.y - 7, -2);
            // Position bar below the player
            bar.transform.position = barPos;
            // Position score below and left of the player
            scoreText.transform.position = new Vector3(_cam.transform.position.x - 11, _cam.transform.position.y - 6, -2);
        }
    }
}