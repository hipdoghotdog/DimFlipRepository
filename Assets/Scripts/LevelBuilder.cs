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
    public int[,,] levelTemplate;

    public Dictionary<int, GameObject> blockTemplates;

    void BuildDictionary()
    {
        blockTemplates = new Dictionary<int, GameObject>(){
            {0, emptyTemplate},
            {1, blockTemplate},
            {2, startTemplate},
            {3, endTemplate},
            {4, ladderTemplateRight},
            {5, ladderTemplateLeft}
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
        level0TemplateSetup();
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
    }

    void levelTemplateSetup()
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
        levelTemplate = new int[,,] { // Get ladders into this!!
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
