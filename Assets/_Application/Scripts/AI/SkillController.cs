using UnityEngine;

namespace AI
{
    public class SkillController : MonoBehaviour
    {

        [SerializeField] private GameObject aiSkills;
     
        private Skills.Call _call;
        private Skills.Buff _buff;
        private Skills.Acid _acid;
        private Skills.Ice _ice;

        public void InitSkills()
        {
            _call = aiSkills.GetComponent<Skills.Call>();
            _call.SetTeamConstraint(Planets.Team.Red);
            _call.SetDecreasingFunction(DecreaseAISciCounter);

            _buff = aiSkills.GetComponent<Skills.Buff>();
            _buff.SetTeamConstraint(Planets.Team.Red);
            _buff.SetDecreasingFunction(DecreaseAISciCounter);

            _acid = aiSkills.GetComponent<Skills.Acid>();
            _acid.SetTeamConstraint(Planets.Team.Red);
            _acid.SetDecreasingFunction(DecreaseAISciCounter);

            _ice = aiSkills.GetComponent<Skills.Ice>();

            if (_acid == null || _buff == null || _call == null || _ice == null)
                throw new MyException("cannot get skill component, AI");
        }

        private static void DecreaseAISciCounter(float value)
        {
            AI.Core.ScientificCount -= value;
        }

        public void AttackByAcid(Planets.Base target)
        {
            _acid.ExecuteForAI(target);
        }

        public void BuffPlanet(Planets.Base target)
        {
            _buff.ExecuteForAI(target);
        }
        
        
        public void Call(Planets.Base target)
        {
            _call.ExecuteForAI(target);
        }
    }   
}