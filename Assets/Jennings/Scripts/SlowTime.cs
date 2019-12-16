using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jennings {

    /// <summary>
    /// This script was intended to be a powerup and it is one of two that I attempted to make. While I deleted all
    /// of the SafeMode one mostly because it resulted in compiler errors, I believe this piece works although lacks 
    /// a trigger so it just slightly slows down the entire game.
    /// </summary>
    public class SlowTime : MonoBehaviour {

        public float slowdownFactor = 0.05f;
        public float slowdownLength = 2f;
        // The impact made on time followed by how long the effect will last in seconds.
      

        // Update is called once per frame
        void Update()
        {
            Time.timeScale += (1f / slowdownLength) * Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);
        }

        public void DoSlowmotion()
        {
            Time.timeScale = slowdownFactor;
            Time.fixedDeltaTime = Time.timeScale * .02f;
        }

        

    }
}
