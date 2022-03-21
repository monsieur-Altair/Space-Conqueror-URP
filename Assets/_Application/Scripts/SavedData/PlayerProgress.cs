using System;

namespace _Application.Scripts.SavedData
{
    [Serializable]
    public class PlayerProgress
    {
        public LevelInfo levelInfo;

        public PlayerProgress(int levelNumber) => 
            levelInfo = new LevelInfo(levelNumber);
    }
}