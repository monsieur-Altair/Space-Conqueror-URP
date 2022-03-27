using System;
using System.Linq;
using UnityEngine;

namespace _Application.Scripts.Scriptables.Upgrades
{
    [CreateAssetMenu(fileName = "new upgrade info",menuName = "Resources/Upgrade info")]
    public class UpgradeInfo : ScriptableObject
    {
        public SingleUpgradeStats[] upgradesStats;

        public SingleUpgradeStats GetUpgradeStats(int number) => 
            upgradesStats.First(stats => stats.number == number);
    }

    [Serializable]
    public struct SingleUpgradeStats
    {
        [Min(0)]
        public int number;
        
        [Min(50)]
        public int cost;

        [Range(0.0f, 0.7f)]
        public float upgradeCoefficient;
        
        [Min(1)]
        public int minCountOfTries;
        
        [Min(1)]
        public int maxCountOfTries;
    }
}