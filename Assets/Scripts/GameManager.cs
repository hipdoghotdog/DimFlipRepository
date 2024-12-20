using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject[,,] CurrentLevel;
    
    [HideInInspector]
    public int currentLevelIndex;
    
    [HideInInspector]
    public LevelBuilder levelBuilder;
    
    [HideInInspector]
    public LevelFlipper levelFlipper;
    
    [HideInInspector]
    public GameObject player;
    
    [HideInInspector]
    public PlayerMovement playerMovement;
    
    [HideInInspector]
    public CameraScript cameraScript;
    
    [HideInInspector]
    public ArtifactManager artifactManager;

    [HideInInspector]
    public View currentView;

    public enum View
    {
        SideView,
        TopdownView
    }
    
    [HideInInspector] public HashSet<int> CompletedLevels = new HashSet<int>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist between scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadLevel(currentLevelIndex);
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            ResetLevel();
        }

        else if (Input.GetKeyDown(KeyCode.Escape))
        {
           MenuManager.Instance.ToggleMenu();
        }
    }

    public void LoadLevel(int levelIndex)
    {
        currentLevelIndex = levelIndex;
        
        MenuManager.Instance.buttonDictionary[levelIndex].interactable = true;
        
        currentView = View.SideView;

        // Load the scene corresponding to the level
        SceneManager.LoadScene("Level" + levelIndex);

        // Initialize managers after the scene is loaded
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        EventSystem.current.SetSelectedGameObject(null);
    }

    private void ResetLevel()
    {
        LoadLevel(0);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        // Find or initialize managers for the new scene
        InitializeManagers();
    }

    private void InitializeManagers()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        
        levelBuilder = FindObjectOfType<LevelBuilder>();
        levelFlipper = FindObjectOfType<LevelFlipper>();
        playerMovement = FindObjectOfType<PlayerMovement>();
        cameraScript = FindObjectOfType<CameraScript>();
        artifactManager = FindObjectOfType<ArtifactManager>();

        if (levelBuilder) levelBuilder.Initialize(currentLevelIndex);
        if (levelFlipper) levelFlipper.Initialize();
        if (artifactManager) artifactManager.Initialize();
        if (playerMovement) playerMovement.Initialize();
    }

    public void NextLevel()
    {
        if (currentLevelIndex + 1 > levelBuilder.Levels.Count)
        {
            SceneManager.LoadScene("MenuScreen");
            return;
        }
        
        CompletedLevels.Add(currentLevelIndex);
        LoadLevel(currentLevelIndex + 1);
    }

    public void CheckGravity()
    {
        bool blocksFell = true;
        while (blocksFell)
        {
            blocksFell = false;

            for (int x = 0; x < CurrentLevel.GetLength(0); x++)
            {
                for (int y = 0; y < CurrentLevel.GetLength(1); y++)
                {
                    for (int z = 0; z < CurrentLevel.GetLength(2); z++)
                    {
                        GameObject go = CurrentLevel[x, y, z];
                        if (go == null) continue;

                        Block block = go.GetComponent<Block>();
                        if (block != null && block.isPushable && block.isActive)
                        {
                            int belowY = y - 1;
                            if (belowY < 0)
                            {
                                // Block falls out of the level
                                // Instead of destroying immediately, animate it falling out.
                                // Let's say we drop it one extra unit below:
                                Vector3 fallOutPos = new Vector3(x, -1, z);
                                StartCoroutine(FallOutOfLevelCoroutine(go, fallOutPos));

                                CurrentLevel[x, y, z] = null;
                                blocksFell = true;
                            }
                            else
                            {
                                // Check if below is empty
                                GameObject belowBlockGO = CurrentLevel[x, belowY, z];
                                if (belowBlockGO == null)
                                {
                                    // Just fall down
                                    StartCoroutine(MoveBlockCoroutine(block, new Vector3(x, belowY, z)));
                                    CurrentLevel[x, belowY, z] = go;
                                    CurrentLevel[x, y, z] = null;
                                    blocksFell = true;
                                }
                                else
                                {
                                    Block belowBlock = belowBlockGO.GetComponent<Block>();
                                    if (belowBlock != null && belowBlock.blockType.ToLower() == "empty")
                                    {
                                        // Swap with empty block
                                        StartCoroutine(MoveBlockCoroutine(block, new Vector3(x, belowY, z)));
                                        CurrentLevel[x, belowY, z] = go;
                                        CurrentLevel[x, y, z] = belowBlockGO;
                                        blocksFell = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public IEnumerator MoveBlockCoroutine(Block block, Vector3 targetPosition)
    {
        if (block == null) yield break; // Safety check

        float duration = 0.2f;
        float elapsed = 0f;
        Vector3 initialPosition = block.transform.position;

        // Make sure the block's GameObject is still alive
        GameObject blockGO = block.gameObject;

        while (elapsed < duration)
        {
            if (blockGO == null) yield break; // Block got destroyed externally?
            blockGO.transform.position = Vector3.Lerp(initialPosition, targetPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (blockGO != null)
        {
            blockGO.transform.position = targetPosition;
        }
    }

    // New coroutine for falling out of the level
    private IEnumerator FallOutOfLevelCoroutine(GameObject blockGO, Vector3 targetPosition)
    {
        Block block = blockGO.GetComponent<Block>();
        if (block == null) yield break;

        // Animate the block falling out of the level
        yield return MoveBlockCoroutine(block, targetPosition);

        // After finishing the animation, we can now safely destroy the block
        if (blockGO != null)
        {
            Destroy(blockGO);
        }
    }
}