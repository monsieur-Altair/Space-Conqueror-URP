using _Application.Scripts.Scriptables;
using _Application.Scripts.Scriptables.Rewards;

namespace _Application.Scripts.Infrastructure.Services.Scriptables
{
    public interface IScriptableService : IService
    {
        RewardList RewardList { get; }
        
        Planet PlayerAttackerPlanet { get; set; }
        Planet PlayerScientificPlanet { get; set; }
        Planet PlayerSpawnerPlanet { get; set; }
        Unit PlayerAttackerUnit { get; set; }
        Unit PlayerScientificUnit { get; set; }
        Unit PlayerSpawnerUnit { get; set; }
        Scientific PlayerScientific { get; set; }
        Acid PlayersAcid { get; }
        Buff PlayersBuff { get; }
        Call PlayersCall { get; }
        Ice PlayersIce { get; }
        
        Planet AIAttackerPlanet { get; set; }
        Planet AIScientificPlanet { set; get; }
        Planet AISpawnerPlanet { get; set; }
        Unit AIAttackerUnit { get; set; }
        Unit AIScientificUnit { get; set; }
        Unit AISpawnerUnit { get; set; }
        Scientific AIScientific { get; set; }
        Acid AIsAcid { get; }
        Buff AIsBuff { get; }
        Call AIsCall { get; }
        
        void LoadAllScriptables();
    }
}