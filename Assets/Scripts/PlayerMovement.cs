using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    Rigidbody rb;
    public GameObject @Cam;
    float perspective;
    float xInput;
    float yInput;
    float zInput;

    private void Awake() { 
        rb = GetComponent<Rigidbody>();
        perspective = Cam.GetComponent<CameraScript>().perspective;
    }



    void FixedUpdate() {
        perspective = Cam.GetComponent<CameraScript>().perspective;
        //SideCam
        if ( perspective == 1.0f ) 
        {
            xInput = Input.GetAxis("Horizontal");
            yInput = Input.GetAxis("Jump");
        }
        else //Topcam
        {
            xInput = Input.GetAxis("Horizontal");
            zInput = Input.GetAxis("Vertical");

        }
        

        rb.AddForce(xInput * speed, yInput * speed, zInput * speed);
    }
}
