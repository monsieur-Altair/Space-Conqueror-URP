using System;
using System.Linq;
using UnityEngine;

namespace _Application.Scripts.Scriptables.Rewards
{
    [CreateAssetMenu(fileName = "new reward list",menuName = "Resources/Reward list")]
    public class RewardList : ScriptableObject
    {
        public Reward[] rewards;

        public int GetReward(int completedLevel) => 
            rewards.First(reward => reward.level == completedLevel).money;
    }
    
    [Serializable]
    public struct Reward
    {
        public int level;
        public int money;
    }
}