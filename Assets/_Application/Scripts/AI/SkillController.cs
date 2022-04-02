using _Application.Scripts.Infrastructure.Services.AssetManagement;
using _Application.Scripts.Infrastructure.Services.Factory;
using _Application.Scripts.Managers;
using _Application.Scripts.Skills;
using UnityEngine;
using Base = _Application.Scripts.Buildings.Base;

namespace _Application.Scripts.AI
{
    public class SkillController
    {
        public Call Call { get; }
        public Buff Buff { get; }
        public Acid Acid { get; }
        
        private readonly ObjectPool _objectPool;

        public SkillController(IGameFactory gameFactory, ObjectPool objectPool)
        public SkillController(IGameFactory gameFactory, IScriptableService scriptableService, ObjectPool objectPool)
        {
            _scriptableService = scriptableService;
            _objectPool = objectPool;
            
            Call = new Call(Team.Red ,DecreaseAISciCounter);
            Call.NeedObjectFromPool += SpawnUnit;
            Call.SetSkillObject(gameFactory.CreateIndicator());

            Buff = new Buff(Team.Red ,DecreaseAISciCounter);

            Acid = new Acid(Team.Red ,DecreaseAISciCounter);
            Acid.SetSkillObject(gameFactory.CreateAcid());

            ReloadSkills();
        }

        private void ReloadSkills()
        {
            Acid.Reload(_scriptableService.AIsAcid, 1.0f);
            Buff.Reload(_scriptableService.AIsBuff, 1.0f);
            Call.Reload(_scriptableService.AIsCall, 1.0f);
        }

        private static void DecreaseAISciCounter(float value) => 
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

        private static void DecreaseAISciCounter(float value) => 
            Core.ScientificCount -= value;
    }   
}