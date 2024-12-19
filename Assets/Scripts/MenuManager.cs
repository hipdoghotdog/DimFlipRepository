using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    
    public GameObject buttonPrefab; // Prefab for the button
    public Transform buttonsParent; // Parent object with a GridLayoutGroup
    public Canvas menuCanvas;
    public EventSystem eventSystem;
    
    private readonly int[] _levels = Enumerable.Range(0, 11).ToArray();

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
        DontDestroyOnLoad(menuCanvas);
        DontDestroyOnLoad(eventSystem);
        PopulateMenu();
        menuCanvas.enabled = false;
    }

    private void PopulateMenu()
    {
        if (_levels.Length == 0)
        {
            Debug.LogError("MenuManager: No scenes assigned.");
            return;
        }

        foreach (int level in _levels)
        {
            CreateButton(level);
        }
    }

    private void CreateButton(int level)
    {
        // Instantiate button
        GameObject button = Instantiate(buttonPrefab, buttonsParent);
        button.SetActive(true);

        // Set button text
        Text buttonText = button.GetComponentInChildren<Text>();
        if (buttonText != null)
        {
            buttonText.text = "Level " + level;
        }
        else
        {
            Debug.LogError("MenuManager: ButtonText " + level + " not found.");
        }
        
        int currentLevel = level;

        // Add click listener to load the scene
        Button buttonComponent = button.GetComponent<Button>();
        if (buttonComponent != null)
        {
            buttonComponent.onClick.AddListener(() =>
            {
                LoadLevel(currentLevel);
                ToggleMenu();
            });
        }
        else
        {
            Debug.LogError("MenuManager: Button " + level + " not found.");
        }
    }

    private void LoadLevel(int level)
    {
        // Ensure the scene is in the build settings
        if (!Application.CanStreamedLevelBeLoaded("Level" + level))
        {
            Debug.LogError($"MenuManager: Level {level} is not in the build settings.");
            return;
        }

        // Load the scene
        GameManager.Instance.LoadLevel(level);
    }

    public void ToggleMenu()
    {
        menuCanvas.enabled = !menuCanvas.enabled;
    }
}
