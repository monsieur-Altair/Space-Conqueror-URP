using System.Collections;
using System.Collections.Generic;
using _Application.Scripts.Planets;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AI
{
    public class AttackSomePlanet : MonoBehaviour, IAction
    {
        [SerializeField]private int buffProbability = 40;
        [SerializeField]private int acidProbability = 40;
        [SerializeField]private int callProbability = 40;
        
        
        private List<List<Base>> _allPlanets;
        private Core _core;
        private Base _target;
        private readonly List<Base> _planetsForLaunch = new List<Base>();
        private Vector3 _mainPos;
        private int _countForLaunch;
        private const int MINCountForLaunch = 1;//inclusive
        private const int MAXCountForLaunch = 3;//inclusive
        private bool _isCastedSkill;
        private SkillController _skillController;
        private const float DelayAfterCasting = 2.0f;


        public void InitAction()
        {
            if (_core==null)
            {
                _core = Core.Instance;
            }
            _allPlanets = _core.AllPlanets;
            _mainPos = _core.MainPos;
            _isCastedSkill = false;

            if (_skillController==null)
            {
                _skillController = GetComponent<SkillController>();
                if (_skillController==null)
                {
                    throw new MyException("skill controller = null");
                }
                _skillController.InitSkills();
            }
            
        }

        public void Execute()
        {
            _planetsForLaunch.Clear();
            _countForLaunch = Random.Range(MINCountForLaunch, MAXCountForLaunch+1);
            
            TryToCall();
            if(!_isCastedSkill)
                StartCoroutine(LaunchToTarget());
            
            _isCastedSkill = false;

        }

       

        private IEnumerator LaunchToTarget()
        {
            yield return StartCoroutine(FindAllPlanetsToAttack());
            foreach (Base planet in _planetsForLaunch)
            {
                if((int)planet.Team==Core.Own)
                    planet.LaunchUnit(_target);
                yield return null;
            }

        }

        private int FindTargetTeam()
        {
            int enemyCount = _allPlanets[Core.Enemy].Count;
            int neutralCount = _allPlanets[Core.Neutral].Count;
            int hundredPercent = enemyCount + neutralCount;
            int randomType = Random.Range(1, hundredPercent + 1);
            
            if (enemyCount <= 2 && neutralCount != 0)
                return Core.Neutral;
            
            return (randomType <= enemyCount) ? Core.Enemy : Core.Neutral;
        }

        private bool ApplyDecision(float probability)
        {
            int result = Random.Range(1, 101);
            return result <= probability;
        }
        
        private IEnumerator FindTarget()
        {
            int teamForAttack = FindTargetTeam();
            yield return null;
            
            float minDistance = float.PositiveInfinity;

            foreach (Base possibleTarget in _allPlanets[teamForAttack])
            {
                float distance = Vector3.Distance(_mainPos, possibleTarget.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    _target = possibleTarget;
                }

                yield return null;
            }

            TryToAcid();

            yield return null;
        }

        private void TryToAcid()
        {
            if (_isCastedSkill) 
                return;
            
            bool isApplied = ApplyDecision(acidProbability);

            if (isApplied)
            {
                _skillController.AttackByAcid(_target);
                _isCastedSkill = true;
            }
        }
        
        private void TryToCall()
        {
            if (_isCastedSkill) 
                return;
            
            bool isApplied = ApplyDecision(callProbability);
            if (isApplied)
            {
                var randomIndex = Random.Range(0, _allPlanets[Core.Own].Count);
                _target = _allPlanets[Core.Own][randomIndex];
                _skillController.Call(_target);
                _isCastedSkill = true;
            }
        }

        private void TryToBuff()
        {
            if (_isCastedSkill)
                return;
            
            bool isApplied = ApplyDecision(buffProbability);
            if (isApplied)
            {
                int randomIndex = Random.Range(0, _planetsForLaunch.Count);
                _skillController.BuffPlanet(_planetsForLaunch[randomIndex]);
                _isCastedSkill = true;
            }
        }
        
        private IEnumerator FindAllPlanetsToAttack()
        {
            yield return StartCoroutine(FindTarget());
            
            Vector3 launchPos = _target.transform.position;
            List<KeyValuePair<Base, float>> dataForLaunch = new List<KeyValuePair<Base, float>>();
            foreach (Base planetForLaunch in _allPlanets[Core.Own])
            {
                float distance = Vector3.Distance(launchPos, planetForLaunch.transform.position);
                dataForLaunch.Add(new KeyValuePair<Base, float>(planetForLaunch,distance));
                yield return null;
            }

            int allPossibleLaunchCount = dataForLaunch.Count;
            
            //sort list by ascending
            if (allPossibleLaunchCount >= 2)
            {
                dataForLaunch.Sort((pair1,pair2)=>pair1.Value.CompareTo(pair2.Value));
                yield return null;
            }
            
            
            if (_countForLaunch > allPossibleLaunchCount)
                _countForLaunch = allPossibleLaunchCount;
            for (int i = 0; i < _countForLaunch; i++)
            {
                _planetsForLaunch.Add(dataForLaunch[i].Key);
                yield return null;
            }

            TryToBuff();

            if (_isCastedSkill)
            {
                yield return new WaitForSeconds(DelayAfterCasting);
            }
        }

    }
}