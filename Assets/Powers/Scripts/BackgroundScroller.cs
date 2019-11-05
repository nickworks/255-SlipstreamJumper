using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    public float speed;
    public float repeaterPoint;
    public Vector3 resetPoint;

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3 (transform.position.x + (speed * Time.deltaTime), transform.position.y, transform.position.z);
        if(speed > 0 && transform.position.x > repeaterPoint || speed < 0 && transform.position.x < repeaterPoint) transform.position = resetPoint;
    }
}
