using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    public GameObject blockTemplate;
    public GameObject startTemplate;
    public GameObject endTemplate;
    public GameObject levelParent;
    public int[,,] levelTemplate;


    // Start is called before the first frame update
    public GameObject[,,] RemoteBuild()
    {
        level2TemplateSetup();
        return buildLevel(levelParent.transform, levelTemplate);
    }


    void levelTemplateSetup()
    {
        levelTemplate = new int[,,] { 
                                    { { 2, 0, 3 }, { 0, 0, 0 } },
                                    { { 1, 0, 1 }, { 0, 0, 0 } },
                                    { { 1, 1, 0 }, { 0, 0, 1 } },
                                    { { 1, 0, 0 }, { 0, 1, 1 } } 
                                    };
    }

    void level2TemplateSetup() 
    {
        levelTemplate = new int[,,] {
                                    {{2,1,0,0}, {0,0,0,1}, {0,0,3,0}, {0,0,0,0}},
                                    {{0,1,0,0}, {0,0,0,1}, {0,0,0,0}, {0,1,1,0}},
                                    {{1,1,0,1}, {0,0,0,0}, {1,0,0,1}, {0,1,0,0}},
                                    {{1,0,1,1}, {0,1,0,0}, {1,0,1,0}, {0,1,0,0}},
                                    {{0,1,1,0}, {1,0,0,1}, {0,0,0,0}, {0,1,1,0}},
                                    {{1,0,1,1}, {0,0,0,0}, {0,0,1,1}, {0,0,0,0}}
        };
    }

    // Parent will act as position 0,0,0 for x,y,z coordinates. 
    // lt[0][0][0] will be exactly at parent position
    public GameObject[,,] buildLevel(Transform parent, int[,,] lt)
    {
        GameObject[,,] level = new GameObject[lt.GetLength(0), lt.GetLength(1), lt.GetLength(2)];
        for(int i = 0; i < lt.GetLength(0); i++)
        {
            for(int k = 0; k < lt.GetLength(1); k++)
            {
                for(int j = 0; j < lt.GetLength(2); j++)
                {
                    if (lt[i,k,j] == 1)
                    {
                        GameObject block = Instantiate(blockTemplate, parent);
                        block.transform.position = new Vector3 (i, k, j);
                        level[i,k,j] = block;
                    }
                    if (lt[i, k, j] == 2)
                    {
                        GameObject block = Instantiate(startTemplate, parent);
                        block.transform.position = new Vector3(i, k, j);
                        level[i, k, j] = block;
                    }
                    if (lt[i, k, j] == 3)
                    {
                        GameObject block = Instantiate(endTemplate, parent);
                        block.transform.position = new Vector3(i, k, j);
                        level[i, k, j] = block;
                    }
                }
            }
        }
        return level;
    }

}
