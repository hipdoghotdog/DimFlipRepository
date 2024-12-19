using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    // Singleton Instance
    public static GameManager Instance;

    // Level Management
    public int STARTLEVEL = 7;
    public GameObject[,,] CurrentLevel;

    [HideInInspector]
    public int currentLevelIndex;

    [HideInInspector]
    public LevelBuilder levelBuilder;

    [HideInInspector]
    public LevelFlipper levelFlipper;

    // Player and Camera References
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

    private void Awake()
    {
        // Singleton Pattern Implementation
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist between scenes
        }
        else
        {
            Destroy(gameObject);
            return; // Exit to prevent further initialization
        }

        // Initialize components if necessary
        InitializeInitialComponents();
    }

    private void InitializeInitialComponents()
    {
        // Display a welcome message or perform initial setup
        // Assuming 'am' is an ArtifactManager or similar; ensure it's initialized
        if (artifactManager != null)
        {
            artifactManager.DisplayText("Hello There!");
        }
        else
        {
            Debug.LogWarning("ArtifactManager is not assigned in the GameManager.");
        }

        // Initialize CameraScript
        Camera cam = Camera.main;
        if (cam != null)
        {
            cameraScript = cam.GetComponent<CameraScript>();
        }
        else
        {
            Debug.LogWarning("Main Camera not found in the scene.");
        }

        // Load the starting level
        LoadNewLevel(STARTLEVEL);

        // Initialize PlayerMovement
        if (player != null)
        {
            playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.ResetPlayerPosition();
            }
            else
            {
                Debug.LogWarning("PlayerMovement component not found on the player GameObject.");
            }
        }
        else
        {
            Debug.LogWarning("Player GameObject is not assigned in the GameManager.");
        }
    }

    public void Flip(Vector3 pp, bool camFlip = true)
    {
        if (levelFlipper != null)
        {
            levelFlipper.FlipLevel(pp, currentView);
        }
        else
        {
            Debug.LogWarning("LevelFlipper is not assigned in the GameManager.");
            return;
        }

        // Toggle the current view
        currentView = currentView == View.SideView ? View.TopdownView : View.SideView;

        if (currentView == View.TopdownView && camFlip)
        {
            // Additional camera flip logic can be added here if necessary
            // For example, adjusting camera angles or positions
        }
    }

    private void Start()
    {
        LoadLevel(currentLevelIndex);
    }

    private void LoadLevel(int levelIndex)
    {
        currentLevelIndex = levelIndex;
        currentView = View.SideView;

        // Load the scene corresponding to the level
        SceneManager.LoadScene("Level" + levelIndex);

        // Initialize managers after the scene is loaded
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void LoadNewLevel(int levelNumber)
    {
        // Implementation for loading a new level
        // This method can be expanded based on specific requirements
        LoadLevel(levelNumber);
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

        if (levelBuilder != null)
        {
            levelBuilder.Initialize(currentLevelIndex);
        }
        else
        {
            Debug.LogWarning("LevelBuilder not found in the scene.");
        }

        if (levelFlipper != null)
        {
            levelFlipper.Initialize();
        }
        else
        {
            Debug.LogWarning("LevelFlipper not found in the scene.");
        }

        if (artifactManager != null)
        {
            artifactManager.Initialize();
        }
        else
        {
            Debug.LogWarning("ArtifactManager not found in the scene.");
        }
    }

    public void NextLevel()
    {
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

    // Coroutine for falling out of the level
    public IEnumerator FallOutOfLevelCoroutine(GameObject blockGO, Vector3 targetPosition)
    {
        Block block = blockGO.GetComponent<Block>();
        if (block == null) yield break;

        // Animate the block falling out of the level
        yield return MoveBlockCoroutine(block, targetPosition);

        // After finishing the animation, safely destroy the block
        if (blockGO != null)
        {
            Destroy(blockGO);
        }
    }
}
