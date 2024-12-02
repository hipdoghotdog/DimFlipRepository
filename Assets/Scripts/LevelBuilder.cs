using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    public GameObject emptyTemplate;

    public GameObject levelParent;

    public int[,,] levelTemplate;
    public int currentLevel = 0;

    public Dictionary<int, GameObject> blockTemplates;

    public Dictionary<int, int> interActPairs;

    public Vector3 startBlockPosition;

    public void BuildDictionary()
    {
        ThemeData theme = ThemeManager.Instance.currentTheme;

        blockTemplates = new Dictionary<int, GameObject>()
        {
            {0, theme.emptyModel != null ? theme.emptyModel : emptyTemplate},
            {1, theme.blockModel},
            {2, theme.startModel},
            {3, theme.endModel},
            {4, theme.ladderModel}, //right
            {5, theme.ladderModel}, //left
            {6, theme.leverModel},
            {7, theme.blockwlightModel},
            {8, theme.pushableModel},
        };
    }

    public GameObject[,,] RemoteBuild()
    {
        BuildDictionary();
        levelTemplate = null;
        switch (currentLevel)
        {
            case 0:
                level5TemplateSetup();
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
            case 5:
                level5TemplateSetup();
                break;
            // Add more cases for additional levels as needed
            default:
                Debug.LogWarning("LevelBuilder: No setup defined for currentLevel index.");
                break;
        }

        return buildLevel(levelParent.transform, levelTemplate);
    }

    void level0TemplateSetup()
    {
        levelTemplate = new int[,,] {
            { {2}, {0} },
            { {1}, {0} },
            { {7}, {0} },
            { {1}, {1} },
            { {1}, {0} },
            { {3}, {0} },
        };
        // Uncomment and define interActPairs if needed
        /*
        interActPairs = new Dictionary<int, int>();
        interActPairs.Add(100,2102); // Last digit refers to block direction
                                        // 1=UP-DOWN, 2=DOWN-UP, 3=LEFT-RIGHT, 4=RIGHT-LEFT
        */
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
            { {2,1,0,0}, {0,0,0,1}, {0,0,3,0}, {0,0,4,0} },
            { {0,1,0,0}, {0,0,0,1}, {0,0,0,4}, {0,1,1,0} },
            { {1,1,0,1}, {0,4,0,0}, {1,0,0,1}, {0,1,0,0} },
            { {1,0,1,1}, {0,1,0,4}, {1,0,1,0}, {0,1,4,0} },
            { {0,1,1,0}, {1,5,0,1}, {5,0,0,4}, {0,1,1,0} },
            { {1,0,1,1}, {5,0,0,0}, {0,0,1,1}, {0,0,5,0} }
        };
    }

    void level3TemplateSetup()
    {
        levelTemplate = new int[,,] {
            { {2}, {0} },
            { {1}, {0} },
            { {6}, {0} },
            { {0}, {1} },
            { {1}, {0} },
            { {3}, {0} },
        };
        interActPairs = new Dictionary<int, int>();
        interActPairs.Add(200, 3102); // Last digit refers to block direction
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

    void level5TemplateSetup()
    {
        levelTemplate = new int[,,] {
            { { 2, 0, 0 }, { 0, 0, 0 } },
            { { 1, 0, 0 }, { 0, 0, 0 } },
            { { 1, 1, 0 }, { 4, 0, 0 } },
            { { 1, 1, 0 }, { 0, 1, 1 } },
            { { 1, 1, 0 }, { 8, 0, 1 } },
            { { 1, 1, 0 }, { 0, 0, 1 } },
            { { 0, 0, 0 }, { 3, 1, 1 } },
        };
    }


    void level11TemplateSetup() // DONE
    {
        // Define the layout using a 3D array
        levelTemplate = new int[,,] {
            { { 2, 0, 6, 1 }, { 0, 0, 0, 0 }, { 0, 1, 4, 0 } },
            { { 1, 0, 0, 1 }, { 0, 1, 1, 1 }, { 5, 0, 0, 4 } },
            { { 1, 1, 0, 1 }, { 0, 4, 0, 0 }, { 0, 0, 6, 1 } },
            { { 0, 0, 0, 1 }, { 1, 1, 1, 1 }, { 0, 0, 0, 0 } },
            { { 1, 0, 1, 1 }, { 4, 0, 5, 0 }, { 0, 3, 0, 0 } },
            { { 0, 0, 0, 0 }, { 1, 1, 0, 0 }, { 0, 1, 0, 0 } },
            { { 0, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 5, 0, 0 } },
            { { 0, 0, 0, 0 }, { 1, 1, 1, 1 }, { 0, 0, 0, 0 } },
            { { 0, 0, 0, 0 }, { 1, 1, 1, 1 }, { 0, 0, 0, 0 } },
        };
        interActPairs = new Dictionary<int, int>();
        interActPairs.Add(002, 0222);
        interActPairs.Add(222, 3102);
    }


    void level8TemplateSetup()
    {
        levelTemplate = new int[,,] {
            { { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 1, 1, 1 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 } },
            { { 0, 0, 0, 1, 1, 1 }, { 0, 0, 1, 1, 5, 1 }, { 0, 0, 0, 4, 0, 0 }, { 0, 0, 0, 0, 0, 0 } },
            { { 0, 0, 0, 0, 0, 1 }, { 0, 0, 0, 0, 0, 4 }, { 0, 0, 1, 1, 1, 0 }, { 0, 0, 4, 0, 4, 0 } },
            { { 0, 0, 0, 0, 0, 0 }, { 0, 1, 1, 1, 1, 1 }, { 0, 0, 0, 0, 0, 0 }, { 1, 1, 1, 0, 1, 1 } },
            { { 0, 0, 0, 0, 0, 0 }, { 0, 1, 1, 1, 1, 1 }, { 0, 0, 0, 0, 0, 0 }, { 1, 0, 0, 6, 0, 1 } },
            { { 0, 0, 0, 0, 0, 0 }, { 0, 1, 1, 2, 1, 1 }, { 1, 0, 0, 0, 6, 1 }, { 5, 0, 0, 1, 0, 5 } },
            { { 6, 0, 0, 0, 0, 0 }, { 0, 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 0, 0 }, { 4, 0, 0, 5, 0, 0 } },
            { { 1, 0, 0, 0, 0, 0 }, { 4, 1, 1, 1, 0, 1 }, { 0, 0, 0, 0, 0, 0 }, { 1, 1, 1, 1, 0, 0 } },
            { { 0, 1, 1, 1, 1, 1 }, { 1, 4, 1, 0, 3, 5 }, { 0, 0, 4, 0, 0, 0 }, { 1, 1, 0, 1, 0, 0 } },
            { { 0, 0, 0, 0, 0, 0 }, { 1, 1, 0, 0, 0, 0 }, { 0, 0, 1, 1, 0, 0 }, { 0, 0, 0, 5, 0, 0 } },
        };
        interActPairs = new Dictionary<int, int>();
        interActPairs.Add(600, 9232);
        interActPairs.Add(524, 8031);
        interActPairs.Add(433, 8143);
    }

    void level9TemplateSetup()
    {
        levelTemplate = new int[,,] {
            { { 1, 1, 1, 1, 1}, { 4, 0, 0, 0, 0}, { 0, 0, 1, 1, 1 }, { 0, 0, 0, 0, 0 } },
            { { 0, 0, 1, 0, 1}, { 6, 0, 1, 0, 0}, { 0, 0, 0, 1, 1 }, { 0, 0, 0, 0, 4 } },
            { { 0, 0, 1, 0, 1}, { 0, 0, 1, 1, 0}, { 1, 1, 0, 5, 0 }, { 0, 4, 0, 0, 1 } },
            { { 0, 0, 0, 0, 1}, { 0, 0, 2, 0, 4}, { 1, 0, 0, 0, 0 }, { 0, 1, 0, 0, 6 } },
            { { 0, 0, 1, 0, 0}, { 1, 1, 0, 0, 1}, { 5, 0, 0, 0, 0 }, { 0, 1, 6, 0, 0 } },
            { { 0, 0, 1, 0, 0}, { 1, 1, 0, 1, 1}, { 0, 4, 0, 0, 4 }, { 0, 0, 0, 1, 0 } },
            { { 0, 0, 1, 0, 0}, { 0, 0, 0, 0, 0 }, { 1, 1, 0, 0, 1 }, { 4, 0, 0, 1, 0 } },
            { { 0, 0, 0, 0, 0}, { 0, 0, 3, 0, 0 }, { 0, 0, 0, 1, 1 }, { 1, 1, 1, 5, 0 } },
        };
        interActPairs = new Dictionary<int, int>();
        interActPairs.Add(334, 4021);
        interActPairs.Add(432, 5021);
        interActPairs.Add(110, 6021);
    }


    public GameObject[,,] buildLevel(Transform parent, int[,,] lt)
    {
        if (lt == null)
        {
            Debug.LogError("LevelBuilder: Level template is null.");
            return null;
        }

        GameObject[,,] level = new GameObject[lt.GetLength(0), lt.GetLength(1), lt.GetLength(2)];
        for (int i = 0; i < lt.GetLength(0); i++)
        {
            for (int k = 0; k < lt.GetLength(1); k++)
            {
                for (int j = 0; j < lt.GetLength(2); j++)
                {
                    int blockID = lt[i, k, j];

                    // Check if the blockID exists in the dictionary
                    if (!blockTemplates.ContainsKey(blockID))
                    {
                        Debug.LogWarning($"LevelBuilder: Block ID {blockID} not found in blockTemplates.");
                        continue;
                    }

                    GameObject blockPrefab = blockTemplates[blockID];

                    // Instantiate the block
                    GameObject block = Instantiate(blockPrefab, parent);
                    block.transform.position = new Vector3(i, k, j);
                    level[i, k, j] = block;

                    // Rotate the ladder if necessary
                    if (blockID == 5) // Ladder to the left
                    {
                        block.transform.Rotate(0, 180, 0);
                    }
                    // No rotation needed for blockID == 4 (Ladder to the right)

                    // Set the blockType
                    Block blockComponent = block.GetComponent<Block>();
                    if (blockComponent != null)
                    {
                        blockComponent.blockType = GetBlockTypeFromID(blockID);
                        blockComponent.ApplyTheme(); // Apply the theme after setting blockType
                    }
                    else
                    {
                        Debug.LogWarning($"LevelBuilder: No Block component found on prefab for Block ID {blockID}.");
                    }

                    // Check if the block is the startTemplate
                    if (blockID == 2) // Assuming 2 corresponds to startTemplate
                    {
                        startBlockPosition = new Vector3(i, k, j);
                    }
                }
            }
        }
        return level;
    }

    private string GetBlockTypeFromID(int id)
    {
        switch (id)
        {
            case 0:
                return "empty";
            case 1:
                return "block";
            case 2:
                return "start";
            case 3:
                return "end";
            case 4:
                return "right ladder";
            case 5:
                return "left ladder";
            case 6:
                return "lever";
            case 7:
                return "blockwlight";
            case 8:
                return "pushable"; 
            default:
                return "unknown";
        }
    }

    public HashSet<string> GetUnsteppableBlocks()
    {
        return new HashSet<string>()
        {
            "empty",
            "right ladder",
            "left ladder"
        };
    }
}

