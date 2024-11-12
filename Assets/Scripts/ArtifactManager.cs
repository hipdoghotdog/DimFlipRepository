using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ArtifactManager : MonoBehaviour
{

    public Transform followPos;
    public Vector3 offset;
    public float speed;

    private bool displayActive = false;
    private float displayCountdown = 0;

    // Make some TMPro magic i guess

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
            }
        }
    }

    // Will display text and remove it after a few seconds
    public void DisplayText(string text){

    }

    
}
