using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;
using Image = Microsoft.Unity.VisualStudio.Editor.Image;

namespace Managers
{
    public enum GameStates
    {
        Opening,
        Gameplay,
        GameOver
    }
    
    [DefaultExecutionOrder(500)]
    public class Main : MonoBehaviour
    {
        private readonly int _teamNumber = Enum.GetNames(typeof(Planets.Team)).Length;
        [SerializeField] private AI.Core core;
        //private AI.AI _ai;
        public List<Planets.Base> AllPlanets { get; private set; }
        [SerializeField] private GameObject planetsLay;
        private List<int> _objectsCount;

        public GameStates GameState { get; private set; }


        public static Main Instance;
        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            //Debug.Log(_teamNumber);
            _objectsCount=new List<int>(_teamNumber);
            for (int i = 0; i < _teamNumber;i++)
                _objectsCount.Add(0);
        }
        
        public void Start()
        {
            AllPlanets = planetsLay.GetComponentsInChildren<Planets.Base>().ToList();
            FillTeamCount();
            core = core.GetComponent<AI.Core>();
            if (core==null)
            {
                throw new MyException("cannot get ai component");
            }
            GameState = GameStates.Gameplay;
            UpdateState();
        }

        public void UpdateState()
        {
            switch (GameState)
            {
                case GameStates.Opening:
                {
                    break;
                }
                case GameStates.Gameplay:
                {
                    core.Init(AllPlanets);
                    core.Enable();
                    break;
                }
                case GameStates.GameOver:
                {
                    break;
                }
            }
        }
        
        private void FillTeamCount()
        {
            foreach (var planet in AllPlanets)
            {
                var team = (int) planet.Team;
                //Debug.Log(team);
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
                core.Disable();
            }

            if (noneRed)
            {
                Debug.Log("Win");
                core.Disable();
            }
        }
    }
}