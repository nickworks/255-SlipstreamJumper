using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SimpleButton : MonoBehaviour {

    public delegate void ClickCallback();

    ClickCallback callback;

    public void Init(string caption, ClickCallback callback) {
        Text textfield = GetComponentInChildren<Text>();
        if(textfield) textfield.text = caption;
        this.callback = callback;
    }
    public void Clicked() {
        if (callback != null) {
            callback();
        }
    }
    private void OnMouseOver() {
        print("?");
    }
    private void OnMouseEnter() {
        print("??");
    }
    private void OnPointerEnter(PointerEventData eventData) {
        Button bttn = GetComponent<Button>();
        print("???");
        if (bttn != null) bttn.Select();
    }
}
