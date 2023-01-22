using _Application.Scripts.Buildings;
using _Application.Scripts.Infrastructure.Services;
using _Application.Scripts.Infrastructure.Services.Factory;
using _Application.Scripts.Infrastructure.Services.Scriptables;
using _Application.Scripts.Managers;
using _Application.Scripts.Skills;
using Pool_And_Particles;
using UnityEngine;

namespace _Application.Scripts.AI
{
    public class AISkillController
    {
        public Call Call { get; }
        public Buff Buff { get; }
        public Acid Acid { get; }
        private readonly GlobalPool _objectPool;
        private readonly ScriptableService _scriptableService;

        public AISkillController(GlobalPool pool, ScriptableService scriptableService, GlobalPool objectPool,
            GlobalCamera globalCamera)
        {
            _scriptableService = scriptableService;
            _objectPool = objectPool;

            Call = new Call(pool,Team.Red, DecreaseAIManaCounter, globalCamera);
            Acid = new Acid(pool,Team.Red, DecreaseAIManaCounter);
            Buff = new Buff(pool,Team.Red, DecreaseAIManaCounter);

            ReloadSkills();
        }

        private void ReloadSkills()
        {
            Acid.Reload(_scriptableService.AIsAcid, 1.0f);
            Buff.Reload(_scriptableService.AIsBuff, 1.0f);
            Call.Reload(_scriptableService.AIsCall, 1.0f);
        }

        private static void DecreaseAIManaCounter(float value) => 
        	Core.ManaCount -= value;
        
        public void Refresh()
        {
            Call.Refresh(); 
            Buff.Refresh();
            Acid.Refresh();
        }
        
        public void AttackByAcid(BaseBuilding target) => 
            Acid.ExecuteForAI(target);

        public void BuffBuilding(BaseBuilding target) => 
            Buff.ExecuteForAI(target);

        public void CallSupply(BaseBuilding target) => 
            Call.ExecuteForAI(target);
    }   
}