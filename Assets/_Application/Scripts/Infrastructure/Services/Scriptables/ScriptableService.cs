using _Application.Scripts.Buildings;
using _Application.Scripts.Control;
using _Application.Scripts.Managers;
using _Application.Scripts.SavedData;
using _Application.Scripts.Scriptables;
using _Application.Scripts.Scriptables.Rewards;
using _Application.Scripts.Scriptables.Upgrades;

namespace _Application.Scripts.Infrastructure.Services.Scriptables
{
    public class ScriptableService : IService
    {
        private const int AIIndex = (int)Team.Red;
        private const int PlayerIndex = (int)Team.Blue;
        
        private const int AttackerIndex = (int)BuildingType.Attacker;
        private const int AltarIndex = (int)BuildingType.Altar;
        private const int SpawnerIndex = (int)BuildingType.Spawner;
        
        private readonly Building[,] _buildingsInfo = new Building[2,3];
        private readonly Unit[,] _unitsInfo = new Unit[2,3];
        private readonly Mana[] _mana = new Mana[2];
        private readonly CoreConfig _coreConfig;

        public RewardList RewardList { get; private set; }

        #region PlayerStats

        public Skill PlayersAcid => _coreConfig.PlayerConfig.Skills.GetValue(SkillName.Acid);
        public Skill PlayersBuff => _coreConfig.PlayerConfig.Skills.GetValue(SkillName.Buff);
        public Skill PlayersCall => _coreConfig.PlayerConfig.Skills.GetValue(SkillName.Call);
        public Skill PlayersIce  => _coreConfig.PlayerConfig.Skills.GetValue(SkillName.Ice);

        #endregion
        
        #region AIStats

        public Skill AIsAcid => _coreConfig.AIConfig.Skills.GetValue(SkillName.Acid);
        public Skill AIsBuff => _coreConfig.AIConfig.Skills.GetValue(SkillName.Buff);
        public Skill AIsCall => _coreConfig.AIConfig.Skills.GetValue(SkillName.Call);

        #endregion
        
        public ScriptableService(CoreConfig coreConfig)
        {
            _coreConfig = coreConfig;
        }

        public void LoadAllScriptables()
        {
            RewardList = _coreConfig.RewardList;
            
            LoadAllPlayerStats(PlayerIndex, _coreConfig.PlayerConfig);
            LoadAllPlayerStats(AIIndex, _coreConfig.AIConfig);
        }

        public UpgradeInfo GetUpgradeInfo(UpgradeType upgradeType) => 
            _coreConfig.Upgrades.GetValue(upgradeType);

        public Building GetBuildingInfo(Team team, BuildingType buildingType) => 
            _buildingsInfo[(int) team, (int) buildingType];

        public Unit GetUnitInfo(Team team, BuildingType buildingType) => 
            _unitsInfo[(int) team, (int) buildingType];

        public Mana GetManaInfo(Team team) => 
            _mana[(int) team];

        private void LoadAllPlayerStats(int playerIndex, PlayerConfig config)
        {
            MyDictionary<BuildingType,Building> buildings = config.Buildings;
            
            _buildingsInfo[playerIndex, AttackerIndex] = buildings.GetValue(BuildingType.Attacker);
            _buildingsInfo[playerIndex, AltarIndex] = buildings.GetValue(BuildingType.Altar);
            _buildingsInfo[playerIndex, SpawnerIndex] = buildings.GetValue(BuildingType.Spawner);

            MyDictionary<BuildingType, Unit> units = config.Units;
            
            _unitsInfo[playerIndex, AttackerIndex] = units.GetValue(BuildingType.Attacker);
            _unitsInfo[playerIndex, AltarIndex]    = units.GetValue(BuildingType.Altar);
            _unitsInfo[playerIndex, SpawnerIndex]  = units.GetValue(BuildingType.Spawner);

            _mana[playerIndex] = _coreConfig.PlayerConfig.ManaConfig;
        }
    }
}