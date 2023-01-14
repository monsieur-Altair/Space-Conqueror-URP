using _Application.Scripts.Buildings;
using _Application.Scripts.Infrastructure.Services.Factory;
using _Application.Scripts.Infrastructure.Services.Scriptables;
using _Application.Scripts.Managers;
using _Application.Scripts.Skills;
using UnityEngine;
using Base = _Application.Scripts.Buildings.Base;

namespace _Application.Scripts.AI
{
    public class AISkillController
    {
        public Call Call { get; }
        public Buff Buff { get; }
        public Acid Acid { get; }
        private readonly ObjectPool _objectPool;
        private readonly ScriptableService _scriptableService;

        public AISkillController(ObjectPool pool, ScriptableService scriptableService, ObjectPool objectPool)
        {
            _scriptableService = scriptableService;
            _objectPool = objectPool;

            Call = new Call(pool,Team.Red, DecreaseAIManaCounter);
            Acid = new Acid(pool,Team.Red, DecreaseAIManaCounter);
            Buff = new Buff(Team.Red, DecreaseAIManaCounter);

            Call.NeedObjectFromPool += SpawnUnit;

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
        
        public void AttackByAcid(Base target) => 
            Acid.ExecuteForAI(target);

        public void BuffBuilding(Base target) => 
            Buff.ExecuteForAI(target);

        public void CallSupply(Base target) => 
            Call.ExecuteForAI(target);

        private Units.Base SpawnUnit(PoolObjectType poolObjectType, Vector3 launchPos, Quaternion rotation) => 
            _objectPool.GetObject(poolObjectType, launchPos, rotation).GetComponent<Units.Base>();
        
    }   
}