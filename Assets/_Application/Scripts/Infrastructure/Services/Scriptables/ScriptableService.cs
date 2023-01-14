using System;
using _Application.Scripts.Buildings;
using _Application.Scripts.Infrastructure.Services.AssetManagement;
using _Application.Scripts.SavedData;
using _Application.Scripts.Scriptables;
using _Application.Scripts.Scriptables.Rewards;
using _Application.Scripts.Scriptables.Upgrades;
using UnityEngine;
using Type = _Application.Scripts.Buildings.Type;

namespace _Application.Scripts.Infrastructure.Services.Scriptables
{
    public class ScriptableService : IService
    {
        private const int AIIndex = (int)Team.Red;
        private const int PlayerIndex = (int)Team.Blue;
        
        private const int AttackerIndex = (int)Type.Attacker;
        private const int AltarIndex = (int)Type.Altar;
        private const int SpawnerIndex = (int)Type.Spawner;
        
        private readonly AssetProvider _assetProvider;
        private readonly Building[,] _buildingsInfo = new Building[2,3];
        private readonly Unit[,] _unitsInfo = new Unit[2,3];
        private readonly Mana[] _mana = new Mana[2];
        
        private UpgradeInfo _rainUpgradeInfo;
        private UpgradeInfo _unitSpeedUpgradeInfo;
        private UpgradeInfo _unitAttackUpgradeInfo;
        private UpgradeInfo _buildingDefenceInfo;
        private UpgradeInfo _buildingsMaxCounInfo;

        public RewardList RewardList { get; private set; }

        #region PlayerStats

        public Acid PlayersAcid { get; private set; }
        public Buff PlayersBuff { get; private set; }
        public Call PlayersCall { get; private set; }
        public Ice PlayersIce { get; private set; }

        #endregion
        
        #region AIStats

        public Acid AIsAcid { get; private set; }
        public Buff AIsBuff { get; private set; }
        public Call AIsCall { get; private set; }

        #endregion


        public ScriptableService(AssetProvider assetProvider) => 
            _assetProvider = assetProvider;

        public void LoadAllScriptables()
        {
            LoadRewards();
            LoadAllPlayerStats();
            LoadAllAIStats();
            LoadAllUpgradesInfo();
        }

        public UpgradeInfo GetUpgradeInfo(UpgradeType upgradeType)
        {
            return upgradeType switch
            {
                UpgradeType.Rain => _rainUpgradeInfo,
                UpgradeType.UnitAttack => _unitAttackUpgradeInfo,
                UpgradeType.UnitSpeed => _unitSpeedUpgradeInfo,
                UpgradeType.BuildingDefence => _buildingDefenceInfo,
                UpgradeType.BuildingMaxCount => _buildingsMaxCounInfo,
                _ => throw new ArgumentOutOfRangeException(nameof(upgradeType), upgradeType, null)
            };
        }

        public Building GetBuildingInfo(Team team, Type type) => 
            _buildingsInfo[(int) team, (int) type];

        public Unit GetUnitInfo(Team team, Type type) => 
            _unitsInfo[(int) team, (int) type];

        public Mana GetManaInfo(Team team) => 
            _mana[(int) team];

        private void LoadAllUpgradesInfo()
        {
            _rainUpgradeInfo = _assetProvider.InstantiateScriptable<UpgradeInfo>(AssetPaths.RainUpgradeInfoPath);
            _unitAttackUpgradeInfo = _assetProvider.InstantiateScriptable<UpgradeInfo>(AssetPaths.UnitAttackUpgradeInfoPath);
            _unitSpeedUpgradeInfo = _assetProvider.InstantiateScriptable<UpgradeInfo>(AssetPaths.UnitSpeedUpgradeInfoPath);
            _buildingDefenceInfo = _assetProvider.InstantiateScriptable<UpgradeInfo>(AssetPaths.BuildingDefenceUpgradeInfoPath);
            _buildingsMaxCounInfo = _assetProvider.InstantiateScriptable<UpgradeInfo>(AssetPaths.BuildingMaxCountUpgradeInfoPath);
        }

        private void LoadAllAIStats()
        {
            AIsAcid = LoadScriptable<Acid>(AssetPaths.AIAcidResourcePath);
            AIsBuff = LoadScriptable<Buff>(AssetPaths.AIBuffResourcePath);
            AIsCall = LoadScriptable<Call>(AssetPaths.AICallResourcePath);
            
            _buildingsInfo[AIIndex,AttackerIndex]= _assetProvider.InstantiateScriptable<Building>(AssetPaths.AIAttackerBuildingPath);
            _buildingsInfo[AIIndex,AltarIndex]   = _assetProvider.InstantiateScriptable<Building>(AssetPaths.AIAltarBuildingPath);
            _buildingsInfo[AIIndex,SpawnerIndex] = _assetProvider.InstantiateScriptable<Building>(AssetPaths.AISpawnerBuildingPath);

            _unitsInfo[AIIndex,AttackerIndex]= _assetProvider.InstantiateScriptable<Unit>(AssetPaths.AIAttackerUnitPath);
            _unitsInfo[AIIndex,AltarIndex]   = _assetProvider.InstantiateScriptable<Unit>(AssetPaths.AIAltarUnitPath);
            _unitsInfo[AIIndex,SpawnerIndex] = _assetProvider.InstantiateScriptable<Unit>(AssetPaths.AISpawnerUnitPath);

            _mana[AIIndex] = _assetProvider.InstantiateScriptable<Mana>(AssetPaths.AIManaPath);
        }
        
        private void LoadAllPlayerStats()
        {
            PlayersAcid = LoadScriptable<Acid>(AssetPaths.AcidResourcePath);
            PlayersBuff = LoadScriptable<Buff>(AssetPaths.BuffResourcePath);
            PlayersCall = LoadScriptable<Call>(AssetPaths.CallResourcePath);
            PlayersIce  = LoadScriptable<Ice>(AssetPaths.IceResourcePath);

            _buildingsInfo[PlayerIndex, AttackerIndex] = _assetProvider.InstantiateScriptable<Building>(AssetPaths.PlayerAttackerBuildingPath);
            _buildingsInfo[PlayerIndex, AltarIndex]    = _assetProvider.InstantiateScriptable<Building>(AssetPaths.PlayerAltarBuildingPath);
            _buildingsInfo[PlayerIndex, SpawnerIndex]  = _assetProvider.InstantiateScriptable<Building>(AssetPaths.PlayerSpawnerBuildingPath);

            _unitsInfo[PlayerIndex, AttackerIndex] = _assetProvider.InstantiateScriptable<Unit>(AssetPaths.PlayerAttackerUnitPath);
            _unitsInfo[PlayerIndex, AltarIndex]    = _assetProvider.InstantiateScriptable<Unit>(AssetPaths.PlayerAltarUnitPath);
            _unitsInfo[PlayerIndex, SpawnerIndex]  = _assetProvider.InstantiateScriptable<Unit>(AssetPaths.PlayerSpawnerUnitPath);

            _mana[PlayerIndex] = _assetProvider.InstantiateScriptable<Mana>(AssetPaths.PlayerScientificPath);
        }

        private void LoadRewards() => 
            RewardList = LoadScriptable<RewardList>(AssetPaths.RewardListPath);

        private T LoadScriptable<T>(string path) where T : ScriptableObject =>
            _assetProvider.InstantiateScriptable<T>(path);
    }
}