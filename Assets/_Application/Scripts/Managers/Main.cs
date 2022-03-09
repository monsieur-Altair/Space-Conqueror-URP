using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Application.Scripts.Control;
using UnityEngine;

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

        private ObjectPool _objectPool;
        private Levels _levelsManager;
        private AI.Core _core;
        private UserControl _userControl;
        private Warehouse _warehouse;
        private UI _ui;
        private TeamManager _teamManager;
        
        private GameObject _planetsLay;
        private bool _isWin;
        private GameStates _currentGameState;

        
        public List<Planets.Base> AllPlanets { get; private set; }

        public void Awake()
        {
            if (Instance == null) 
                Instance = this;

            AllPlanets = new List<Planets.Base>();

            _teamManager = new TeamManager();
            
            _levelsManager=Levels.Instance;

            Planets.Base.Conquered += _teamManager.UpdateObjectsCount;
            _teamManager.GameEnded += EndGame;
        }

        public void OnEnable()
        {
            _core = AI.Core.Instance;
            _objectPool = ObjectPool.Instance;
            _userControl = UserControl.Instance;
            _warehouse = Warehouse.Instance;
            _ui = UI.Instance;

            _ui.SetUIBehaviours(_teamManager, RetryLevel, LoadNextLevel);
            
            // _currentGameState = GameStates.Gameplay;
            // UpdateState();
        }

        public void OnDisable()
        {
            _ui.RemoveBehaviours(_teamManager);
        }

        public void OnDestroy()
        {
            _teamManager.GameEnded -= EndGame;
        }
        
        public void StartGame()
        {
            _currentGameState = GameStates.Gameplay; ////////////////
            UpdateState(); //////////////////////
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
            
            _teamManager.FillTeamCount(AllPlanets);
        }

        private Units.Base SpawnUnit(ObjectPool.PoolObjectType poolObjectType, Vector3 launchPos, Quaternion rotation) => 
            _objectPool.GetObject(poolObjectType, launchPos, rotation).GetComponent<Units.Base>();

        private void UpdateState()
        {
            switch (_currentGameState)
            { 
                case GameStates.Gameplay:
                {
                    _ui.ShowGameplayUI();
                    StartCoroutine(StartGameplay());
                    break;
                }
                case GameStates.GameOver:
                {
                    _userControl.Disable();
                    _objectPool.DisableAllUnitsInScene();
                    _core.Disable();
                    _ui.ShowGameOverUI(_isWin);
                    break;
                }
            }
        }

        private void EndGame(bool isWin)
        {
            _isWin = isWin;
            _currentGameState = GameStates.GameOver;
            UpdateState();
        }

        private IEnumerator StartGameplay()
        {
            ClearLists();
            yield return StartCoroutine(_levelsManager.InstantiateLevel());
            _planetsLay = _levelsManager.GetCurrentLay();
            PrepareLevel();
            _core.Init(AllPlanets);
            _core.Enable();
            _warehouse.PrepareLevel(AllPlanets);
            _ui.PrepareLevel();
            
            Planets.Scientific.DischargeScientificCount();//sci-count = 0
            
            _userControl.Enable();
        }

        private void ClearLists()
        {
            foreach (Planets.Base planet in AllPlanets) 
                planet.LaunchingUnit -= SpawnUnit;

            _teamManager.Clear();
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
    }
}