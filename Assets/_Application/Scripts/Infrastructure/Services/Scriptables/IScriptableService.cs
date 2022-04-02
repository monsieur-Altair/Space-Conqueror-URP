using _Application.Scripts.SavedData;
using _Application.Scripts.Scriptables;
using _Application.Scripts.Scriptables.Rewards;
using _Application.Scripts.Scriptables.Upgrades;

namespace _Application.Scripts.Infrastructure.Services.Scriptables
{
    public interface IScriptableService : IService
    {
        RewardList RewardList { get; }

        Building PlayerSpawnerBuilding { get; set; }
        Building PlayerAttackerBuilding { get; set; }
        Building PlayerAltarBuilding { get; set; }
        Unit PlayerAttackerUnit { get; set; }
        Unit PlayerAltarUnit { get; set; }
        Unit PlayerSpawnerUnit { get; set; }
        Mana PlayerMana { get; set; }
        Acid PlayersAcid { get; }
        Buff PlayersBuff { get; }
        Call PlayersCall { get; }
        Ice PlayersIce { get; }
        
        Building AIAttackerBuilding { get; set; }
        Building AIAltarBuilding { set; get; }
        Building AISpawnerBuilding { get; set; }
        Unit AIAttackerUnit { get; set; }
        Unit AIScientificUnit { get; set; }
        Unit AISpawnerUnit { get; set; }
        Mana AIMana { get; set; }
        Acid AIsAcid { get; }
        Buff AIsBuff { get; }
        Call AIsCall { get; }
        
        UpgradeInfo RainUpgradeInfo { get; }
        
        void LoadAllScriptables();
        UpgradeInfo GetUpgradeInfo(UpgradeType upgradeType);
    }
}