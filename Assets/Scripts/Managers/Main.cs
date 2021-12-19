using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TMPro;
using UnityEngine;

namespace Managers
{
    [DefaultExecutionOrder(500)]
    public class Main : MonoBehaviour
    {
        //one of them is neutral
        private readonly int _teamNumber = Enum.GetNames(typeof(Planets.Team)).Length - 1;
        public List<Planets.Base> AllPlanets { get; private set; }
        [SerializeField] private GameObject planetsLay;
        private List<int> _objectsCount;

        public static Main Instance;
        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            
            _objectsCount=new List<int>(_teamNumber);
            for (int i = 0; i < _teamNumber;i++)
                _objectsCount.Add(0);
        }
        
        public void Start()
        {
            AllPlanets = planetsLay.GetComponentsInChildren<Planets.Base>().ToList();
            FillTeamCount();
        }

        private void FillTeamCount()
        {
            foreach (var planet in AllPlanets)
            {
                var team = (int) planet.Team;
                _objectsCount[team]++;
            }
        }

        public void UpdateObjectsCount(Planets.Team oldTeam, Planets.Team newTeam)
        {
            _objectsCount[(int) oldTeam]--;
            _objectsCount[(int) newTeam]++;
        }

        public void CheckGameOver()
        {
            var noneBlue = _objectsCount[(int)Planets.Team.Blue]==0;
            var noneRed = _objectsCount[(int)Planets.Team.Red]==0;
            
            if (noneBlue)
            {
                Debug.Log("Game over");
            }

            if (noneRed)
            {
                Debug.Log("Win");
            }
        }
        
    }
}