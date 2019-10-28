using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuBuilder : MonoBehaviour {

    public bool isInFocus = true;
    public EventSystem eventSystem;


    public SimpleButton prefabButton;
    public RectTransform prefabColumn;

    private SimpleButton buttonToFocusOn;
    private List<RectTransform> columns = new List<RectTransform>();
    private List<SimpleButton> bttns = new List<SimpleButton>();

    public int numberOfColumns = 2;
    public int spacingHeight = 50;
    public int gutterWidth = 5;

    void Start() {
        if (eventSystem == null) eventSystem = GetComponentInChildren<EventSystem>();
        BuildMenu(Game.zones);
    }
    void BuildMenu(List<ZoneInfo> zones) {

        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }

        buttonToFocusOn = null;

        MakeColumns();
        MakeButton("Back", () => { });
        NoMoreButtonsThisRow();

        foreach (ZoneInfo zone in zones) {
            MakeButton(zone.zoneName, ()=> { print($"load: {zone.level}"); });
        }
    }
    void NoMoreButtonsThisRow() {
        for (int i = bttns.Count % numberOfColumns; i < numberOfColumns; i++) {
            bttns.Add(null);
        }
    }
    void MakeColumns() {

        for (int i = 0; i < numberOfColumns; i++) {
            RectTransform col = Instantiate(prefabColumn, transform);
            float x1 = i / (float)numberOfColumns;
            float x2 = (i + 1) / (float)numberOfColumns;
            col.anchorMin = new Vector2(x1, 0);
            col.anchorMax = new Vector2(x2, 1);
            col.offsetMin = new Vector2(0, 0);
            col.offsetMax = new Vector2(0, 0);
            columns.Add(col);
        }
    }
    void MakeButton(string caption, SimpleButton.ClickCallback callback) {

        int col = bttns.Count % numberOfColumns;
        int row = bttns.Count / numberOfColumns;
        int y = - row * spacingHeight;

        RectTransform column = columns[col];
        SimpleButton bttn = Instantiate(prefabButton, columns[col]);
        bttn.Init(caption, callback);
        bttns.Add(bttn);

        if (buttonToFocusOn == null) buttonToFocusOn = bttn;

        RectTransform bttnTrans = bttn.transform as RectTransform;
        bttnTrans.anchorMin = new Vector2(0, 1);
        bttnTrans.anchorMax = new Vector2(1, 1);

        int marginLeft = (col == 0) ? 0 : gutterWidth;
        int marginRight = (col == numberOfColumns - 1) ? 0 : -gutterWidth;

        bttnTrans.anchoredPosition = new Vector2(0, y);
        bttnTrans.offsetMin = new Vector2(marginLeft, bttnTrans.offsetMin.y);
        bttnTrans.offsetMax = new Vector2(marginRight, bttnTrans.offsetMax.y);
    }

    void Update() {
        if(isInFocus) Focus();
    }
    void Focus() {
        if (eventSystem == null) return;
        if (eventSystem.currentSelectedGameObject == null) {
            if (buttonToFocusOn != null) {
                eventSystem.SetSelectedGameObject(buttonToFocusOn.gameObject);
            }
        }
    }
}
