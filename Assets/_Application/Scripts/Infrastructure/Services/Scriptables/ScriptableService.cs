using System;
using _Application.Scripts.Infrastructure.Services.AssetManagement;
using _Application.Scripts.SavedData;
using _Application.Scripts.Scriptables;
using _Application.Scripts.Scriptables.Rewards;
using _Application.Scripts.Scriptables.Upgrades;
using UnityEngine;

namespace _Application.Scripts.Infrastructure.Services.Scriptables
{
    public class ScriptableService : IScriptableService
    {
        private readonly IAssetProvider _assetProvider;

        public RewardList RewardList { get; private set; }

        #region PlayerStats

        public Building PlayerAttackerBuilding { get; set; }
        public Building PlayerAltarBuilding { get; set; }
        public Building PlayerSpawnerBuilding { get; set; }

        public Unit PlayerAttackerUnit { get; set; }
        public Unit PlayerAltarUnit { get; set; }
        public Unit PlayerSpawnerUnit { get; set; }

        public Mana PlayerMana { get; set; }
        
        public Acid PlayersAcid { get; private set; }
        public Buff PlayersBuff { get; private set; }
        public Call PlayersCall { get; private set; }
        public Ice PlayersIce { get; private set; }
        
        #endregion
        

        #region AIStats

        public Building AIAttackerBuilding { get; set; }
        public Building AIAltarBuilding { set; get; }
        public Building AISpawnerBuilding { get; set; }
        
        public Unit AIAttackerUnit { get; set; }
        public Unit AIScientificUnit { get; set; }
        public Unit AISpawnerUnit { get; set; }

        public Mana AIMana { get; set; }

        public Acid AIsAcid { get; private set; }
        public Buff AIsBuff { get; private set; }
        public Call AIsCall { get; private set; }

        #endregion

        public UpgradeInfo RainUpgradeInfo { get; private set; }


        public ScriptableService(IAssetProvider assetProvider) => 
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
                UpgradeType.Rain => RainUpgradeInfo,
                _ => throw new ArgumentOutOfRangeException(nameof(upgradeType), upgradeType, null)
            };
        }

        private void LoadAllUpgradesInfo()
        {
            RainUpgradeInfo = _assetProvider.InstantiateScriptable<UpgradeInfo>(AssetPaths.RainUpgradesPath);
        }

        private void LoadAllAIStats()
        {
            AIsAcid = LoadScriptable<Acid>(AssetPaths.AIAcidResourcePath);
            AIsBuff = LoadScriptable<Buff>(AssetPaths.AIBuffResourcePath);
            AIsCall = LoadScriptable<Call>(AssetPaths.AICallResourcePath);
            
            AIAttackerBuilding = _assetProvider.InstantiateScriptable<Building>(AssetPaths.AIAttackerBuildingPath);
            AIAltarBuilding    = _assetProvider.InstantiateScriptable<Building>(AssetPaths.AIAltarBuildingPath);
            AISpawnerBuilding  = _assetProvider.InstantiateScriptable<Building>(AssetPaths.AISpawnerBuildingPath);

            AIAttackerUnit   = _assetProvider.InstantiateScriptable<Unit>(AssetPaths.AIAttackerUnitPath);
            AIScientificUnit = _assetProvider.InstantiateScriptable<Unit>(AssetPaths.AIAltarUnitPath);
            AISpawnerUnit    = _assetProvider.InstantiateScriptable<Unit>(AssetPaths.AISpawnerUnitPath);

            AIMana = _assetProvider.InstantiateScriptable<Mana>(AssetPaths.AIManaPath);
        }
        
        private void LoadAllPlayerStats()
        {
            PlayersAcid = LoadScriptable<Acid>(AssetPaths.AcidResourcePath);
            PlayersBuff = LoadScriptable<Buff>(AssetPaths.BuffResourcePath);
            PlayersCall = LoadScriptable<Call>(AssetPaths.CallResourcePath);
            PlayersIce  = LoadScriptable<Ice>(AssetPaths.IceResourcePath);

            PlayerAttackerBuilding = _assetProvider.InstantiateScriptable<Building>(AssetPaths.PlayerAttackerBuildingPath);
            PlayerAltarBuilding    = _assetProvider.InstantiateScriptable<Building>(AssetPaths.PlayerAltarBuildingPath);
            PlayerSpawnerBuilding  = _assetProvider.InstantiateScriptable<Building>(AssetPaths.PlayerSpawnerBuildingPath);

            PlayerAttackerUnit = _assetProvider.InstantiateScriptable<Unit>(AssetPaths.PlayerAttackerUnitPath);
            PlayerAltarUnit    = _assetProvider.InstantiateScriptable<Unit>(AssetPaths.PlayerAltarUnitPath);
            PlayerSpawnerUnit  = _assetProvider.InstantiateScriptable<Unit>(AssetPaths.PlayerSpawnerUnitPath);

            PlayerMana = _assetProvider.InstantiateScriptable<Mana>(AssetPaths.PlayerScientificPath);
        }

        private void LoadRewards() => 
            RewardList = LoadScriptable<RewardList>(AssetPaths.RewardListPath);

        private T LoadScriptable<T>(string path) where T : ScriptableObject =>
            _assetProvider.InstantiateScriptable<T>(path);
    }
}