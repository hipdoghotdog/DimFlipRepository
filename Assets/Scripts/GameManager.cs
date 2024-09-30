using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public LevelBuilder lb;
    public LevelFlipper lf;
    public GameObject player;

    public enum View
    {
        SideView,
        TopdownView
    }

    public View currentView;

    // Start is called before the first frame update
    void Start()
    {
        lf.SetLevel(lb.RemoteBuild());
        player.transform.position = lb.levelParent.transform.position;
        //lf.flipLevel(player.transform.position, currentView);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pp = player.transform.position;

        // Flip level
        if(Input.GetKeyDown(KeyCode.Space)) {
            lf.flipLevel(pp, currentView);
            currentView = currentView == View.SideView ? View.TopdownView : View.SideView;
        }

        

        // Move Player
        if(Input.GetKeyDown(KeyCode.RightArrow)){
            pp.x += 1;    
        }
        
        if(Input.GetKeyDown(KeyCode.LeftArrow)){
            pp.x -= 1;
        }

        if(Input.GetKeyDown(KeyCode.UpArrow) && currentView == View.TopdownView){
            pp.z += 1;
        }

        if(Input.GetKeyDown(KeyCode.DownArrow) && currentView == View.TopdownView){
            pp.z -= 1;
        }

        player.transform.position = pp;

    }
}
