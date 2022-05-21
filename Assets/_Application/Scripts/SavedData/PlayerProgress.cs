using System;
using System.Linq;

namespace _Application.Scripts.SavedData
{
    [Serializable]
    public class PlayerProgress
    {
        public LevelInfo LevelInfo;
        public int Money;
        public AchievedUpgrades[] AchievedUpgrades;
        public Statistic Statistic;
        
        
        public PlayerProgress(int levelNumber)
        {
            Money = 0;
            LevelInfo = new LevelInfo(levelNumber);
            Statistic = new Statistic();
            
            CreateAchieveUpgrades();
        }

        public AchievedUpgrades GetAchievedUpgrade(UpgradeType upgradeType) => 
            AchievedUpgrades.First(upgrades => upgrades.upgradeType == upgradeType);
        //AchievedUpgrades[(int)upgradeType];

        private void CreateAchieveUpgrades()
        {
            int length = Enum.GetValues(typeof(UpgradeType)).Length;
            AchievedUpgrades = new AchievedUpgrades[length];
            
            for (int i = 0; i < length; i++)
                AchievedUpgrades[i] = new AchievedUpgrades((UpgradeType) (i), 0, 1.0f);
        }
    }
}