using _Application.Scripts.Infrastructure.AssetManagement;
using _Application.Scripts.Infrastructure.Factory;
using _Application.Scripts.Infrastructure.Services;
using _Application.Scripts.Planets;
using _Application.Scripts.Skills;
using UnityEngine;
using Base = _Application.Scripts.Planets.Base;

namespace AI
{
    public class SkillController : MonoBehaviour
    {
        private Call _call;
        private Buff _buff;
        private Acid _acid;
        private Ice _ice;

        public void InitSkills()
        {
            //_call = aiSkills.GetComponent<Call>();
            IGameFactory gameFactory = AllServices.Instance.GetSingle<IGameFactory>();
            
            _call = new Call();
            _call.Construct(gameFactory, gameFactory.CreateSkillResource(AssetPaths.AICallResourcePath));
            _call.SetTeamConstraint(Team.Red);
            _call.SetDecreasingFunction(DecreaseAISciCounter);

            _buff = new Buff();
            _buff.Construct(gameFactory, gameFactory.CreateSkillResource(AssetPaths.AIBuffResourcePath));
            _buff.SetTeamConstraint(Team.Red);
            _buff.SetDecreasingFunction(DecreaseAISciCounter);

            _acid = new Acid();
            _acid.Construct(gameFactory, gameFactory.CreateSkillResource(AssetPaths.AIAcidResourcePath));
            _acid.SetTeamConstraint(Team.Red);
            _acid.SetDecreasingFunction(DecreaseAISciCounter);

            _ice = new Ice();
            _ice.Construct(gameFactory, gameFactory.CreateSkillResource(AssetPaths.AIIceResourcePath));

        }

        private static void DecreaseAISciCounter(float value)
        {
            AI.Core.ScientificCount -= value;
        }

        public void AttackByAcid(Base target)
        {
            _acid.ExecuteForAI(target);
        }

        public void BuffPlanet(Base target)
        {
            _buff.ExecuteForAI(target);
        }
        
        
        public void Call(Base target)
        {
            _call.ExecuteForAI(target);
        }
    }   
}