using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public LevelBuilder lb;
    public LevelFlipper lf;
    public GameObject player;
    public GameObject cam;
    private GameObject[,,] level1;
    private bool init = false;
    public GameObject endScene;
    
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
        level1 = lf.level;
        player.transform.position = lb.levelParent.transform.position;
        
        Vector3 spp = player.transform.position;
        cam.transform.position = new Vector3(level1.GetLength(0) / 2, spp.y + 5, spp.z - 10);
        cam.transform.LookAt(new Vector3(level1.GetLength(0) / 2, spp.y, spp.z));
        endScene.SetActive(false);
    }
    
    // Update is called once per frame
    void Update()
    {
        Vector3 pp = player.transform.position;
        if (!init)
        {
            for (int i = 0; i < 4; i++)
            {
                lf.flipLevel(pp, currentView);
                currentView = currentView == View.SideView ? View.TopdownView : View.SideView;
            }
            init = true;
        }
        if (level1[(int)pp.x,(int)pp.y,(int)pp.z].gameObject.layer == 3)
        {
            endScene.SetActive(true);
        }

        // Flip level
        if(Input.GetKeyDown(KeyCode.Space)) {
            lf.flipLevel(pp, currentView);
            currentView = currentView == View.SideView ? View.TopdownView : View.SideView;
            if (currentView == View.TopdownView)
            {
                cam.transform.position = new Vector3 (level1.GetLength(0) / 2, pp.y + 10, (level1.GetLength(2) / 2)-2);
                cam.transform.LookAt(new Vector3(level1.GetLength(0) / 2, pp.y, level1.GetLength(2) / 2));
            }
            else if (currentView == View.SideView)
            {
                cam.transform.position = new Vector3 (level1.GetLength(0) / 2, pp.y + 5, pp.z - 10);
                cam.transform.LookAt(new Vector3(level1.GetLength(0) / 2, pp.y, pp.z));
            }
        }
        
        


        // Move Player
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (level1[(int)pp.x + 1, (int)pp.y, (int)pp.z] != null)
            {
                pp.x += 1;
            }
            
            else if (pp.y != level1.GetLength(1)-1 && level1[(int)pp.x + 1, (int)pp.y + 1, (int)pp.z] != null && currentView == View.SideView)
            {
                pp.y += 1;
                pp.x += 1;
            }
            else if (pp.y != 0 && level1[(int)pp.x + 1, (int)pp.y - 1, (int)pp.z] != null && currentView == View.SideView)
            {
                pp.x += 1;
                pp.y -= 1;
            }
        }
        
        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (level1[(int)pp.x - 1, (int)pp.y, (int)pp.z] != null)
            {
                pp.x -= 1;
            }

            else if (pp.y != level1.GetLength(1) - 1 && level1[(int)pp.x - 1, (int)pp.y + 1, (int)pp.z] != null && currentView == View.SideView)
            {
                pp.y += 1;
                pp.x -= 1;
            }
            else if (pp.y != 0 && level1[(int)pp.x - 1, (int)pp.y - 1, (int)pp.z] != null && currentView == View.SideView)
            {
                pp.x -= 1;
                pp.y -= 1;
            }

        }

        if(Input.GetKeyDown(KeyCode.UpArrow) && level1[(int)pp.x,(int)pp.y,(int)pp.z+1] != null && currentView == View.TopdownView){
            pp.z += 1;
        }

        if(Input.GetKeyDown(KeyCode.DownArrow) && level1[(int)pp.x, (int)pp.y, (int)pp.z - 1] != null && currentView == View.TopdownView){
            pp.z -= 1;
        }
        pp.x = Mathf.Clamp(pp.x, 0, (float)level1.GetLength(0)-1);
        pp.y = Mathf.Clamp(pp.y, 0, level1.GetLength(1));
        pp.z = Mathf.Clamp(pp.z, 0, level1.GetLength(2)-1);

        player.transform.position = pp;

    }
}
