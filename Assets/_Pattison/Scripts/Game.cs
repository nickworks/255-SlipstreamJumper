using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour {

    public static Game main { get; private set; }

    public bool isPaused { get; private set; }
    float prePauseTimescale = 1;

    public float timePerZone = 30;
    float timerUntilWarp;
    List<ZoneInfo> zones = new List<ZoneInfo>();
    List<ZoneInfo> zonesUnplayed = new List<ZoneInfo>();

    Pattison.MainHUD hud;

    void Start() {

        if (main != null) {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        main = this;
        hud = GetComponent<Pattison.MainHUD>();

        zones.Add(Andrea.Zone.info);
        zones.Add(Breu.Zone.info);
        zones.Add(Caughman.Zone.info);
        zones.Add(Jennings.Zone.info);
        zones.Add(Myles.Zone.info);
        zones.Add(Petzak.Zone.info);
        zones.Add(Powers.Zone.info);
        zones.Add(Smith.Zone.info);
        zones.Add(Stralle.Zone.info);
        zones.Add(Takens.Zone.info);
        zones.Add(Wynalda.Zone.info);

    }
    void Update() {
        if (Input.GetButtonDown("Pause")) TogglePause();
        if (isPaused) return;

        timerUntilWarp -= Time.unscaledDeltaTime;
        hud.UpdateTimer(timerUntilWarp / timePerZone);
        if (timerUntilWarp < 0) Warp();
    }
    public void Warp() {
        timerUntilWarp = timePerZone;
        if (zonesUnplayed.Count == 0) zonesUnplayed = new List<ZoneInfo>(zones);
        if (zonesUnplayed.Count == 0) return;
        int index = Random.Range(0, zonesUnplayed.Count);
        Play(zonesUnplayed[index]);
        zonesUnplayed.RemoveAt(index);
    }
    public void Play(ZoneInfo zone) {
        SceneManager.LoadScene(zone.level, LoadSceneMode.Single);
        hud.SetLevelDetails(zone);
    }
    public void TogglePause() {
        isPaused = !isPaused;
        if (isPaused) prePauseTimescale = Time.timeScale;
        Time.timeScale = isPaused ? 0 : prePauseTimescale;
    }
}
