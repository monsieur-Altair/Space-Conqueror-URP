using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Application.Scripts.Control;
using UnityEngine;

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
        [SerializeField] private AI.Core core;
        [SerializeField] private GameObject nextLevelButton;
        [SerializeField] private GameObject retryLevelButton;

        public List<Planets.Base> AllPlanets { get; private set; } 
        public static List<int> ObjectsCount { get; private set; }
        public GameStates CurrentGameState { get; private set; }
        
        private ObjectPool _objectPool;
        private GameObject _planetsLay;
        private Levels _levelsManager;
        private readonly int _teamNumber = Enum.GetNames(typeof(Planets.Team)).Length;
        private bool _isWin;

        public static Main Instance;
        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            ObjectsCount=new List<int>(_teamNumber);
            for (int i = 0; i < _teamNumber;i++)
                ObjectsCount.Add(0);
            
            _levelsManager=Levels.Instance;
            if (_levelsManager == null)
                throw new MyException("cannot get level manager");

            nextLevelButton.SetActive(false);
            retryLevelButton.SetActive(false);
        }
        
        public void OnEnable()
        {
            core = core.GetComponent<AI.Core>();
            if (core==null)
            {
                throw new MyException("cannot get ai component");
            }
            _objectPool = ObjectPool.Instance;
            if (_objectPool==null)
            {
                throw new MyException("cannot get object pool component");
            }
            
            CurrentGameState = GameStates.Gameplay;
            UpdateState();
        }

        private void PrepareLevel()
        {
            AllPlanets = _planetsLay.GetComponentsInChildren<Planets.Base>().ToList();
            
            foreach (Planets.Base planet in AllPlanets)
            {
                planet.gameObject.SetActive(true);
                planet.Init();
            }
            
            FillTeamCount();
            
        }


        private void UpdateState()
        {
            switch (CurrentGameState)
            {
                case GameStates.Opening:
                {
                    break;
                }
                case GameStates.Gameplay:
                {
                    nextLevelButton.SetActive(false);
                    retryLevelButton.SetActive(false);
                    StartCoroutine(StartGameplay());
                    break;
                }
                case GameStates.GameOver:
                {
                    UserControl.Instance.isActive = false;
                    _objectPool.DisableAllUnitsInScene();
                    core.Disable();
                    if(_isWin)
                        nextLevelButton.SetActive(true);
                    else 
                        retryLevelButton.SetActive(true);
                    break;
                }
            }
        }

        private IEnumerator StartGameplay()
        {
            yield return StartCoroutine(_levelsManager.InstantiateLevel());
            _planetsLay = _levelsManager.GetCurrentLay();
            this.PrepareLevel();
            core.Init(AllPlanets);
            core.Enable();
            Outlook.Instance.PrepareLevel();
            UI.Instance.PrepareLevel();
            Planets.Scientific.DischargeScientificCount();
            UserControl.Instance.isActive = true;
        }
        
        public void LoadNextLevel()
        {
            _levelsManager.SwitchToNextLevel();
            CurrentGameState = GameStates.Gameplay;
            UpdateState();
        }

        public void RetryLevel()
        {
            _levelsManager.RestartLevel();
            CurrentGameState = GameStates.Gameplay;
            UpdateState();
        }
        
        
        private void FillTeamCount()
        {
            ObjectsCount.Clear();
            for (int i = 0; i < _teamNumber;i++)
                ObjectsCount.Add(0);
            foreach (Planets.Base planet in AllPlanets)
            {
                int team = (int) planet.Team;
                ObjectsCount[team]++;
            }
        }

        public void UpdateObjectsCount(Planets.Team oldTeam, Planets.Team newTeam)
        {
            ObjectsCount[(int) oldTeam]--;
            ObjectsCount[(int) newTeam]++;
        }

        public void CheckGameOver()
        {
            bool noneBlue = ObjectsCount[(int)Planets.Team.Blue]==0;
            bool noneRed = ObjectsCount[(int)Planets.Team.Red]==0;
            if (noneBlue || noneRed )
            {
                _isWin = noneRed;
                CurrentGameState = GameStates.GameOver;
                UpdateState();
            }

        }
    }
}