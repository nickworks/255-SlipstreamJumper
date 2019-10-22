using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Pattison {
    public class MainHUD : MonoBehaviour {

        public Text buttonText;
        public Text levelText;
        public Text nameText;
        public Image imageOfTimer;
        public RectTransform background;

        public float backgroundTransitionTime;
        public AnimationCurve backgroundTransitionCurve;
        float timerBackground;

        void Start() {

        }
        public void UpdateTimer(float p) {
            imageOfTimer.fillAmount = p;
        }
        void Update() {
            buttonText.text = Game.main.isPaused ? "Play" : "Pause";
            AnimateBackground();
        }
        public void SetLevelDetails(ZoneInfo info) {
            nameText.text = info.creator;
            levelText.text = info.zoneName;
        }
        private void AnimateBackground() {
            timerBackground += Game.main.isPaused ? Time.unscaledDeltaTime : -Time.unscaledDeltaTime;
            timerBackground = Mathf.Clamp(timerBackground, 0, backgroundTransitionTime);
            float p = timerBackground / backgroundTransitionTime;
            p = 1 - backgroundTransitionCurve.Evaluate(p);
            background.anchorMin = new Vector2(0, p);
        }

        public void TogglePause() {
            Game.main.TogglePause();
        }
    }
}