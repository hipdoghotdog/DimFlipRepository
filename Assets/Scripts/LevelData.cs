using System.Collections.Generic;
using UnityEngine;

public class LevelData
{
    public int[,,] LevelTemplate { get; set; }
    public Dictionary<int, int> InterActPairs { get; set; }
    public Dictionary<Vector3Int, string> StoryBlocks { get; set; }

    public LevelData(int[,,] template, Dictionary<int, int> pairs = null, Dictionary<Vector3Int, string> storyBlocks = null)
    {
        LevelTemplate = template;
        InterActPairs = pairs ?? new Dictionary<int, int>();
        StoryBlocks = storyBlocks ?? new Dictionary<Vector3Int, string>();
    }
}