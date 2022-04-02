using System;
using System.Linq;
using JetBrains.Annotations;

namespace _Application.Scripts.SavedData
{
    [Serializable]
    public class PlayerProgress
    {
        public LevelInfo levelInfo;
        public int money;
        public AchievedUpgrades[] achievedUpgrades;
        
        public PlayerProgress(int levelNumber)
        {
            money = 0;
            levelInfo = new LevelInfo(levelNumber);
            
            CreateAchieveUpgrades();
        }

        public AchievedUpgrades GetAchievedUpgrade(UpgradeType upgradeType) => 
            achievedUpgrades.First(upgrades => upgrades.upgradeType == upgradeType);

        private void CreateAchieveUpgrades()
        {
            int length = Enum.GetValues(typeof(UpgradeType)).Length;
            achievedUpgrades = new AchievedUpgrades[length];
            
            for (int i = 0; i < length; i++)
                achievedUpgrades[i] = new AchievedUpgrades((UpgradeType) (i), 0, 1.0f);
        }
    }
}