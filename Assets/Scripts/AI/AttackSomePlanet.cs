using System;
using System.Collections;
using System.Collections.Generic;
using Planets;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AI
{
    public class AttackSomePlanet : MonoBehaviour, IAction
    {
        private List<List<Planets.Base>> _allPlanets;
        private Core _core;
        private Planets.Base _target;
        private readonly List<Planets.Base> _planetsForLaunch = new List<Planets.Base>();
        private Vector3 _mainPos;
        private int _countForLaunch;
        private const int MINCountForLaunch = 1;//inclusive
        private const int MAXCountForLaunch = 3;//inclusive
        private bool _isCastedSkill;
        private SkillController _skillController;
        
        
        private const float DelayAfterCasting = 2.0f;
        
        [SerializeField]private int buffProbability = 40;
        [SerializeField]private int acidProbability = 40;
        [SerializeField]private int callProbability = 40;

        
        public void InitAction()
        {
            _core = Core.Instance;
            _allPlanets = _core.AllPlanets;
            _mainPos = _core.MainPos;
            _isCastedSkill = false;
            
            
            _skillController = GetComponent<SkillController>();
            if (_skillController==null)
            {
                throw new MyException("skill controller = null");
            }
            _skillController.InitSkills();
            
            
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
            //Debug.Log("count for launch = "+_planetsForLaunch.Count);
            foreach (var planet in _planetsForLaunch)
            {
                //Debug.Log("launch");
                if((int)planet.Team==Core.Own)
                    planet.LaunchUnit(_target);
                yield return null;
            }

        }

        private int FindTargetTeam()
        {
            var enemyCount = _allPlanets[Core.Enemy].Count;
            var neutralCount = _allPlanets[Core.Neutral].Count;
            var hundredPercent = enemyCount + neutralCount;
            var randomType = Random.Range(1, hundredPercent + 1);
            return (randomType <= enemyCount) ? Core.Enemy : Core.Neutral;
        }

        private bool ApplyDecision(float probability)
        {
            //Debug.Log("probab="+probability);
            var result = Random.Range(1, 101);
            return result <= probability;
        }
        
        private IEnumerator FindTarget()
        {
            var teamForAttack = FindTargetTeam();
            //Debug.Log("team="+teamForAttack);
            yield return null;
            
            var minDistance = float.PositiveInfinity;

            foreach (var possibleTarget in _allPlanets[teamForAttack])
            {
                var distance = Vector3.Distance(_mainPos, possibleTarget.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    _target = possibleTarget;
                }

                yield return null;
            }

            TryToAcid();

            //Debug.Log("target="+_target.Team+_target.ID);
            yield return null;
        }

        private void TryToAcid()
        {
            if (!_isCastedSkill)
            {
                var isAttackedByAcid = ApplyDecision(acidProbability);
                if (isAttackedByAcid)
                {
                    _skillController.AttackByAcid(_target);
                    _isCastedSkill = true;
                } 
            }
        }
        
        private void TryToCall()
        {
            if (!_isCastedSkill)
            {
                var isCalled = ApplyDecision(callProbability);
                if (isCalled)
                {
                    var randomIndex = Random.Range(0, _allPlanets[Core.Own].Count);
                    _target = _allPlanets[Core.Own][randomIndex];
                    _skillController.Call(_target);
                    _isCastedSkill = true;
                } 
            } 
        }

        private void TryToBuff()
        {
            if (!_isCastedSkill)
            {
                var isBuffedRandomPlanet = ApplyDecision(buffProbability);
                if (isBuffedRandomPlanet)
                {
                    var randomIndex = Random.Range(0, _planetsForLaunch.Count);
                    _skillController.BuffPlanet(_planetsForLaunch[randomIndex]);
                    _isCastedSkill = true;
                }
            }
        }
        
        private IEnumerator FindAllPlanetsToAttack()
        {
            yield return StartCoroutine(FindTarget());

            
            //Debug.Log("try to find planet to launch");
            var launchPos = _target.transform.position;
            List<KeyValuePair<Planets.Base, float>> dataForLaunch = new List<KeyValuePair<Planets.Base, float>>();
            foreach (var planetForLaunch in _allPlanets[Core.Own])
            {
                var distance = Vector3.Distance(launchPos, planetForLaunch.transform.position);
                dataForLaunch.Add(new KeyValuePair<Planets.Base, float>(planetForLaunch,distance));
                yield return null;
            }

            var allPossibleLaunchCount = dataForLaunch.Count;
            //sort list by ascending
            if (allPossibleLaunchCount >= 2)
            {
                dataForLaunch.Sort((pair1,pair2)=>pair1.Value.CompareTo(pair2.Value));
                yield return null;
            }
            
            
            //Debug.Log("all poss count "+allPossibleLaunchCount);
            if (_countForLaunch > allPossibleLaunchCount)
                _countForLaunch = allPossibleLaunchCount;
            for (var i = 0; i < _countForLaunch; i++)
            {
                _planetsForLaunch.Add(dataForLaunch[i].Key);
                yield return null;
            }

            TryToBuff();

            if (_isCastedSkill)
            {
                yield return new WaitForSeconds(DelayAfterCasting);
            }
            //yield return StartCoroutine(FindAllPlanetsToLaunch());
        }

    }
}