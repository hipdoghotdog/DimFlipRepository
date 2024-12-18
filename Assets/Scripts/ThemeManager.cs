using UnityEngine;

public class ThemeManager : MonoBehaviour
{
    public ThemeData currentTheme;

    public static ThemeManager Instance { get; private set; }

    private void Awake()
    {
        // Implement singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ChangeTheme(ThemeData newTheme)
    {
        currentTheme = newTheme;
        Block[] allBlocks = FindObjectsOfType<Block>();
        foreach (Block block in allBlocks)
        {
            block.ApplyTheme();
        }
    }
}
