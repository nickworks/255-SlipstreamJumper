using UnityEngine;

namespace Powers
{
    public class HorizontalScroller : MonoBehaviour
    {
        public float speed;
        public float repeaterPoint;
        public Vector3 resetPoint;

        // Update is called once per frame
        void Update()
        {
            //move position horizontally
            transform.position = new Vector3(transform.position.x + (speed * Time.deltaTime), transform.position.y, transform.position.z);
            //if repeat point is reached, reset the position
            if (speed > 0 && transform.position.x > repeaterPoint || speed < 0 && transform.position.x < repeaterPoint) transform.position = resetPoint;
        }
    }
}

