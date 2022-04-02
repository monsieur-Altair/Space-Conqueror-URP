using System.Linq;
using UnityEngine;

namespace _Application.Scripts.Scriptables.Upgrades
{
    [CreateAssetMenu(fileName = "new upgrade info",menuName = "Resources/Upgrade info")]
    public class UpgradeInfo : ScriptableObject
    {
        public SingleUpgradeStats[] upgradesStats;

        public SingleUpgradeStats? GetUpgradeStats(int number) => 
            number < upgradesStats.Length 
                ? upgradesStats.First(stats => stats.number == number) //upgradesStats[number]
                : (SingleUpgradeStats?) null;
    }
}