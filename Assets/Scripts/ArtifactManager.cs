using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArtifactManager : MonoBehaviour
{

    public Transform followPos;
    public Vector3 offset;
    public float speed;

    private bool displayActive = false;
    private float displayCountdown = 0;

    private TextMesh tm;
    // Make some TMPro magic i guess

    void Start() {
        tm = gameObject.GetComponentInChildren<TextMesh>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 preciseFollowPos = followPos.position + offset;
        transform.position = Vector3.MoveTowards(transform.position, preciseFollowPos, speed*Time.deltaTime);   

        if(displayActive) {
            displayCountdown -= Time.deltaTime;
            if(displayCountdown <= 0) {
                displayActive = false;
                displayCountdown = 0;
                tm.text = "";
            }
        }
    }

    // Will display text and remove it after a few seconds
    public void DisplayText(string text){
        displayActive = true;
        displayCountdown = 3f;
        tm.text = text;
    }

    
}
