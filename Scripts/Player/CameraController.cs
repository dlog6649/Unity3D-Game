using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    float turnSpeed = 80.0f;
    //private float y = 0f;
    //Transform cameraTransform;

    //float viewRange = 90;

    void Start () {
        //cameraTransform = Camera.main.transform;
        
    }
	
	// Update is called once per frame
	void Update () {
        //cameraTransform.localEulerAngles = new Vector3(Mathf.Clamp(cameraTransform.localEulerAngles.x, 0, 180), 0, 0);
        //if(transform.rotation.x > -100.0f && transform.rotation.x < 100.0f)
        transform.Rotate(-Input.GetAxis("Mouse Y") * turnSpeed * Time.deltaTime, 0, 0);

        //y = -Input.GetAxis("Mouse Y") * turnSpeed * Time.deltaTime;
        //print("1:"+y);
        //y = Mathf.Clamp(y, -90f, 90f);
        //print("2:"+y);
        //transform.Rotate(y, 0, 0);
        //transform.rotation *= Quaternion.Euler(y, 0, 0);

        //transform.localEulerAngles = new Vector3(y, 0, 0);
        /*
        if (cameraTransform.localEulerAngles.x > viewRange)
        {
            cameraTransform.localEulerAngles = new Vector3(viewRange, 0, 0);
        }
        if (cameraTransform.localEulerAngles.x < -viewRange)
        {
            cameraTransform.localEulerAngles = new Vector3(-viewRange, 0, 0);
        }*/
        /*
        var upDown = Input.GetAxis("Vertical") * Time.deltaTime * turnSpeed;

        //making it turn up or down
        if (transform.localEulerAngles.x < 25)
        {
            transform.Rotate(upDown, 0, 0);
        }
        else if (transform.localEulerAngles.x > 295)
        {
            transform.Rotate(upDown, 0, 0);
        }
        
        //making it not exeed rotation limit (up and down) and preventing it lock up
        if (transform.localEulerAngles.x >= 25 && transform.localEulerAngles.x < 295)
        {
            if (transform.localEulerAngles.x < 180)
            {
                transform.localEulerAngles.x = 24.9;
            }
            else
            {
                transform.localEulerAngles.x = 295.1;
            }
        }*/

    }
}
