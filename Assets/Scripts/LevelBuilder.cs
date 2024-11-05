using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    public GameObject emptyTemplate;
    public GameObject blockTemplate;
    public GameObject startTemplate;
    public GameObject endTemplate;
    public GameObject ladderTemplateRight;
    public GameObject ladderTemplateLeft;
    public GameObject levelParent;
    public GameObject leverTemplate;
    public int[,,] levelTemplate;
    public int currentLevel = 0;
    public Dictionary<int, GameObject> blockTemplates;
    public Dictionary<int, int> interActPairs;

    void BuildDictionary()
    {
        blockTemplates = new Dictionary<int, GameObject>(){
            {0, emptyTemplate},
            {1, blockTemplate},
            {2, startTemplate},
            {3, endTemplate},
            {4, ladderTemplateRight},
            {5, ladderTemplateLeft},
            {6, leverTemplate }
        };
    }

    public HashSet<string> GetUnsteppableBlocks() {
        return new HashSet<string>() {
            emptyTemplate.GetComponent<Block>().GetBlockType(),
            ladderTemplateRight.GetComponent<Block>().GetBlockType(), 
            ladderTemplateLeft.GetComponent<Block>().GetBlockType()
        };
    }

    // Add more if new blocks are added that the player should not be inside
    public HashSet<string> GetSteppableBlocks() {
        return new HashSet<string>() {
            blockTemplate.GetComponent<Block>().GetBlockType(),
            startTemplate.GetComponent<Block>().GetBlockType(),
            endTemplate.GetComponent<Block>().GetBlockType()
        };
    }

    public List<GameObject> GetLadders() {
        return new List<GameObject>() {
            ladderTemplateRight, ladderTemplateLeft
        };
    }
    
    // Start is called before the first frame update
    public GameObject[,,] RemoteBuild()
    {
        BuildDictionary();
        levelTemplate = null;   
        switch(currentLevel)
        {
            case 0:
                level0TemplateSetup();
                break;
            case 1: 
                level1TemplateSetup();
                break;
            case 2:
                level2TemplateSetup();
                break;
            case 3:
                level3TemplateSetup();
                break;
            case 4:
                level4TemplateSetup();
                break;
        }
        
        return buildLevel(levelParent.transform, levelTemplate);
    }

    

    void level0TemplateSetup(){
        levelTemplate = new int[,,] {
            {{2},{0}},
            {{1},{0}},
            {{1},{0}},
            {{1},{1}},
            {{1},{0}},
            {{3},{0}},
        };
        //interActPairs = new Dictionary<int, int>();
        //interActPairs.Add(100,2102); //Last digit refers block direction
                                    //1=UP-DOWN, 2=DOWN-UP, 3=LEFT-RIGHT, 4=RIGHT-LEFT

    }

    void level1TemplateSetup()
    {
        levelTemplate = new int[,,] { 
                                    { { 2, 0, 3 }, { 0, 0, 0 } },
                                    { { 1, 0, 1 }, { 0, 0, 4 } },
                                    { { 1, 1, 0 }, { 0, 4, 1 } },
                                    { { 1, 0, 0 }, { 0, 1, 1 } } 
                                    };
    }

    

    void level2TemplateSetup() 
    {
        levelTemplate = new int[,,] { 
                                    {{2,1,0,0}, {0,0,0,1}, {0,0,3,0}, {0,0,4,0}},
                                    {{0,1,0,0}, {0,0,0,1}, {0,0,0,4}, {0,1,1,0}},
                                    {{1,1,0,1}, {0,4,0,0}, {1,0,0,1}, {0,1,0,0}},
                                    {{1,0,1,1}, {0,1,0,4}, {1,0,1,0}, {0,1,4,0}},
                                    {{0,1,1,0}, {1,5,0,1}, {5,0,0,4}, {0,1,1,0}},
                                    {{1,0,1,1}, {5,0,0,0}, {0,0,1,1}, {0,0,5,0}}
        };
    }

    void level3TemplateSetup()
    {
        levelTemplate = new int[,,] {
            {{2},{0}},
            {{1},{0}},
            {{6},{0}},
            {{0},{1}},
            {{1},{0}},
            {{3},{0}},
        };
        interActPairs = new Dictionary<int, int>();
        interActPairs.Add(200,3102); //Last digit refers block direction
        //1=UP-DOWN, 2=DOWN-UP, 3=LEFT-RIGHT, 4=RIGHT-LEFT
    }

    void level4TemplateSetup()
    {
        levelTemplate = new int[,,] {
                                    { { 2, 0, 0, 0 }, { 0, 0, 6, 1 } },
                                    { { 1, 0, 1, 0 }, { 0, 1, 5, 1 } },
                                    { { 1, 1, 0, 0 }, { 0, 0, 0, 1 } },
                                    { { 1, 0, 0, 0 }, { 0, 6, 0, 1 } },
                                    { { 6, 0, 1, 0 }, { 0, 1, 4, 1 } },
                                    { { 0, 0, 0, 0 }, { 1, 1, 1, 1 } },
                                    { { 3, 0, 0, 0 }, { 0, 0, 0, 0 } }
                                    };
        interActPairs = new Dictionary<int, int>();
        interActPairs.Add(400, 4112);
        interActPairs.Add(012, 2014);
        interActPairs.Add(311, 5102);
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

                    // insert block
                    GameObject block = Instantiate(blockTemplates[lt[i,k,j]], parent);
                    block.transform.position = new Vector3(i, k, j);
                    level[i, k, j] = block;
                }
            }
        }
        return level;
    }

}
