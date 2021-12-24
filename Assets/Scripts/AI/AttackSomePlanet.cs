using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AI
{
    public class AttackSomePlanet : MonoBehaviour, IAction
    {
        private List<List<Planets.Base>> _allPlanets;
        private AI _ai;
        private Planets.Base _target;
        private readonly List<Planets.Base> _planetsForLaunch = new List<Planets.Base>();
        private Vector3 _mainPos;
        private int _countForLaunch;
        private const int MINCountForLaunch = 1;//inclusive
        private const int MAXCountForLaunch = 4;//exclusive
        
        public void InitAction()
        {
            _ai = AI.Instance;
            _allPlanets = _ai.AllPlanets;
            _mainPos = _ai.MainPos;
        }

        public void Execute()
        {
            _planetsForLaunch.Clear();
            _countForLaunch = Random.Range(MINCountForLaunch, MAXCountForLaunch);
            
            StartCoroutine(LaunchToTarget());
        }

        private IEnumerator LaunchToTarget()
        {
            yield return StartCoroutine(FindTargetForLaunch());
            
            foreach (var planet in _planetsForLaunch)
            {
                planet.LaunchUnit(_target);
            }
        }
        
        private IEnumerator FindTargetForLaunch()
        {
            var minDistance = float.PositiveInfinity;
            
            foreach (var possibleTarget in _allPlanets[AI.Neutral])
            {
                var distance = Vector3.Distance(_mainPos, possibleTarget.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    _target = possibleTarget;
                }

                yield return null;
            }

            
            var launchPos = _target.transform.position;
            //Dictionary<int,float> dataForLaunch = new Dictionary<int, float>();
            List<KeyValuePair<Planets.Base, float>> dataForLaunch = new List<KeyValuePair<Planets.Base, float>>();
            foreach (var planetForLaunch in _allPlanets[AI.Own])
            {
                var distance = Vector3.Distance(launchPos, planetForLaunch.transform.position);
                dataForLaunch.Add(new KeyValuePair<Planets.Base, float>(planetForLaunch,distance));
                yield return null;
            }
            
            
            //sort list by ascending
            dataForLaunch.Sort((pair1,pair2)=>pair1.Value.CompareTo(pair2.Value));
            yield return null;

            
            if (_countForLaunch > dataForLaunch.Count)
                _countForLaunch = dataForLaunch.Count;
            for (var i = 0; i < _countForLaunch; i++)
            {
                _planetsForLaunch.Add(dataForLaunch[i].Key);
                yield return null;
            }
            

        }
    }
}