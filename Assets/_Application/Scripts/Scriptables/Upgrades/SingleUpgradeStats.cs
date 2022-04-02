using System;
using UnityEngine;

namespace _Application.Scripts.Scriptables.Upgrades
{
    [Serializable]
    public struct SingleUpgradeStats
    {
        [Min(0)]
        public int number;
        
        [Min(50)]
        public int cost;

        [Range(0.0f, 0.7f)]
        public float upgradeCoefficient;
        
        // [Min(1)]
        // public int minCountOfTries;
        //
        // [Min(1)]
        // public int maxCountOfTries;
    }
}