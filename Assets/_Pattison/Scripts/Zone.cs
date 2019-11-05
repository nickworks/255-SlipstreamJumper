using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ZoneInfo {
    public string zoneName;
    public string creator;
    public string level;
}
namespace Pattison {
    public class Zone : MonoBehaviour {
        static public ZoneInfo info = new ZoneInfo() {
            zoneName = "",
            creator = "",
            level = ""
        };

        public AABB player;
        /// <summary>
        /// The current chunks in our scene.
        /// </summary>
        List<Chunk> chunks = new List<Chunk>();
        /// <summary>
        /// The AABBs of all platforms in our scene.
        /// </summary>
        List<AABB> platforms = new List<AABB>();
        /// <summary>
        /// The AABBs of all springs in our scene.
        /// </summary>
        List<AABB> springs = new List<AABB>();

        /// <summary>
        /// The chunks we are allowed to spawn.
        /// </summary>
        public Chunk[] prefabChunks;

        public float gapSizeMin = 2;
        public float gapSizeMax = 10;

        Camera cam;


        void Awake() {
            cam = GetComponent<Camera>();
        }

        void Start() {
        }

        void Update() {


            if (chunks.Count < 2) {
                SpawnChunk();
            }

            RemoveOffscreenChunks();

        }

        private void RemoveOffscreenChunks() {

            float limitX = FindScreenLeftX();

            for (int i = chunks.Count - 1; i >= 0; i--) {

                if (chunks[i].rightEdge.position.x < limitX) {

                    Chunk chunk = chunks[i];

                    // remove this chunk's platforms from the game:
                    Platform[] deadPlatforms = chunk.GetComponentsInChildren<Platform>();
                    foreach(Platform platform in deadPlatforms) {
                        platforms.Remove(platform.GetComponent<AABB>());
                    }

                    // remove this chunk's springs from the game:
                    Spring[] deadSprings = chunk.GetComponentsInChildren<Spring>();
                    foreach(Spring spring in deadSprings) {
                        springs.Remove(spring.GetComponent<AABB>());
                    }

                    // remove the chunk from the game:
                    chunks.RemoveAt(i);
                    Destroy(chunk.gameObject);
                }
            }
        }

        private float FindScreenLeftX() {
            Plane xy = new Plane(Vector3.forward, Vector3.zero);
            Ray ray = cam.ScreenPointToRay(new Vector3(0, Screen.height / 2));
            if (xy.Raycast(ray, out float dis)) {
                Vector3 pt = ray.GetPoint(dis);
                return pt.x;
                //Debug.DrawRay(ray.origin, ray.direction * dis, Color.yellow);
            }
            return -10;
        }

        private void SpawnChunk() {
            // spawn new chunk:

            float gapSize = Random.Range(gapSizeMin, gapSizeMax);

            Vector3 pos = new Vector3(-5, -3, 0);

            if (chunks.Count > 0) {
                pos.x = chunks[chunks.Count - 1].rightEdge.position.x + gapSize;
                pos.y = chunks[chunks.Count - 1].rightEdge.position.y;
            }

            int index = Random.Range(0, prefabChunks.Length);

            Chunk chunk = Instantiate(prefabChunks[index], pos, Quaternion.identity);
            chunks.Add(chunk);


            Platform[] newplatforms = chunk.GetComponentsInChildren<Platform>();
            foreach(Platform p in newplatforms) {
                platforms.Add(p.GetComponent<AABB>());
            }

            Spring[] newsprings = chunk.GetComponentsInChildren<Spring>();
            foreach(Spring s in newsprings) {
                springs.Add(s.GetComponent<AABB>());
            }

        }

        void LateUpdate() {

            // check player AABB against every platform AABB:
            foreach(AABB platform in platforms) {
                if (player.CollidesWith(platform)) {
                    // collision!!!
                    Vector3 fix = player.FindFix(platform);
                    player.BroadcastMessage("ApplyFix", fix);
                }
            }

            // check player AABB against every spring AABB:

            foreach (AABB spring in springs) {
                if (player.CollidesWith(spring)) {
                    // boing!!!
                    PlayerMovement mover = player.GetComponent<PlayerMovement>();
                    Spring s = spring.GetComponent<Spring>();

                    if (mover != null && s != null) {
                        mover.LaunchUpwards(s.springiness);
                    }
                }
            }

        } // end LateUpdate()
    } // end class
} // end namespace