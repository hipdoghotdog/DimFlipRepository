using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.InputSystem;

public class LevelFlipper : MonoBehaviour
{

    public GameObject[,,] level;
    
    public void SetLevel(GameObject[,,] l) {
        level = l;
    }

    public void flipLevel(Vector3 pp, GameManager.View view)
    {
        int ppx = (int)pp.x;
        int ppy = (int)pp.y;
        int ppz = (int)pp.z;

        for (int i = 0; i < level.GetLength(0); i++)
        {
            for (int k = 0; k < level.GetLength(1); k++)
            {
                for (int j = 0; j < level.GetLength(2); j++)
                {
                    if(level[i,k,j] == null){continue;}
                    
                    //level[i, k, j].gameObject.GetComponent<Block>().activate(false);
                    if (view == GameManager.View.SideView)
                    {
                        bool onHeightPlane = (int)level[i, k, j].transform.position.y == ppy;
                        level[i, k, j].gameObject.GetComponent<Block>().activate(onHeightPlane);
                    }
                    else
                    {
                        bool onSidePlane = (int)level[i, k, j].transform.position.z == ppz;
                        level[i, k, j].gameObject.GetComponent<Block>().activate(onSidePlane);
                    }
                }
            }
        }
    }

    private void Start()
    {
        

    }



}
