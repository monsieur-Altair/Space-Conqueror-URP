using _Application.Scripts.Buildings;
using _Application.Scripts.SavedData;
using _Application.Scripts.Scriptables;
using _Application.Scripts.Scriptables.Rewards;
using _Application.Scripts.Scriptables.Upgrades;

namespace _Application.Scripts.Infrastructure.Services.Scriptables
{
    public interface IScriptableService : IService
    {
        RewardList RewardList { get; }
        
        Acid PlayersAcid { get; }
        Buff PlayersBuff { get; }
        Call PlayersCall { get; }
        Ice PlayersIce { get; }
      
        Acid AIsAcid { get; }
        Buff AIsBuff { get; }
        Call AIsCall { get; }
        
        //UpgradeInfo RainUpgradeInfo { get; }
        
        void LoadAllScriptables();
        UpgradeInfo GetUpgradeInfo(UpgradeType upgradeType);
        Building GetBuildingInfo(Team team, Type type);
        Unit GetUnitInfo(Team team, Type type);
        Mana GetManaInfo(Team team);
    }
}