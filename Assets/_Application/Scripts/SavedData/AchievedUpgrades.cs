using System;

namespace _Application.Scripts.SavedData
{
    [Serializable]
    public class AchievedUpgrades
    {
        public UpgradeType upgradeType;
        public int numberOfCompletedCells;
        //public int completedTries;
        //public int countOfTries;
        public float upgradeCoefficient;

        public AchievedUpgrades(UpgradeType upgradeType, int numberOfCompletedCells, float upgradeCoefficient)
        {
            this.numberOfCompletedCells = numberOfCompletedCells;
            this.upgradeCoefficient = upgradeCoefficient;
            this.upgradeType = upgradeType;
        }
    }
}