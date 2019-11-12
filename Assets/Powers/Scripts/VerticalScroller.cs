using UnityEngine;

namespace Powers
{
    public class VerticalScroller : MonoBehaviour
    {
        public float speed;
        public float maxHeight;
        public float minHeight;
        public float waitTime;

        private bool directionIndicator = true;

        // Update is called once per frame
        void Update()
        {
            //move position vertically
            if(directionIndicator) transform.position = new Vector3(transform.position.x, transform.position.y + (speed * Time.deltaTime), transform.position.z);
            else if(!directionIndicator) transform.position = new Vector3(transform.position.x, transform.position.y - (speed * Time.deltaTime), transform.position.z);

            //change direction indicator if object gets too high or low
            if (transform.position.y > maxHeight) directionIndicator = false;
            else if (transform.position.y < minHeight) directionIndicator = true;
        }
    }
}

