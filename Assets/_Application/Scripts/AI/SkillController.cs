using _Application.Scripts.Infrastructure.Services.AssetManagement;
using _Application.Scripts.Infrastructure.Services.Factory;
using _Application.Scripts.Managers;
using _Application.Scripts.Planets;
using _Application.Scripts.Skills;
using UnityEngine;
using Base = _Application.Scripts.Planets.Base;

namespace _Application.Scripts.AI
{
    public class SkillController
    {
        public Call Call { get; }
        public Buff Buff { get; }
        public Acid Acid { get; }
        
        private readonly ObjectPool _objectPool;

        public SkillController(IGameFactory gameFactory, ObjectPool objectPool)
        {
            _objectPool = objectPool;
            
            Call = new Call();
            Call.NeedObjectFromPool += SpawnUnit;
            Call.Construct(gameFactory, gameFactory.CreateSkillResource(AssetPaths.AICallResourcePath));
            Call.SetTeamConstraint(Team.Red);
            Call.SetDecreasingFunction(DecreaseAISciCounter);

            Buff = new Buff();
            Buff.Construct(gameFactory, gameFactory.CreateSkillResource(AssetPaths.AIBuffResourcePath));
            Buff.SetTeamConstraint(Team.Red);
            Buff.SetDecreasingFunction(DecreaseAISciCounter);

            Acid = new Acid();
            Acid.Construct(gameFactory, gameFactory.CreateSkillResource(AssetPaths.AIAcidResourcePath));
            Acid.SetTeamConstraint(Team.Red);
            Acid.SetDecreasingFunction(DecreaseAISciCounter);
        }
        
        private Units.Base SpawnUnit(PoolObjectType poolObjectType, Vector3 launchPos, Quaternion rotation) => 
            _objectPool.GetObject(poolObjectType, launchPos, rotation).GetComponent<Units.Base>();

        private static void DecreaseAISciCounter(float value) => 
            Core.ScientificCount -= value;

        public void AttackByAcid(Base target) => 
            Acid.ExecuteForAI(target);

        public void BuffPlanet(Base target) => 
            Buff.ExecuteForAI(target);

        public void CallSupply(Base target) => 
            Call.ExecuteForAI(target);
    }   
}