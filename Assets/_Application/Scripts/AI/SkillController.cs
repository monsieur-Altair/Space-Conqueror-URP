using _Application.Scripts.Planets;
using _Application.Scripts.Skills;
using UnityEngine;
using Base = _Application.Scripts.Planets.Base;

namespace AI
{
    public class SkillController : MonoBehaviour
    {

        [SerializeField] private GameObject aiSkills;
     
        private Call _call;
        private Buff _buff;
        private Acid _acid;
        private Ice _ice;

        public void InitSkills()
        {
            _call = aiSkills.GetComponent<Call>();
            _call.SetTeamConstraint(Team.Red);
            _call.SetDecreasingFunction(DecreaseAISciCounter);

            _buff = aiSkills.GetComponent<Buff>();
            _buff.SetTeamConstraint(Team.Red);
            _buff.SetDecreasingFunction(DecreaseAISciCounter);

            _acid = aiSkills.GetComponent<Acid>();
            _acid.SetTeamConstraint(Team.Red);
            _acid.SetDecreasingFunction(DecreaseAISciCounter);

            _ice = aiSkills.GetComponent<Ice>();

            if (_acid == null || _buff == null || _call == null || _ice == null)
                throw new MyException("cannot get skill component, AI");
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