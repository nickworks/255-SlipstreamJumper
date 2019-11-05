using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SimpleButton : MonoBehaviour, IPointerEnterHandler {

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
    public void OnPointerEnter(PointerEventData eventData) {
        Button bttn = GetComponent<Button>();
        if (bttn != null) bttn.Select();
    }
}
