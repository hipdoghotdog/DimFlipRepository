using System;
using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public LevelBuilder lb;
    public LevelFlipper lf;
    public GameObject player;
    public GameObject cam;
    private GameObject[,,] level1;
    private bool init = false;
    public GameObject endScene;

    public Animator camAnimator;
    
    public enum View
    {
        SideView,
        TopdownView
    }

    public View currentView;

    void Flip(Vector3 pp) {
        lf.flipLevel(pp, currentView);
            currentView = currentView == View.SideView ? View.TopdownView : View.SideView;
            if (currentView == View.TopdownView)
            {
                camAnimator.SetTrigger("FlipToTopView");
            }
            else if (currentView == View.SideView)
            {
                camAnimator.SetTrigger("FlipToSideView");
            }
    }

    bool CanIStepOnBlock(Vector3 p) {
        string b = GetBlock(p).GetBlockType();
        //Debug.Log("Checking steppable");
        //string bOnTop = p.y <= level1.GetLength(1)-1 ? GetBlock(new Vector3(0,1,0) + p).GetBlockType() : "none";
        //Debug.Log("Checking top of steppable: " + bOnTop);
        return !(lb.GetUnsteppableBlocks().Contains(b));
    }
    //|| lb.GetSteppableBlocks().Contains(bOnTop)

    // take player position and block your want to go to
    // returns TRUE if there is a valid ladder at the right place
    bool CanIUseLadder(Vector3 playerPos, Vector3 targetPos) {
        // Going right
        if(playerPos.x - targetPos.x == -1) {
            if(playerPos.y - targetPos.y == -1) {
                // Climbing up
                return GetBlock(new Vector3(0,1,0) + playerPos).GetBlockType() == "right ladder";;
            }
            else if(playerPos.y - targetPos.y == 1) {
                // Climbing down
                return GetBlock(new Vector3(1,0,0) + playerPos).GetBlockType() == "left ladder";
            }
        } // Going left
        else if(playerPos.x - targetPos.x == 1) {
            if(playerPos.y - targetPos.y == -1) {
                // Climbing up
                return GetBlock(new Vector3(0,1,0) + playerPos).GetBlockType() == "left ladder";
            }
            else if(playerPos.y - targetPos.y == 1) {
                // Climbing down
                return GetBlock(new Vector3(-1,0,0) + playerPos).GetBlockType() == "right ladder";
            }
        }
        return false;
    }

    Block GetBlock(Vector3 p) {
        // make dummy blocks instead of nulls, this creates error
        return level1[(int)p.x, (int)p.y, (int)p.z].GetComponent<Block>();
    }

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

        if(Input.GetKeyDown(KeyCode.Backspace)) {
            pp = lb.levelParent.transform.position;
            if(currentView == View.TopdownView) {
                Flip(pp);
            }
            else{
                Flip(pp);
                Flip(pp);
            }
        }

        // Flip level
        if(Input.GetKeyDown(KeyCode.Space)) {
            Flip(pp);
        }

        // Move Player Right
        if (Input.GetKeyDown(KeyCode.RightArrow) && pp.x != level1.GetLength(0) -1)
        {

            if(currentView == View.TopdownView) {
                if (CanIStepOnBlock(new Vector3(pp.x+1, pp.y, pp.z)))
                {
                    // Move right
                    //Debug.Log("Moving right, stepping on block x: " + pp.x+1 + " y: " + pp.y + " z: " + pp.z);
                    pp.x += 1;
                }
            }else{
                Vector3 toPos = new Vector3(pp.x+1, pp.y+1, pp.z);
                Vector3 toPos2 = new Vector3(pp.x+1, pp.y-1, pp.z);
                if (pp.y != level1.GetLength(1)-1 && CanIStepOnBlock(toPos) && CanIUseLadder(pp, toPos))
                {
                    // Step up if ladder present
                    pp.y += 1;
                    pp.x += 1;
                }
                else if (pp.y != 0 && CanIStepOnBlock(toPos2) && CanIUseLadder(pp, toPos2))
                {
                    // Step down if ladder present
                    pp.x += 1;
                    pp.y -= 1;
                }
                else if(CanIStepOnBlock(new Vector3(pp.x+1,pp.y,pp.z)) && !CanIStepOnBlock(new Vector3(pp.x+1,pp.y+1,pp.z))) 
                {
                    // Move right
                    pp.x += 1;
                }
                
                
            }
            
            
            
        }

        
        // Move Player Left
        if(Input.GetKeyDown(KeyCode.LeftArrow) && pp.x != 0)
        {
            
            if(currentView == View.TopdownView){
                // Move left
                if (CanIStepOnBlock(new Vector3(pp.x-1, pp.y, pp.z)))
                {
                    pp.x -= 1;
                }
            }else{
                Vector3 toPos = new Vector3(pp.x-1, pp.y+1, pp.z);
                Vector3 toPos2 = new Vector3(pp.x-1, pp.y-1, pp.z);
                if (pp.y != level1.GetLength(1) - 1 && CanIStepOnBlock(toPos) && CanIUseLadder(pp, toPos))
                {
                    // Move up
                    pp.y += 1;
                    pp.x -= 1;
                }
                else if (pp.y != 0 && CanIStepOnBlock(toPos2) && CanIUseLadder(pp, toPos2))
                {
                    // Move down
                    pp.x -= 1;
                    pp.y -= 1;
                }
                else if (CanIStepOnBlock(new Vector3(pp.x-1, pp.y, pp.z)) && !CanIStepOnBlock(new Vector3(pp.x-1,pp.y+1,pp.z)))
                {
                    // Move left
                    pp.x -= 1;
                }
            }
        }

        if(currentView == View.TopdownView){
            // Move up in top down
            if(Input.GetKeyDown(KeyCode.UpArrow) && pp.z != level1.GetLength(2)-1 && CanIStepOnBlock(new Vector3(pp.x, pp.y, pp.z+1))){
                pp.z += 1;
            }

            // Move down in top down
            if(Input.GetKeyDown(KeyCode.DownArrow) && pp.z != 0 && CanIStepOnBlock(new Vector3(pp.x, pp.y, pp.z-1))){
                pp.z -= 1;
            }
        }
        pp.x = Mathf.Clamp(pp.x, 0, (float)level1.GetLength(0)-1);
        pp.y = Mathf.Clamp(pp.y, 0, level1.GetLength(1));
        pp.z = Mathf.Clamp(pp.z, 0, level1.GetLength(2)-1);

        player.transform.position = pp;

    }
}
