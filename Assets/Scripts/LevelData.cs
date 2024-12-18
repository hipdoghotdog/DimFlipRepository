using System.Collections.Generic;

namespace DefaultNamespace
{
    public class LevelData
    {
        public int[,,] LevelTemplate { get; set; }
        public Dictionary<int, int> InterActPairs { get; set; }

        public LevelData(int[,,] template, Dictionary<int, int> pairs = null)
        {
            LevelTemplate = template;
            InterActPairs = pairs ?? new Dictionary<int, int>();
        }
    }
}