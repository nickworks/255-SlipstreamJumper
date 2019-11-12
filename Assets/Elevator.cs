using UnityEngine;

namespace Powers
{
    [RequireComponent(typeof (VerticalScroller))]
    public class Elevator : MonoBehaviour
    {
        private VerticalScroller scroller;

        void Start()
        {
            scroller = gameObject.GetComponent<VerticalScroller>();
            
            //this randomizes the scroller variables to make to ensure multiple prefabs on screen behave differently
            scroller.speed = Random.Range(1, 3);
            scroller.maxHeight = Random.Range(0, -1);
            scroller.minHeight = Random.Range(-3, -4);
        }
    }
}

