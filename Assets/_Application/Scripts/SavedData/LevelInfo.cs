using System;

namespace _Application.Scripts.SavedData
{
    [Serializable]
    public class LevelInfo
    {
        public int lastCompletedLevel;

        public LevelInfo(int levelNumber) =>
            lastCompletedLevel = levelNumber;
    }
}