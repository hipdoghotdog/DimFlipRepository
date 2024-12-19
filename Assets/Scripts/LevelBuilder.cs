using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    [HideInInspector]
    public GameObject emptyTemplate;
    
    [HideInInspector]
    public GameManager gameManager;
    
    [HideInInspector]
    private Vector3 _startBlockPosition;

    public Dictionary<int, GameObject> BlockTemplates;
    public readonly Dictionary<int, LevelData> Levels = new();

    // Additional fields from tempbranch
    private int currentLevel;
    private int[,,] levelTemplate;
    public Transform levelParent; // Assign in the Inspector or initialize appropriately
    private Dictionary<int, int> interActPairs;

    public void Initialize(int levelIndex)
    {
        gameManager = GameManager.Instance;
        InitializeLevels();
        BuildDictionary();
        BuildLevel(levelIndex);
        
        gameManager.artifactManager.DisplayText("Level: " + levelIndex);
    }
    
    private void InitializeLevels()
    {
        // Level 0
        Levels.Add(0, new LevelData(
            new int[,,] {
                { {2}, {0} },
                { {1}, {0} },
                { {7}, {0} },
                { {1}, {1} },
                { {1}, {0} },
                { {3}, {0} },
            }
        ));

        // Level 1
        Levels.Add(1, new LevelData(
            new int[,,] {
                { { 2, 0, 3 }, { 0, 0, 0 } },
                { { 1, 0, 1 }, { 0, 0, 4 } },
                { { 1, 1, 0 }, { 0, 4, 1 } },
                { { 1, 0, 0 }, { 0, 1, 1 } }
            }
        ));

        // Level 2
        Levels.Add(2, new LevelData(
            new int[,,] {
                { {2,1,0,0}, {0,0,0,1}, {0,0,3,0}, {0,0,4,0} },
                { {0,1,0,0}, {0,0,0,1}, {0,0,0,4}, {0,1,1,0} },
                { {1,1,0,1}, {0,4,0,0}, {1,0,0,1}, {0,1,0,0} },
                { {1,0,1,1}, {0,1,0,4}, {1,0,1,0}, {0,1,4,0} },
                { {0,1,1,0}, {1,5,0,1}, {5,0,0,4}, {0,1,1,0} },
                { {1,0,1,1}, {5,0,0,0}, {0,0,1,1}, {0,0,5,0} }
            }
        ));

        // Level 3
        Levels.Add(3, new LevelData(
            new int[,,] {
                { {2}, {0} },
                { {1}, {0} },
                { {6}, {0} },
                { {0}, {1} },
                { {1}, {0} },
                { {3}, {0} },
            },
            new Dictionary<int, int> {
                { 200, 3102 }
            }
        ));

        // Level 4
        Levels.Add(4, new LevelData(
            new int[,,] {
                { { 2, 0, 0, 0 }, { 0, 0, 6, 1 } },
                { { 1, 0, 1, 0 }, { 0, 1, 5, 1 } },
                { { 1, 1, 0, 0 }, { 0, 0, 0, 1 } },
                { { 1, 0, 0, 0 }, { 0, 6, 0, 1 } },
                { { 6, 0, 1, 0 }, { 0, 1, 4, 1 } },
                { { 0, 0, 0, 0 }, { 1, 1, 1, 1 } },
                { { 3, 0, 0, 0 }, { 0, 0, 0, 0 } }
            },
            new Dictionary<int, int> {
                { 400, 4112 },
                { 012, 2014 },
                { 311, 5102 }
            }
        ));
        
        // Level 5
        Levels.Add(5, new LevelData(
            new int[,,] {
                { { 2, 0, 0 }, { 0, 0, 0 } },
                { { 1, 0, 0 }, { 0, 0, 0 } },
                { { 1, 1, 0 }, { 4, 0, 0 } },
                { { 1, 1, 0 }, { 0, 1, 1 } },
                { { 1, 1, 0 }, { 8, 0, 1 } },
                { { 1, 1, 0 }, { 0, 0, 1 } },
                { { 0, 0, 0 }, { 3, 1, 1 } }, 
            }
        ));

        // Level 6
        Levels.Add(6, new LevelData(
            new int[,,]
            {
                { { 2, 0, 6, 1 }, { 0, 0, 0, 0 }, { 0, 1, 4, 0 } },
                { { 1, 0, 0, 1 }, { 0, 1, 1, 1 }, { 5, 0, 0, 4 } },
                { { 1, 1, 0, 1 }, { 0, 4, 0, 0 }, { 0, 0, 6, 1 } },
                { { 0, 0, 0, 1 }, { 1, 1, 1, 1 }, { 0, 0, 0, 0 } },
                { { 1, 0, 1, 1 }, { 4, 0, 5, 0 }, { 0, 3, 0, 0 } },
                { { 0, 0, 0, 0 }, { 1, 1, 0, 0 }, { 0, 1, 0, 0 } },
                { { 0, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 5, 0, 0 } },
                { { 0, 0, 0, 0 }, { 1, 1, 1, 1 }, { 0, 0, 0, 0 } },
                { { 0, 0, 0, 0 }, { 1, 1, 1, 1 }, { 0, 0, 0, 0 } },
            },
            new Dictionary<int, int>
            {
                { 002, 0222 },
                { 222, 3102 }
            }
        ));
        
        // Level 7
        Levels.Add(7, new LevelData(
            new int[,,]
            {
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
            },
            new Dictionary<int, int>
            {
                { 334, 4021},
                { 432, 5021},
                { 110, 6021}
            }
        ));
        
        // Level 8
        Levels.Add(8, new LevelData(
            new int[,,]
            {
                { { 1, 1, 1, 1, 1}, { 4, 0, 0, 0, 0}, { 0, 0, 1, 1, 1 }, { 0, 0, 0, 0, 0 } },
                { { 0, 0, 1, 0, 1}, { 6, 0, 1, 0, 0}, { 0, 0, 0, 1, 1 }, { 0, 0, 0, 0, 4 } },
                { { 0, 0, 1, 0, 1}, { 0, 0, 1, 1, 0}, { 1, 1, 0, 5, 0 }, { 0, 4, 0, 0, 1 } },
                { { 0, 0, 0, 0, 1}, { 0, 0, 2, 0, 4}, { 1, 0, 0, 0, 0 }, { 0, 1, 0, 0, 6 } },
                { { 0, 0, 1, 0, 0}, { 1, 1, 0, 0, 1}, { 5, 0, 0, 0, 0 }, { 0, 1, 6, 0, 0 } },
                { { 0, 0, 1, 0, 0}, { 1, 1, 0, 1, 1}, { 0, 4, 0, 0, 4 }, { 0, 0, 0, 1, 0 } },
                { { 0, 0, 1, 0, 0}, { 0, 0, 0, 0, 0 }, { 1, 1, 0, 0, 1 }, { 4, 0, 0, 1, 0 } },
                { { 0, 0, 0, 0, 0}, { 0, 0, 3, 0, 0 }, { 0, 0, 0, 1, 1 }, { 1, 1, 1, 5, 0 } },
            },
            new Dictionary<int, int>
            {
                { 334, 4021},
                { 432, 5021},
                { 110, 6021 }
            }
        ));
    }

    public void BuildDictionary()
    {
        ThemeData theme = ThemeManager.Instance.currentTheme;

        BlockTemplates = new Dictionary<int, GameObject>()
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
            {9, theme.storyModel},
        };
    }

    // Methods from multi-scene branch
    private int[,,] GetLevelTemplate(int levelIndex)
    {
         if (Levels.TryGetValue(levelIndex, out LevelData levelData))
         {
             return levelData.LevelTemplate;
         }
         Debug.LogError($"Level {levelIndex} not found");
         return null;
    }

    // Function for building the level
    void BuildLevel(int levelIndex)
    {
        // Grab the level template 
        int[,,] levelTemplate = GetLevelTemplate(levelIndex);
        
        if (levelTemplate == null)
        {
            Debug.LogError("BuildLevel: Level template is null.");
            return;
        }

        Transform parent = GameObject.Find("Level").transform;

        // Initialize CurrentLevel array in GameManager
        gameManager.CurrentLevel = new GameObject[
            levelTemplate.GetLength(0),
            levelTemplate.GetLength(1),
            levelTemplate.GetLength(2)];

        for (int i = 0; i < levelTemplate.GetLength(0); i++) // x-axis
        {
            for (int k = 0; k < levelTemplate.GetLength(1); k++) // y-axis
            {
                for (int j = 0; j < levelTemplate.GetLength(2); j++) // z-axis
                {
                    int blockID = levelTemplate[i, k, j];

                    // Check if the blockID exists in the dictionary
                    if (!BlockTemplates.ContainsKey(blockID))
                    {
                        Debug.LogWarning($"LevelBuilder: Block ID {blockID} not found in BlockTemplates.");
                        gameManager.CurrentLevel[i, k, j] = null; // Explicitly set to null
                        continue;
                    }

                    GameObject blockPrefab = BlockTemplates[blockID];

                    // Instantiate the block
                    GameObject block = Instantiate(blockPrefab, parent);
                    block.transform.position = new Vector3(i, k, j);

                    // Assign the block to CurrentLevel
                    gameManager.CurrentLevel[i, k, j] = block;

                    // Rotate the ladder if necessary
                    if (blockID == 5) // Ladder to the left
                    {
                        block.transform.Rotate(0, 180, 0);
                    }

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
                        _startBlockPosition = new Vector3(i, k, j);
                    }
                }
            }
        }
    }

    // Methods from tempbranch
    public GameObject[,,] RemoteBuild()
    {
        BuildDictionary();
        levelTemplate = null;
        switch (currentLevel)
        {
            case 0:
                testlevel();
                break;
            case 1:
                basic2();
                break;
            case 2:
                basic3();
                break;
            case 3:
                box1();
                break;
            case 4:
                box2();
                break;
            case 5:
                box3();
                break;
            case 6:
                lever1();
                break;
            case 7:
                lever2();
                break;
            case 8:
                lever3();
                break;
            // Add more cases for additional levels as needed
            default:
                Debug.LogWarning("LevelBuilder: No setup defined for currentLevel index.");
                break;
        }

        return buildLevel(levelParent.transform, levelTemplate);
    }


    void testlevel()
    {
        levelTemplate = new int[,,] {
            { {2}, {0} },
            { {1}, {0} },
            { {1}, {9} },
            { {1}, {0} },
            { {1}, {9} },
            { {1}, {0} },
            { {1}, {9} },
            { {1}, {0} },
            { {1}, {9} },
            { {3}, {0} },
        };

        storyBlockTexts.Clear();
        storyBlockTexts.Add(new Vector3Int(2, 1, 0), "HELLO"); //(SIDEVIEW DIRECTION, HEIGHT, TOPVIEW DIRECTION)
        storyBlockTexts.Add(new Vector3Int(4, 1, 0), "HELLO THERE");
        storyBlockTexts.Add(new Vector3Int(6, 1, 0), "HELLO AGAIN");
        storyBlockTexts.Add(new Vector3Int(8, 1, 0), "123");
    }


    /*  ---------------- BASIC ---------------- */

    void basic1()
    {
        levelTemplate = new int[,,] {
            { {2}, {0} },
            { {1}, {9} },
            { {7}, {0} },
            { {1}, {1} },
            { {1}, {0} },
            { {3}, {0} },
        };

        storyBlockTexts.Clear();
        storyBlockTexts.Add(new Vector3Int(1, 1, 0), "HELLO"); //(SIDEVIEW DIRECTION, HEIGHT, TOPVIEW DIRECTION)
    }

    void basic2()
    {
        levelTemplate = new int[,,] {
            { { 2, 0, 3 }, { 0, 0, 0 } },
            { { 1, 0, 1 }, { 0, 0, 4 } },
            { { 1, 1, 0 }, { 0, 4, 1 } },
            { { 1, 0, 0 }, { 0, 1, 1 } }
        };
    }

    void basic3()
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


    /*  ---------------- BOX LEVELS ---------------- */
    void box1()
    {
        levelTemplate = new int[,,] {
            { { 0 }, { 2 }, { 0 } },
            { { 0 }, { 1 }, { 0 } },
            { { 0 }, { 1 }, { 8 } },
            { { 0 }, { 1 }, { 0 } },
            { { 0 }, { 1 }, { 0 } },
            { { 1 }, { 0 }, { 0 } },
            { { 0 }, { 1 }, { 0 } },
            { { 0 }, { 1 }, { 0 } },
            { { 0 }, { 3 }, { 0 } },
        };
    }

    void box2()
    {
        levelTemplate = new int[,,] {
            { { 0, 0, 2, 0, 0 }, { 0, 0, 0, 0, 0 } },
            { { 0, 0, 1, 0, 0 }, { 0, 0, 0, 0, 0 } },
            { { 0, 0, 1, 1, 1 }, { 0, 0, 4, 0, 0 } },
            { { 0, 0, 1, 0, 1 }, { 1, 1, 0, 0, 0 } },
            { { 0, 0, 1, 0, 1 }, { 1, 0, 8, 0, 0 } },
            { { 0, 0, 1, 1, 1 }, { 1, 0, 0, 0, 0 } },
            { { 0, 0, 0, 0, 0 }, { 1, 1, 3, 0, 0 } },
        };
    }

    void box3()
    {
        levelTemplate = new int[,,] {
            { { 0, 0, 0, 0, 0, 0, 0 }, { 0, 1, 1, 1, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 } },
            { { 1, 0, 0, 0, 0, 0, 0 }, { 0, 1, 0, 1, 0, 0, 0 }, { 0, 0, 0, 8, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 } },
            { { 1, 1, 0, 0, 0, 0, 0 }, { 0, 5, 0, 1, 0, 0, 0 }, { 0, 0, 0, 0, 1, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 } },
            { { 1, 1, 0, 0, 0, 0, 0 }, { 0, 4, 1, 1, 0, 0, 0 }, { 0, 0, 0, 5, 1, 1, 0 }, { 0, 0, 0, 0, 4, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 } },
            { { 1, 0, 0, 0, 0, 0, 0 }, { 0, 1, 1, 0, 1, 1, 0 }, { 0, 0, 0, 0, 0, 5, 0 }, { 0, 0, 0, 0, 1, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 } }, 
            { { 1, 0, 0, 0, 0, 0, 0 }, { 4, 0, 1, 0, 1, 1, 0 }, { 0, 0, 0, 0, 8, 0, 0 }, { 0, 0, 0, 0, 1, 0, 0 }, { 0, 0, 0, 0, 8, 0, 0 } },
            { { 0, 0, 0, 0, 1, 0, 0 }, { 1, 1, 2, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 } }, 
            { { 0, 0, 0, 0, 0, 0, 0 }, { 1, 0, 1, 1, 1, 1, 1 }, { 4, 0, 0, 0, 0, 0, 4 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 } },
            { { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 1, 0, 1, 1, 0 }, { 1, 0, 0, 0, 8, 0, 1 }, { 0, 0, 0, 0, 4, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 } },
            { { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 1, 1, 1, 1, 0 }, { 1, 0, 0, 0, 0, 8, 1 }, { 4, 0, 1, 1, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 } },
            { { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 1, 0, 0, 1, 0 }, { 0, 0, 0, 0, 1, 0, 1 }, { 1, 0, 1, 0, 8, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 } },
            { { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 1, 0, 0, 1, 0 }, { 0, 0, 0, 0, 1, 4, 1 }, { 1, 0, 1, 0, 0, 0, 0 }, { 8, 0, 8, 0, 0, 0, 0 } },
            { { 1, 0, 1, 0, 0, 0, 0 }, { 0, 1, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 1, 1, 1 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 } },
            { { 0, 0, 1, 0, 0, 0, 0 }, { 1, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 } },
            { { 0, 0, 0, 0, 0, 0, 0 }, { 1, 1, 3, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 } },
        };
    }

    /*  --------------- LEVER LEVELS --------------- */


    void lever1()
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
        interActPairs.Add(200, 3102); // Last digit refers to block direction. // 1=UP-DOWN, 2=DOWN-UP, 3=LEFT-RIGHT, 4=RIGHT-LEFT
    }


    void lever2()
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

    void lever3()
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

    /*  -------------- COMBINED LEVELS ------------- */

    /*  -------------------------------------------- */


    public GameObject[,,] buildLevel(Transform parent, int[,,] lt)
    {
        if (lt == null)
        {
            Debug.LogError("buildLevel: Level template is null.");
            return null;
        }

        for (int i = 0; i < lt.GetLength(0); i++) // x-axis
        {
            for (int k = 0; k < lt.GetLength(1); k++) // y-axis
            {
                for (int j = 0; j < lt.GetLength(2); j++) // z-axis
                {
                    int blockID = lt[i, k, j];

                    // Check if the blockID exists in the dictionary
                    if (!BlockTemplates.ContainsKey(blockID))
                    {
                        Debug.LogWarning($"LevelBuilder: Block ID {blockID} not found in BlockTemplates.");
                        gameManager.CurrentLevel[i, k, j] = null; // Explicitly set to null
                        continue;
                    }

                    GameObject blockPrefab = BlockTemplates[blockID];

                    // Instantiate the block
                    GameObject block = Instantiate(blockPrefab, parent);
                    block.transform.position = new Vector3(i, k, j);

                    // Assign the block to CurrentLevel
                    gameManager.CurrentLevel[i, k, j] = block;

                    // Rotate the ladder if necessary
                    if (blockID == 5) // Ladder to the left
                    {
                        block.transform.Rotate(0, 180, 0);
                    }

                    // Set the blockType
                    Block blockComponent = block.GetComponent<Block>();
                    if (blockComponent != null)
                    {
                        blockComponent.blockType = GetBlockTypeFromID(blockID);
                        blockComponent.ApplyTheme(); // Apply the theme after setting blockType

                        if (blockID == 9)
                        {
                            
                            Vector3Int coord = new Vector3Int(i, k, j);
                            if (storyBlockTexts.ContainsKey(coord))
                            {
                                blockComponent.storyText = storyBlockTexts[coord];
                            }
                            else
                            {
                                blockComponent.storyText = "You discovered a piece of the story!";
                            }

                            //Collider col = block.GetComponent<Collider>();
                            //if (col != null) col.enabled = false; 
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"LevelBuilder: No Block component found on prefab for Block ID {blockID}.");
                    }

                    // Check if the block is the startTemplate
                    if (blockID == 2) // Assuming 2 corresponds to startTemplate
                    {
                        _startBlockPosition = new Vector3(i, k, j);
                    }
                }
            }
        }

        return gameManager.CurrentLevel;
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
            case 9:
                return "story";
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
            "left ladder",
            "story"
        };
    }
}


/*
    void level11TemplateSetup() // DONE
    {
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
*/
