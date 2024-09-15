using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    Rigidbody rb;

    float xInput;
    float yInput;

    private void Awake() { 
        rb = GetComponent<Rigidbody>();
    }



    void FixedUpdate() { 
        xInput = Input.GetAxis("Horizontal");
        yInput = Input.GetAxis("Vertical");

        rb.AddForce(xInput * speed, 0, yInput * speed);
    }
}
