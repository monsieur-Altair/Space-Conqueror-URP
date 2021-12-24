using UnityEngine;

namespace AI
{
    public class SkillController : MonoBehaviour
    {
        
        /*private List<List<Planets.Base>> _allPlanets;
        private Core _core;
        private Planets.Base _target;
        private Vector3 _mainPos;*/

        [SerializeField] private GameObject aiSkills;
        private Skills.Call _call;
        private Skills.Buff _buff;
        private Skills.Acid _acid;
        private Skills.Ice _ice;

        public void InitSkills()
        {
            /*_core = Core.Instance;
            _allPlanets = _core.AllPlanets;
            _mainPos = _core.MainPos;*/
            
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

        private void DecreaseAISciCounter(float value)
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
        
        
        /*private IEnumerator FindTargetForLaunch()
        {
            var minDistance = float.PositiveInfinity;

            foreach (var possibleTarget in _allPlanets[Core.Neutral])
            {
                var distance = Vector3.Distance(_mainPos, possibleTarget.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    _target = possibleTarget;
                }

                yield return null;
            }
        }*/
        
        /*private IEnumerator Cast()
        {
            yield return StartCoroutine(FindTargetForLaunch());

            Skills.ISkill skillForCast = FindSkill();
            
            skillForCast.ExecuteForAI(_target);
        }*/

        /*private Skills.ISkill FindSkill()//without ice, because Ai will cast it only to planet
        {
            return _acid;
            //List<ISkill> possibleSkills = new List<ISkill>();
            var acidCost = 1.0f / _acid.Cost;
            var buffCost = 1.0f / _buff.Cost;
            var callCost = 1.0f / _call.Cost;
            var iceCost  = 1.0f / _ice.Cost;

            var hundredPercent = acidCost + buffCost + callCost;// + iceCost;
            var result = Random.Range(0, hundredPercent);
            
            if (result <= acidCost)
                return _acid;
            result -= acidCost;
            if (result <= buffCost)
                return _buff;
            result -= buffCost;
            if (result <= callCost)
                return _call;
            //result -= callCost;
            return null;
        }*/
    }   
}