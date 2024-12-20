using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class LevelBuilder : MonoBehaviour
{
    [HideInInspector]
    public GameObject emptyTemplate;
    
    [HideInInspector]
    public GameManager gameManager;

    public ThemeData natureTheme;
    public ThemeData ruinsTheme;

    [FormerlySerializedAs("_startBlockPosition")] [HideInInspector] public Vector3 StartBlockPosition;

    public Dictionary<int, GameObject> BlockTemplates;
    public readonly Dictionary<int, LevelData> Levels = new();

    public void Initialize(int levelIndex)
    {
        gameManager = GameManager.Instance;

        ThemeData chosenTheme = (levelIndex < 6) ? ruinsTheme : natureTheme;
        ThemeManager.Instance.ChangeTheme(chosenTheme);

        InitializeLevels();
        BuildDictionary();
        BuildLevel(levelIndex);
    }
    
    private void InitializeLevels()
    {
        // Level 0
        Levels.Add(0, new LevelData(
            new[,,] {
                { {2}, {0} },
                { {1}, {0} },
                { {7}, {0} },
                { {1}, {9} },
                { {1}, {0} },
                { {1}, {0} },
                { {1}, {1} },
                { {1}, {0} },
                { {3}, {0} },
            },
            storyBlocks: new Dictionary<Vector3Int, string>
            {
                { new Vector3Int(3, 1, 0), "A sudden shift in perspective reveals hidden pathways previously obscured." },
            }
        ));

        // Level 1
        Levels.Add(1, new LevelData(
            new[,,] {
                { { 0, 0, 3 }, { 0, 0, 0 } },
                { { 2, 0, 1 }, { 0, 0, 9 } },
                { { 1, 0, 1 }, { 0, 0, 4 } },
                { { 1, 1, 0 }, { 0, 4, 1 } },
                { { 1, 0, 0 }, { 0, 1, 1 } }
            },
            storyBlocks: new Dictionary<Vector3Int, string>
            {
                { new Vector3Int(1, 1, 2), "The artifact glows when we speak its name. Perhaps it's a guide." },
            }
        ));

        // Level 2  
        Levels.Add(2, new LevelData(
            new[,,] {
                { {2,1,0,0}, {0,0,0,1}, {0,0,3,0}, {0,0,4,0} },
                { {0,1,0,0}, {0,0,0,1}, {0,0,0,4}, {0,1,1,0} },
                { {1,1,0,1}, {0,4,0,0}, {1,0,1,1}, {0,1,9,0} },
                { {1,0,1,1}, {9,1,0,4}, {1,0,1,0}, {0,1,4,0} },
                { {0,1,1,0}, {1,5,0,1}, {5,0,0,4}, {0,1,1,0} },
                { {1,0,1,1}, {5,0,0,0}, {0,0,1,1}, {0,0,5,0} }
            },
            storyBlocks: new Dictionary<Vector3Int, string>
            {
                { new Vector3Int(3, 1, 0), "The path begins in confusion, but every step forward brings clarity, or so they say." },
                { new Vector3Int(2, 3, 2), "They are always watching. Tread carefully." },
            }
        ));

        //Level 3 BOX 1 (Introduction)
        Levels.Add(3, new LevelData(
            new[, ,]
            {
                { { 0 }, { 2 }, { 0 } },
                { { 0 }, { 1 }, { 0 } },
                { { 0 }, { 1 }, { 8 } },
                { { 0 }, { 1 }, { 0 } },
                { { 0 }, { 1 }, { 0 } },
                { { 1 }, { 0 }, { 0 } },
                { { 0 }, { 1 }, { 0 } },
                { { 0 }, { 1 }, { 9 } },
                { { 0 }, { 3 }, { 0 } },
            },
            storyBlocks: new Dictionary<Vector3Int, string>
            {
                { new Vector3Int(7, 2, 0), "The only way out is through." },
            }
        ));

        // Level 4 BOX 2
        Levels.Add(4, new LevelData(
            new[, ,] {
                { { 0, 0, 2, 0, 0 }, { 0, 0, 0, 0, 0 } },
                { { 0, 0, 1, 0, 0 }, { 0, 0, 0, 0, 0 } },
                { { 0, 0, 1, 1, 1 }, { 0, 0, 4, 0, 0 } },
                { { 0, 0, 1, 0, 1 }, { 1, 1, 0, 0, 0 } },
                { { 0, 0, 1, 0, 1 }, { 1, 0, 8, 0, 9 } },
                { { 0, 0, 1, 1, 1 }, { 1, 0, 0, 0, 0 } },
                { { 0, 0, 0, 0, 0 }, { 1, 1, 3, 0, 0 } },
            },
            storyBlocks: new Dictionary<Vector3Int, string>
            {
                { new Vector3Int(7, 2, 0), "Hmmm, this note seems to be left by someone else, however the text is to smudged. Lets find another!" },
            }
        ));

        // Level 5 LEVER 1
        Levels.Add(5, new LevelData(
            new[,,] {
                { {2}, {0} },
                { {1}, {0} },
                { {6}, {0} },
                { {0}, {1} },
                { {1}, {0} },
                { {1}, {0} },
                { {1}, {9} },
                { {3}, {0} },
            },
            new Dictionary<int, int> {
                { 200, 3102 }
            },
            storyBlocks: new Dictionary<Vector3Int, string>
            {
                { new Vector3Int(6, 1, 0), "Day 3: I've been in here for a while now, but it seems to make more and more sense." },
            }
        ));

        // Level 6 LEVER 2
        Levels.Add(6, new LevelData(
            new[,,] {
                { { 2, 0, 0, 0 }, { 0, 0, 6, 1 }, { 0, 0, 0, 0 } },
                { { 1, 0, 1, 0 }, { 0, 1, 5, 1 }, { 0, 0, 0, 0 } },
                { { 1, 1, 0, 0 }, { 0, 0, 0, 1 }, { 0, 0, 0, 0 } },
                { { 1, 0, 0, 0 }, { 0, 6, 0, 1 }, { 0, 0, 0, 9 } },
                { { 6, 0, 1, 0 }, { 0, 1, 4, 1 }, { 0, 0, 0, 0 } },
                { { 0, 0, 0, 0 }, { 1, 1, 1, 1 }, { 0, 0, 0, 0 } },
                { { 3, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } }
            },
            new Dictionary<int, int> {
                { 400, 4112 },
                { 012, 2014 },
                { 311, 5102 }
            },
            storyBlocks: new Dictionary<Vector3Int, string>
            {
                { new Vector3Int(3, 2, 3), "Day 13: The artifact lets me see the world in entirely new ways... Maybe it'll help me find my way home." },
            }
        ));

        // Level 7 LEVER 3
        Levels.Add(7, new LevelData(
            new[, ,]
            {
                { { 1, 1, 1, 1, 1}, { 4, 0, 0, 0, 0}, { 0, 0, 1, 1, 1 }, { 0, 0, 0, 9, 0 } },
                { { 0, 0, 1, 0, 1}, { 6, 0, 1, 0, 0}, { 0, 0, 0, 1, 1 }, { 0, 0, 0, 0, 4 } },
                { { 0, 0, 1, 0, 1}, { 0, 0, 1, 1, 9}, { 1, 1, 0, 5, 0 }, { 0, 4, 0, 0, 1 } },
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
            },
            storyBlocks: new Dictionary<Vector3Int, string>
            {
                { new Vector3Int(0, 3, 3), "Day 17: I wonder how many levers there are..." },
                { new Vector3Int(2, 1, 4), "Day 19: I keep finding more and more levers." },
            }
            ));

        // Level 8 BOX 3
         Levels.Add(8, new LevelData(
         new[, ,]
         {
                { { 0, 0, 0, 0, 0, 0, 0 }, { 0, 1, 1, 1, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 } },
                { { 1, 0, 0, 0, 0, 0, 0 }, { 9, 1, 0, 1, 0, 0, 0 }, { 0, 0, 0, 8, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 } },
                { { 1, 1, 0, 0, 0, 0, 0 }, { 0, 5, 0, 1, 0, 0, 0 }, { 0, 0, 0, 0, 1, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 } },
                { { 1, 1, 0, 0, 0, 0, 0 }, { 0, 4, 1, 1, 0, 0, 0 }, { 0, 0, 0, 5, 1, 1, 0 }, { 0, 0, 0, 0, 4, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 } },
                { { 1, 0, 0, 0, 0, 0, 0 }, { 0, 1, 1, 0, 1, 1, 0 }, { 0, 0, 0, 0, 0, 5, 0 }, { 0, 0, 0, 0, 1, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 } },
                { { 1, 0, 0, 0, 0, 0, 0 }, { 4, 0, 1, 0, 1, 1, 0 }, { 0, 0, 0, 0, 8, 0, 0 }, { 0, 0, 0, 0, 1, 0, 0 }, { 0, 0, 0, 0, 8, 0, 0 } },
                { { 0, 0, 0, 0, 1, 0, 0 }, { 1, 1, 2, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 } },
                { { 0, 0, 0, 0, 0, 0, 0 }, { 1, 0, 1, 1, 1, 1, 1 }, { 4, 0, 0, 0, 0, 0, 4 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 } },
                { { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 1, 0, 1, 1, 0 }, { 1, 0, 0, 0, 8, 0, 1 }, { 0, 0, 0, 0, 4, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 } },
                { { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 1, 1, 1, 1, 0 }, { 1, 0, 0, 0, 0, 8, 1 }, { 4, 0, 1, 1, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 } },
                { { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 1, 0, 0, 1, 0 }, { 0, 0, 0, 0, 1, 9, 1 }, { 1, 0, 1, 0, 8, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 } },
                { { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 1, 0, 0, 1, 0 }, { 0, 0, 0, 0, 1, 4, 1 }, { 1, 0, 1, 0, 0, 0, 0 }, { 8, 0, 8, 0, 0, 0, 0 } },
                { { 1, 0, 1, 0, 0, 0, 0 }, { 0, 1, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 1, 1, 1 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 } },
                { { 0, 0, 1, 0, 0, 0, 0 }, { 1, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 } },
                { { 0, 0, 0, 0, 0, 0, 0 }, { 1, 1, 3, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 } },
             },
            storyBlocks: new Dictionary<Vector3Int, string>
            {
                { new Vector3Int(10, 2, 5), "Day 33: It seems the boxes might provide a path forward..." },
                { new Vector3Int(1, 1, 9), "Day 31: I've been stuck in here for days... I wonder if I'll ever make it out." },
            }
         ));

        // Level 9 Simple Combined level
        Levels.Add(9, new LevelData(
            new[, ,]
            {
                { { 0, 0, 0, 0, 0, 0}, { 0, 0, 0, 0, 0, 0}, { 0, 0, 1, 1, 1, 1}, { 0, 0, 0, 0, 0, 0}, { 0, 0, 0, 0, 0, 0}, },
                { { 0, 0, 0, 0, 0, 0}, { 0, 0, 0, 0, 0, 0}, { 0, 0, 1, 0, 0, 6}, { 0, 0, 9, 0, 0, 0}, { 0, 0, 0, 0, 0, 0}, },
                { { 0, 0, 0, 0, 0, 0}, { 1, 0, 1, 2, 0, 0}, { 0, 1, 0, 0, 0, 1}, { 0, 0, 0, 0, 0, 0}, { 0, 0, 0, 0, 0, 0}, },
                { { 0, 0, 0, 0, 0, 0}, { 1, 0, 0, 1, 0, 0}, { 4, 0, 0, 0, 0, 1}, { 0, 0, 0, 0, 0, 0}, { 0, 0, 0, 0, 0, 0}, },
                { { 0, 0, 0, 0, 0, 0}, { 0, 0, 1, 1, 1, 1}, { 1, 0, 0, 0, 0, 5}, { 0, 0, 0, 0, 0, 0}, { 0, 0, 0, 0, 0, 0}, },
                { { 0, 0, 1, 0, 0, 0}, { 0, 0, 0, 0, 0, 0}, { 1, 1, 0, 0, 0, 0}, { 0, 4, 0, 0, 0, 0}, { 0, 0, 0, 0, 0, 0}, },
                { { 0, 0, 0, 0, 0, 0}, { 1, 0, 0, 1, 3, 0}, { 5, 1, 1, 0, 0, 0}, { 0, 1, 1, 0, 0, 0}, { 0, 0, 8, 0, 0, 0}, },
                { { 0, 0, 0, 0, 0, 0}, { 6, 0, 0, 0, 0, 0}, { 0, 0, 0, 0, 0, 0}, { 0, 1, 1, 0, 0, 0}, { 0, 0, 0, 0, 0, 0}, },

            },
            new Dictionary<int, int>
            {
                { 125, 2212},
                { 710, 6222},
            },
            storyBlocks: new Dictionary<Vector3Int, string>
            {
                { new Vector3Int(1, 3, 2), "Day 39: Right... More boxes... of course..." },
            }
            ));


        /*
        // Level 6
        Levels.Add(7, new LevelData(
            new[,,]
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
                {002, 0222 },
                { 222, 3102 }
            }
        ));
        
        // Level 7
        Levels.Add(8, new LevelData(
            new[,,]
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
                {334, 4021},
                {432, 5021},
                {110, 6021}
            }
        ));
        */
    }
    
    private void BuildDictionary()
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
                        Debug.LogWarning($"LevelBuilder: Block ID {blockID} not found in blockTemplates.");
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
                            if (Levels[gameManager.currentLevelIndex].StoryBlocks.ContainsKey(coord))
                            {
                                blockComponent.storyText = Levels[gameManager.currentLevelIndex].StoryBlocks[coord];
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
                        StartBlockPosition = new Vector3(i, k, j);
                    }
                }
            }
        }
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