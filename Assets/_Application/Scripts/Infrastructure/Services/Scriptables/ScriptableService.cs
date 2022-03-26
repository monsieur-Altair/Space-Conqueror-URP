using _Application.Scripts.Infrastructure.Services.AssetManagement;
using _Application.Scripts.Scriptables;
using _Application.Scripts.Scriptables.Rewards;
using UnityEngine;

namespace _Application.Scripts.Infrastructure.Services.Scriptables
{
    public class ScriptableService : IScriptableService
    {
        private readonly IAssetProvider _assetProvider;

        public RewardList RewardList { get; private set; }

        #region PlayerStats

        public Planet PlayerAttackerPlanet { get; set; }
        public Planet PlayerScientificPlanet { get; set; }
        public Planet PlayerSpawnerPlanet { get; set; }

        public Unit PlayerAttackerUnit { get; set; }
        public Unit PlayerScientificUnit { get; set; }
        public Unit PlayerSpawnerUnit { get; set; }

        public Scientific PlayerScientific { get; set; }
        
        public Acid PlayersAcid { get; private set; }
        public Buff PlayersBuff { get; private set; }
        public Call PlayersCall { get; private set; }
        public Ice PlayersIce { get; private set; }
        
        #endregion
        

        #region AIStats

        public Planet AIAttackerPlanet { get; set; }
        public Planet AIScientificPlanet { set; get; }
        public Planet AISpawnerPlanet { get; set; }
        
        public Unit AIAttackerUnit { get; set; }
        public Unit AIScientificUnit { get; set; }
        public Unit AISpawnerUnit { get; set; }

        public Scientific AIScientific { get; set; }

        public Acid AIsAcid { get; private set; }
        public Buff AIsBuff { get; private set; }
        public Call AIsCall { get; private set; }
        
        

        #endregion
        

        public ScriptableService(IAssetProvider assetProvider) => 
            _assetProvider = assetProvider;

        public void LoadAllScriptables()
        {
            LoadRewards();
            LoadAllPlayerStats();
            LoadAllAIStats();
        }

        private void LoadAllAIStats()
        {
            AIsAcid = LoadScriptable<Acid>(AssetPaths.AIAcidResourcePath);
            AIsBuff = LoadScriptable<Buff>(AssetPaths.AIBuffResourcePath);
            AIsCall = LoadScriptable<Call>(AssetPaths.AICallResourcePath);
            
            AIAttackerPlanet   = _assetProvider.InstantiateScriptable<Planet>(AssetPaths.AIAttackerPlanetPath);
            AIScientificPlanet = _assetProvider.InstantiateScriptable<Planet>(AssetPaths.AIScientificPlanetPath);
            AISpawnerPlanet    = _assetProvider.InstantiateScriptable<Planet>(AssetPaths.AISpawnerPlanetPath);

            AIAttackerUnit   = _assetProvider.InstantiateScriptable<Unit>(AssetPaths.AIAttackerUnitPath);
            AIScientificUnit = _assetProvider.InstantiateScriptable<Unit>(AssetPaths.AIScientificUnitPath);
            AISpawnerUnit    = _assetProvider.InstantiateScriptable<Unit>(AssetPaths.AISpawnerUnitPath);

            AIScientific = _assetProvider.InstantiateScriptable<Scientific>(AssetPaths.AIScientificPath);
        }
        
        private void LoadAllPlayerStats()
        {
            PlayersAcid = LoadScriptable<Acid>(AssetPaths.AcidResourcePath);
            PlayersBuff = LoadScriptable<Buff>(AssetPaths.BuffResourcePath);
            PlayersCall = LoadScriptable<Call>(AssetPaths.CallResourcePath);
            PlayersIce  = LoadScriptable<Ice>(AssetPaths.IceResourcePath);

            PlayerAttackerPlanet   = _assetProvider.InstantiateScriptable<Planet>(AssetPaths.PlayerAttackerPlanetPath);
            PlayerScientificPlanet = _assetProvider.InstantiateScriptable<Planet>(AssetPaths.PlayerScientificPlanetPath);
            PlayerSpawnerPlanet    = _assetProvider.InstantiateScriptable<Planet>(AssetPaths.PlayerSpawnerPlanetPath);

            PlayerAttackerUnit   = _assetProvider.InstantiateScriptable<Unit>(AssetPaths.PlayerAttackerUnitPath);
            PlayerScientificUnit = _assetProvider.InstantiateScriptable<Unit>(AssetPaths.PlayerScientificUnitPath);
            PlayerSpawnerUnit    = _assetProvider.InstantiateScriptable<Unit>(AssetPaths.PlayerSpawnerUnitPath);

            PlayerScientific = _assetProvider.InstantiateScriptable<Scientific>(AssetPaths.PlayerScientificPath);
        }

        private void LoadRewards() => 
            RewardList = LoadScriptable<RewardList>(AssetPaths.RewardListPath);

        private T LoadScriptable<T>(string path) where T : ScriptableObject =>
            _assetProvider.InstantiateScriptable<T>(path);
    }
}