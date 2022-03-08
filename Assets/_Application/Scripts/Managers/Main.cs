using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Application.Scripts.Control;
using UnityEngine;
using UnityEngine.UI;

namespace _Application.Scripts.Managers
{
    public enum GameStates
    {
        Gameplay,
        GameOver
    }
    
    public class Main : MonoBehaviour
    {
        public static Main Instance;
        private GameObject _nextLevelButton;
        private GameObject _retryLevelButton;


        private ObjectPool _objectPool;
        private Levels _levelsManager;
        private AI.Core _core;
        private UserControl _userControl;
        private Outlook _outlook;
        private UI _ui;
        
        private GameObject _planetsLay;
        private readonly int _teamNumber = Enum.GetNames(typeof(Planets.Team)).Length;
        private bool _isWin;
        private GameStates _currentGameState;


        public List<Planets.Base> AllPlanets { get; private set; }
        public List<int> ObjectsCount { get; private set; }


        public void Awake()
        {
            if (Instance == null) 
                Instance = this;

            ObjectsCount=new List<int>(_teamNumber);
            AllPlanets = new List<Planets.Base>();
            
            for (int i = 0; i < _teamNumber;i++)
                ObjectsCount.Add(0);
            
            _levelsManager=Levels.Instance;

            Planets.Base.Conquered += UpdateObjectsCount;
        }

        public void OnEnable()
        {
            _core = AI.Core.Instance;
            _objectPool = ObjectPool.Instance;
            _userControl = UserControl.Instance;
            _outlook = Outlook.Instance;
            _ui = UI.Instance;

            // _currentGameState = GameStates.Gameplay;
            // UpdateState();
        }

        public void OnDisable() => 
            Planets.Base.Conquered -= UpdateObjectsCount;

        public void SetButtons(GameObject retryButton, GameObject nextLevelButton)
        {
            _retryLevelButton = retryButton;
            _nextLevelButton = nextLevelButton;
            
            _retryLevelButton.GetComponent<Button>().onClick.AddListener(RetryLevel);
            _nextLevelButton.GetComponent<Button>().onClick.AddListener(LoadNextLevel);
            
            _retryLevelButton.SetActive(false);
            _nextLevelButton.SetActive(false);
        }

        public void StartGame()
        {
            _currentGameState = GameStates.Gameplay; ////////////////
            UpdateState(); //////////////////////
        }

        private void UpdateObjectsCount(Planets.Base planet ,Planets.Team oldTeam, Planets.Team newTeam)
        {
            ObjectsCount[(int) oldTeam]--;
            ObjectsCount[(int) newTeam]++;
          
            CheckGameOver();
        }

        private void CheckGameOver()
        {
            bool noneBlue = ObjectsCount[(int)Planets.Team.Blue]==0;
            bool noneRed = ObjectsCount[(int)Planets.Team.Red]==0;
            if (noneBlue || noneRed )
            {
                _isWin = noneRed;
                _currentGameState = GameStates.GameOver;
                UpdateState();
            }

        }

        private void PrepareLevel()
        {
            AllPlanets = _planetsLay.GetComponentsInChildren<Planets.Base>().ToList();
            
            foreach (Planets.Base planet in AllPlanets)
            {
                planet.gameObject.SetActive(true);
                planet.Init();
                planet.LaunchingUnit += SpawnUnit;
            }
            
            FillTeamCount();
        }

        private Units.Base SpawnUnit(ObjectPool.PoolObjectType poolObjectType, Vector3 launchPos, Quaternion rotation)
        {
            return _objectPool.GetObject(poolObjectType, launchPos, rotation).GetComponent<Units.Base>();
        }
        

        private void UpdateState()
        {
            switch (_currentGameState)
            { 
                case GameStates.Gameplay:
                {
                    HideSuitableUI();
                    StartCoroutine(StartGameplay());
                    break;
                }
                case GameStates.GameOver:
                {
                    _userControl.Disable();
                    _objectPool.DisableAllUnitsInScene();
                    _core.Disable();
                    ShowSuitableUI();
                    break;
                }
            }
        }

        private void HideSuitableUI()
        {
            _nextLevelButton.SetActive(false);
            _retryLevelButton.SetActive(false);
        }

        private void ShowSuitableUI()
        {
            if (_isWin)
                _nextLevelButton.SetActive(true);
            else
                _retryLevelButton.SetActive(true);
        }

        private IEnumerator StartGameplay()
        {
            ClearLists();
            yield return StartCoroutine(_levelsManager.InstantiateLevel());
            _planetsLay = _levelsManager.GetCurrentLay();
            PrepareLevel();
            _core.Init(AllPlanets);
            _core.Enable();
            _outlook.PrepareLevel(AllPlanets);
            _ui.PrepareLevel();
            
            Planets.Scientific.DischargeScientificCount();//sci-count = 0
            
            _userControl.Enable();
        }

        private void ClearLists()
        {
            foreach (Planets.Base planet in AllPlanets) 
                planet.LaunchingUnit -= SpawnUnit;

            for (int i = 0; i < _teamNumber;i++)
                ObjectsCount.Add(0);
            
            AllPlanets.Clear();
        }

        private void LoadNextLevel()
        {
            _levelsManager.SwitchToNextLevel();
            _currentGameState = GameStates.Gameplay;
            UpdateState();
        }

        private void RetryLevel()
        {
            _levelsManager.RestartLevel();
            _currentGameState = GameStates.Gameplay;
            UpdateState();
        }
        
        private void FillTeamCount()
        {
            foreach (Planets.Base planet in AllPlanets)
            {
                int team = (int) planet.Team;
                ObjectsCount[team]++;
            }
        }
    }
}