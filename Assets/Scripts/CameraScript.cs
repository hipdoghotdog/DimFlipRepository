using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public float perspective;
    public GameObject @SideCam;
    public GameObject @TopCam;


    // Start is called before the first frame update
    void Start()
    {
        SideCam.SetActive(true);
        TopCam.SetActive(false);
        perspective = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("v"))
        {
            if ( perspective ==  1.0f )
            {
                SideCam.SetActive(false);
                TopCam.SetActive(true);
                perspective = 0f;
            }
            else
            {
                SideCam.SetActive(true);
                TopCam.SetActive(false);
                perspective = 1.0f;
            }
        }
    }
}
