using System;

namespace _Application.Scripts.SavedData
{
    [Serializable]
    public class PlayerProgress
    {
        public LevelInfo levelInfo;
        public int money;
        
        public PlayerProgress(int levelNumber)
        {
            money = 0;
            levelInfo = new LevelInfo(levelNumber);
        }
    }
}