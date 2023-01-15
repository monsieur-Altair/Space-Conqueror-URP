using System.Collections.Generic;
using _Application.Scripts.Infrastructure.Services;
using _Application.Scripts.SavedData;
using _Application.Scripts.Scriptables.Rewards;
using _Application.Scripts.Scriptables.Upgrades;
using UnityEngine;

namespace _Application.Scripts.Managers
{
    [CreateAssetMenu (fileName = "CoreConfig",menuName = "Resources/Core Config")]
    public class CoreConfig: ScriptableObject, IService
    {
        [SerializeField] private MonoBehaviourServices _monoBehaviourServices;
        [Space, SerializeField] private bool _useTutorial;
        [SerializeField] private PlayerConfig _playerConfig;
        [SerializeField] private PlayerConfig _aiConfig;
        [Space, SerializeField] private MyDictionary<UpgradeType, UpgradeInfo> _upgrades;
        [SerializeField] private RewardList _rewardList;
        [SerializeField] private Warehouse _warehouse;
        [SerializeField] private PoolObjects _poolObjects;
        
        public MonoBehaviourServices MonoBehaviourServices => _monoBehaviourServices;
        public bool UseTutorial => _useTutorial;
        public PlayerConfig PlayerConfig => _playerConfig;
        public PlayerConfig AIConfig => _aiConfig;
        public MyDictionary<UpgradeType, UpgradeInfo> Upgrades => _upgrades;
        public RewardList RewardList => _rewardList;
        public Warehouse Warehouse => _warehouse;
        public PoolObjects PoolObjects => _poolObjects;
    }
}