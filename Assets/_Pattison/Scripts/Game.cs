using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour {

    public static Game main { get; private set; }

    public bool isPaused { get; private set; }
    float prePauseTimescale = 1;

    public float timePerZone = 30;
    public float timerUntilWarp { get; private set; }
    private static List<ZoneInfo> _zones = new List<ZoneInfo>() {
        Andrea.Zone.info,
        Breu.Zone.info,
        Caughman.Zone.info,
        Jennings.Zone.info,
        Myles.Zone.info,
        Petzak.Zone.info,
        Powers.Zone.info,
        Smith.Zone.info,
        Stralle.Zone.info,
        Takens.Zone.info,
        Wynalda.Zone.info
    };
    public static List<ZoneInfo> zones { get { return _zones; } }
    List<ZoneInfo> zonesUnplayed = new List<ZoneInfo>();
    public ZoneInfo currentZone { get; private set; }

    static public ZoneInfo queuedZone;

    /// <summary>
    /// If the sceneswitcher isn't loaded yet, this method
    /// loads the scene and queues a zone for loading.
    /// </summary>
    /// <param name="zone">The zone to load</param>
    static public void Play(ZoneInfo zone) {
        queuedZone = zone;
        if (main == null) {
            SceneManager.LoadScene("SceneSwitcher");
        }
    }

    void Awake() {
        if (main != null) {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        main = this;
    }
    void Start() {
        
    }
    void Update() {
        if (Input.GetButtonDown("Pause")) TogglePause();
        if (isPaused) return;

        WarpToQueuedZone();

        timerUntilWarp -= Time.unscaledDeltaTime;

        if (timerUntilWarp < 0) WarpRandom();
    }

    private void WarpToQueuedZone() {
        if (queuedZone.level != null) {
            print($"loading queued level:\"{queuedZone.level}\"");
            WarpTo(queuedZone);
            queuedZone = new ZoneInfo(); // clear the queue
        }
    }

    public void WarpRandom() {
        if (zonesUnplayed.Count == 0) zonesUnplayed = new List<ZoneInfo>(zones);
        if (zonesUnplayed.Count == 0) return;
        int index = Random.Range(0, zonesUnplayed.Count);
        WarpTo(zonesUnplayed[index]);
    }
    public void WarpTo(ZoneInfo zone) {
        timerUntilWarp = timePerZone;
        SceneManager.LoadScene(zone.level, LoadSceneMode.Single);
        currentZone = zone;
        RemoveCurrentFromZoneList();
        print($"warped to \"{currentZone.level}\" ({zonesUnplayed.Count} left)");
    }
    private void RemoveCurrentFromZoneList() {
        if (zonesUnplayed.Count == 0) zonesUnplayed = new List<ZoneInfo>(zones);
        if (zonesUnplayed.Count == 0) return;
        int index = zonesUnplayed.IndexOf(currentZone);
        zonesUnplayed.RemoveAt(index);
        
    }
    public void TogglePause() {
        isPaused = !isPaused;
        if (isPaused) prePauseTimescale = Time.timeScale;
        Time.timeScale = isPaused ? 0 : prePauseTimescale;
    }
    public void BackToMainMenu() {
        SceneManager.LoadScene("MainMenu");
        Destroy(gameObject);
    }
}
