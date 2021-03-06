namespace _Application.Scripts.Infrastructure.Services.AssetManagement
{
    public static class AssetPaths
    {
        public const string PoolPath = "Managers/Object Pool";
        public const string UserControlPath = "Managers/User Control";
        public const string Warehouse = "Managers/Warehouse";
        
        
        public const string UISystemPath = "UI/UISystem";
        
        public const string AcidPrefabPath = "Skills/Acid rain";
        public const string IndicatorPrefabPath = "Skills/Indicator";
        public const string IcePrefabPath = "Skills/Ice";

        public const string RewardListPath = "Scriptables/Rewards list";


        #region AIsPaths

        public const string AIAttackerBuildingPath = "Scriptables/AI/AI Attacker Building";
        public const string AIAltarBuildingPath    = "Scriptables/AI/AI Altar Building";
        public const string AISpawnerBuildingPath  = "Scriptables/AI/AI Spawner Building";

        public const string AIAttackerUnitPath = "Scriptables/AI/AI Attacker Warrior";
        public const string AIAltarUnitPath    = "Scriptables/AI/AI Altar Warrior";
        public const string AISpawnerUnitPath  = "Scriptables/AI/AI Spawner Warrior";

        public const string AIManaPath = "Scriptables/AI/AI Mana";

        public const string AICallResourcePath = "Scriptables/AI/AI Call";
        public const string AIBuffResourcePath = "Scriptables/AI/AI Buff";
        public const string AIAcidResourcePath = "Scriptables/AI/AI Acid";

        #endregion

        #region PlayersPaths

        public const string PlayerAttackerBuildingPath = "Scriptables/Player/Attacker Building";
        public const string PlayerAltarBuildingPath    = "Scriptables/Player/Altar Building";
        public const string PlayerSpawnerBuildingPath  = "Scriptables/Player/Spawner Building";

        public const string PlayerAttackerUnitPath = "Scriptables/Player/Attacker Warrior";
        public const string PlayerAltarUnitPath    = "Scriptables/Player/Altar Warrior";
        public const string PlayerSpawnerUnitPath  = "Scriptables/Player/Spawner Warrior";

        public const string PlayerScientificPath = "Scriptables/Player/Mana";

        public const string CallResourcePath = "Scriptables/Player/Call";
        public const string BuffResourcePath = "Scriptables/Player/Buff";
        public const string AcidResourcePath = "Scriptables/Player/Acid";
        public const string IceResourcePath  = "Scriptables/Player/Ice";

        #endregion

        #region Upgrades
        
        public const string RainUpgradeInfoPath = "Scriptables/Upgrades/Rain info";
        public const string UnitSpeedUpgradeInfoPath = "Scriptables/Upgrades/Unit speed info";
        public const string UnitAttackUpgradeInfoPath = "Scriptables/Upgrades/Unit attack info";
        public const string BuildingDefenceUpgradeInfoPath = "Scriptables/Upgrades/Building defence info";
        public const string BuildingMaxCountUpgradeInfoPath = "Scriptables/Upgrades/Building max count info";

        #endregion

        public const string GlobalObjectPath = "GlobalObject";
        public const string MainCameraPath = "Main Camera";
    }
}