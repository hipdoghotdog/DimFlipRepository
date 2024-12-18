using UnityEngine;
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

    private void LoadLevel(int levelIndex)
    {
        currentLevelIndex = levelIndex;
        currentView = View.SideView;

        // Load the scene corresponding to the level
        SceneManager.LoadScene("Level" + levelIndex);

        // Initialize managers after the scene is loaded
        SceneManager.sceneLoaded += OnSceneLoaded;
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
    }

    public void NextLevel()
    {
        LoadLevel(currentLevelIndex + 1);
    }
}