using System.Collections;
using System.Collections.Generic;
using _Application.Scripts.Infrastructure;
using _Application.Scripts.Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Application.Scripts.AI
{
    public class AttackSomeBuilding : IAction
    {
        private const int BuffProbability = 100;
        private const int AcidProbability = 40;
        private const int CallProbability = 40;
        
        private const int MinCountForLaunch = 1;//inclusive
        private const int MaxCountForLaunch = 3;//inclusive
        private const float DelayAfterCasting = 2.0f;

        private readonly CoroutineRunner _coroutineRunner;
        private readonly AISkillController _aiSkillController;

        private List<List<Buildings.BaseBuilding>> _allBuildings;
        private Buildings.BaseBuilding _target;
        private readonly List<Buildings.BaseBuilding> _buildingsForLaunch = new List<Buildings.BaseBuilding>();
        private Vector3 _mainPos;
        private int _countForLaunch;
        private bool _isCastedSkill;


        public AttackSomeBuilding(CoroutineRunner coroutineRunner, AISkillController aiSkillController)
        {
            _aiSkillController = aiSkillController;
            _coroutineRunner = coroutineRunner;
        }
        
        public void InitAction(List<List<Buildings.BaseBuilding>> buildings, Vector3 mainPos)
        {
            _allBuildings = buildings;
            _mainPos = mainPos;
            _isCastedSkill = false;
        }

        public void Execute()
        {
            _buildingsForLaunch.Clear();
            _countForLaunch = Random.Range(MinCountForLaunch, MaxCountForLaunch+1);
            
            TryToCall();
            if(!_isCastedSkill)
                _coroutineRunner.StartCoroutine(LaunchToTarget());
            
            _isCastedSkill = false;

        }

        private IEnumerator LaunchToTarget()
        {
            yield return _coroutineRunner.StartCoroutine(FindAllBuildingsToAttack());
            foreach (Buildings.BaseBuilding building in _buildingsForLaunch)
            {
                if((int)building.Team==Core.Own)
                    building.LaunchUnit(_target);
                yield return null;
            }

        }

        private IEnumerator FindAllBuildingsToAttack()
        {
            yield return _coroutineRunner.StartCoroutine(FindTarget());
            
            TryToAcid();
            yield return null;
            
            Vector3 launchPos = _target.transform.position;
            List<KeyValuePair<Buildings.BaseBuilding, float>> dataForLaunch = new List<KeyValuePair<Buildings.BaseBuilding, float>>();
            foreach (Buildings.BaseBuilding buildingForLaunch in _allBuildings[Core.Own])
            {
                float distance = Vector3.Distance(launchPos, buildingForLaunch.transform.position);
                dataForLaunch.Add(new KeyValuePair<Buildings.BaseBuilding, float>(buildingForLaunch,distance));
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
                _buildingsForLaunch.Add(dataForLaunch[i].Key);
                yield return null;
            }

            TryToBuff();

            if (_isCastedSkill)
                yield return new WaitForSeconds(DelayAfterCasting);
        }

        private IEnumerator FindTarget()
        {
            int teamForAttack = FindTargetTeam();
            yield return null;
            
            float minDistance = float.PositiveInfinity;

            foreach (Buildings.BaseBuilding possibleTarget in _allBuildings[teamForAttack])
            {
                float distance = Vector3.Distance(_mainPos, possibleTarget.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    _target = possibleTarget;
                }

                yield return null;
            }
        }

        private int FindTargetTeam()
        {
            int enemyCount = _allBuildings[Core.Enemy].Count;
            int neutralCount = _allBuildings[Core.Neutral].Count;
            int hundredPercent = enemyCount + neutralCount;
            int randomType = Random.Range(1, hundredPercent + 1);
            
            if (enemyCount <= 2 && neutralCount != 0)
                return Core.Neutral;
            
            return (randomType <= enemyCount) ? Core.Enemy : Core.Neutral;
        }

        private void TryToAcid()
        {
            //Debug.Log("Try to acid");
            if (_isCastedSkill) 
                return;
            
            bool isApplied = ApplyDecision(AcidProbability, _aiSkillController.Acid.Cost);
           // Debug.Log($"Is acid applied? {isApplied}");

            if (isApplied)
            {
                _aiSkillController.AttackByAcid(_target);
                _isCastedSkill = true;
            }
        }

        private void TryToCall()
        {
            //Debug.Log("Try to call");
            if (_isCastedSkill) 
                return;

            bool isApplied = ApplyDecision(CallProbability, _aiSkillController.Call.Cost);
            //Debug.Log($"Is call applied? {isApplied}");
            if (isApplied)
            {
                int randomIndex = Random.Range(0, _allBuildings[Core.Own].Count);
                _target = _allBuildings[Core.Own][randomIndex];
                _aiSkillController.CallSupply(_target);
                _isCastedSkill = true;
            }
        }

        private void TryToBuff()
        {
           // Debug.Log("Try to buff");
            if (_isCastedSkill)
                return;

            bool isApplied = ApplyDecision(BuffProbability, _aiSkillController.Buff.Cost);
          //  Debug.Log($"Is buff applied? {isApplied}");

            if (isApplied)
            {
                int randomIndex = Random.Range(0, _buildingsForLaunch.Count);
                _aiSkillController.BuffBuilding(_buildingsForLaunch[randomIndex]);
                _isCastedSkill = true;
            }
        }

        private static bool ApplyDecision(float probability, float skillCost)
        {
            int result = Random.Range(1, 101);
            return result <= probability; // && skillCost <= Core.ScientificCount; //makes the AI more pressing
        }
    }
}