using System;
using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameManager;

public class GameManager : MonoBehaviour
{
    public LevelBuilder lb;
    public LevelFlipper lf;
    public GameObject player;
    public GameObject cam;
    private GameObject[,,] level1;
    private bool init = false;
    public GameObject endScene;
    
    public GameObject menu;
    private bool menuVisible = false;

    public Animator camAnimator;
    
    public enum View
    {
        SideView,
        TopdownView
    }

    public View currentView;

    void Flip(Vector3 pp, bool camFlip = true) {
        
        lf.flipLevel(pp, currentView);
            currentView = currentView == View.SideView ? View.TopdownView : View.SideView;
            if (currentView == View.TopdownView && camFlip)
            {
                camAnimator.SetTrigger("FlipToTopView");
            }
            else if (currentView == View.SideView && camFlip)
            {
                camAnimator.SetTrigger("FlipToSideView");
            }
    }



    bool CanIStepOnBlock(Vector3 p ){
            string b = GetBlock(p).GetBlockType();
            return !(lb.GetUnsteppableBlocks().Contains(b));   
    }

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
        return level1[(int)p.x, (int)p.y, (int)p.z].GetComponent<Block>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Hide the menu
        menu.SetActive(false);

        lf.SetLevel(lb.RemoteBuild());
        level1 = lf.level;
        player.transform.position = lb.levelParent.transform.position;
        
        Vector3 spp = player.transform.position;
        cam.transform.position = new Vector3(level1.GetLength(0) / 2, spp.y + 5, spp.z - 10);
        cam.transform.LookAt(new Vector3(level1.GetLength(0) / 2, spp.y, spp.z));
        endScene.SetActive(false);
    }

    // Used to select levels
    public void SelectLevel(int level) {
        Vector3 pp = player.transform.position;
        lb.currentLevel = level;

        /*foreach(var b in level1)
        {
            Destroy(b);
        }*/
        for (int i = 0; i < level1.GetLength(0); i++)
        {
            for (int k = 0; k < level1.GetLength(1); k++)
            {
                for (int j = 0; j < level1.GetLength(2); j++)
                {
                    if (level1[i, k, j] == null) { continue; }

                    Destroy(level1[i, k, j]);
                }
            }
        }
        level1 = null;
        lf.SetLevel(lb.RemoteBuild());
        level1 = lf.level;
        pp = lb.levelParent.transform.position;
        
        Vector3 spp = player.transform.position;
        cam.transform.position = new Vector3(level1.GetLength(0) / 2, spp.y + 5, spp.z - 10);
        cam.transform.LookAt(new Vector3(level1.GetLength(0) / 2, spp.y, spp.z));

        init = false;   
    }
    
    // Update is called once per frame
    void Update()
    {
        Vector3 pp = player.transform.position;
        if (!init)
        {
            player.transform.position.Set(0,0,0);
            Flip(pp, false);
            Flip(pp, false);
            if (currentView == View.TopdownView)
            {
                Flip(pp);
            }
            init = true;
        }
        //When reaching end goal
        if (GetBlock(pp).GetBlockType() == "end")
        {
            pp = lb.levelParent.transform.position;
            if (lb.currentLevel == 4)
            {
                endScene.SetActive(true);
            }
            else
            {
                SelectLevel(lb.currentLevel + 1);
            }            
         
        }

        // Open/Close the menu
        if(Input.GetKeyDown(KeyCode.Escape)) {
            menu.SetActive(!menuVisible);
            menuVisible = !menuVisible;
        }


        if(Input.GetKeyDown(KeyCode.E) && GetBlock(pp).GetBlockType() == "lever")
        {
            
            bool state = !GetBlock(pp).switchOn;
            GetBlock(pp).pull(state);
            int num = (int)(pp.x * 100 + pp.y * 10 + pp.z);
            Debug.Log("num for getting lever is: " + num);
            int BlockCC = lb.interActPairs[num];
            int[] c = new int[4];
            for(int n = 3; n>=0; n--)
            {
                c[n] = BlockCC % 10;
                BlockCC /= 10;
            }
            GameObject block = level1[c[0], c[1], c[2]];
            Vector3 blockPos = block.transform.position;
            int i = state ? -1 : 1;
            switch (c[3])
            {
                case 1:
                    i *= -1;
                    blockPos.y += i;
                    block.transform.position = (blockPos);
                    level1[c[0], c[1], c[2]] = lb.blockTemplates[0];
                    level1[c[0], c[1] + i, c[2]] = block;
                    break;
                case 2:
                    blockPos.y += i;
                    block.transform.position = (blockPos);
                    level1[c[0], c[1], c[2]] = lb.blockTemplates[0];
                    level1[c[0], c[1] + i, c[2]] = block;
                    break;
                case 3:
                    i *= -1;
                    blockPos.x += i;
                    block.transform.position = (blockPos);
                    level1[c[0], c[1], c[2]] = lb.blockTemplates[0];
                    level1[c[0]+i, c[1], c[2]] = block;
                    break;

                case 4:
                    blockPos.x += i;
                    block.transform.position = (blockPos);
                    level1[c[0], c[1], c[2]] = lb.blockTemplates[0];
                    level1[c[0]+i, c[1], c[2]] = block;

                    break;
            }
            Flip(pp, false);
            Flip(pp, false);
            

            lb.interActPairs[num] = c[0] * 1000 + (c[1] + i) * 100 + c[2] *10 + c[3];
            
            //MoveBlock(block);

        }

        if(Input.GetKeyDown(KeyCode.Backspace)) {
            pp = lb.levelParent.transform.position;
            if(currentView == View.TopdownView) {
                Flip(pp);
            }
            else{
                Flip(pp, false);
                Flip(pp, false);
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
                else if(CanIStepOnBlock(new Vector3(pp.x+1,pp.y,pp.z)) && pp.y == level1.GetLength(1)-1 
                        || CanIStepOnBlock(new Vector3(pp.x + 1, pp.y, pp.z)) && !CanIStepOnBlock(new Vector3(pp.x + 1, pp.y + 1, pp.z))) 
                {
                    // Move right
                    pp.x += 1;
                }
                
                
            }

            player.transform.rotation = Quaternion.Euler(0, 0, 0);
            
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
                else if (CanIStepOnBlock(new Vector3(pp.x-1, pp.y, pp.z)) && pp.y == level1.GetLength(1) - 1 ||
                        CanIStepOnBlock(new Vector3(pp.x - 1, pp.y, pp.z)) && !CanIStepOnBlock(new Vector3(pp.x-1,pp.y+1,pp.z)))
                {
                    // Move left
                    pp.x -= 1;
                }
            }
            player.transform.rotation = Quaternion.Euler(0, 180, 0);

        }

        if(currentView == View.TopdownView){
            // Move up in top down
            if(Input.GetKeyDown(KeyCode.UpArrow) && pp.z != level1.GetLength(2)-1 && CanIStepOnBlock(new Vector3(pp.x, pp.y, pp.z+1))){
                pp.z += 1;
                player.transform.rotation = Quaternion.Euler(0, -90, 0);
            }

            // Move down in top down
            if(Input.GetKeyDown(KeyCode.DownArrow) && pp.z != 0 && CanIStepOnBlock(new Vector3(pp.x, pp.y, pp.z-1))){
                pp.z -= 1;
                player.transform.rotation = Quaternion.Euler(0, 90, 0);
            }
        }
        //pp.x = Mathf.Clamp(pp.x, 0, (float)level1.GetLength(0)-1);
        //pp.y = Mathf.Clamp(pp.y, 0, level1.GetLength(1));
        //pp.z = Mathf.Clamp(pp.z, 0, level1.GetLength(2)-1);

        player.transform.position = pp;

    }
}
