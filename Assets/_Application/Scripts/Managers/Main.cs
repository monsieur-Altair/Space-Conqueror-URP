using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Application.Scripts.Control;
using _Application.Scripts.Infrastructure;
using UnityEngine;

namespace _Application.Scripts.Managers
{
    public enum GameStates
    {
        Gameplay,
        GameOver
    }

    public class Main
    {
        private readonly UI _ui;
        private readonly AI.Core _core;
        private readonly Outlook _outlook;
        private readonly Levels _levelsManager;
        private readonly ObjectPool _objectPool;
        private readonly UserControl _userControl;
        private readonly TeamManager _teamManager;
        private readonly ICoroutineRunner _coroutineRunner;
        
        private GameObject _planetsLay;
        private bool _isWin;
        private GameStates _currentGameState;
        
        private List<Planets.Base> _allPlanets;

        public Main(Levels levelsManager, TeamManager teamManager, ICoroutineRunner coroutineRunner,
            AI.Core core, ObjectPool pool ,Outlook  outlook, UI ui, UserControl userControl)
        {
            _allPlanets = new List<Planets.Base>();
            _teamManager = teamManager;
            _coroutineRunner = coroutineRunner;
            _levelsManager = levelsManager;

            Planets.Base.Conquered += _teamManager.UpdateObjectsCount;
            _teamManager.GameEnded += EndGame;
            
            _core = core;
            _objectPool = pool;
            _outlook = outlook;
            _ui = ui;
            _userControl = userControl;
            _ui.SetUIBehaviours(_teamManager, RetryLevel, LoadNextLevel);
        }

        public void Destroy()
        {
            _ui.RemoveBehaviours(_teamManager);
            _teamManager.GameEnded -= EndGame;
        }
        
        public void StartGame()
        {
            _currentGameState = GameStates.Gameplay;
            UpdateState();
        }

        private void PrepareLevel()
        {
            _allPlanets = _planetsLay.GetComponentsInChildren<Planets.Base>().ToList();
            
            foreach (Planets.Base planet in _allPlanets)
            {
                planet.gameObject.SetActive(true);
                planet.Init();
                planet.LaunchingUnit += SpawnUnit;
            }
            
            _teamManager.FillTeamCount(_allPlanets);
        }

        private Units.Base SpawnUnit(PoolObjectType poolObjectType, Vector3 launchPos, Quaternion rotation) => 
            _objectPool.GetObject(poolObjectType, launchPos, rotation).GetComponent<Units.Base>();

        private void UpdateState()
        {
            switch (_currentGameState)
            { 
                case GameStates.Gameplay:
                {
                    _ui.ShowGameplayUI();
                    _coroutineRunner.StartCoroutine(StartGameplay());
                    break;
                }
                case GameStates.GameOver:
                {
                    _userControl.Disable();
                    _objectPool.DisableAllUnitsInScene();
                    _core.Disable();
                    _ui.DisableSkillUI();
                    _ui.ShowGameOverUI(_isWin);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void EndGame(bool isWin)
        {
            _coroutineRunner.CancelAllInvoked();
            
            _isWin = isWin;
            _currentGameState = GameStates.GameOver;
            UpdateState();
        }

        private IEnumerator StartGameplay()
        {
            ClearLists();
            yield return _coroutineRunner.StartCoroutine(_levelsManager.InstantiateLevel());
            _planetsLay = _levelsManager.GetCurrentLay();
            PrepareLevel();
            _core.Init(_allPlanets);
            _core.Enable();
            _outlook.PrepareLevel(_allPlanets);
            _ui.PrepareLevel(_allPlanets);
            
            Planets.Scientific.DischargeScientificCount();//sci-count = 0
            
            _ui.EnableSkillUI();
            _userControl.Enable();
        }

        private void ClearLists()
        {
            foreach (Planets.Base planet in _allPlanets) 
                planet.LaunchingUnit -= SpawnUnit;

            _teamManager.Clear();
            _allPlanets.Clear();
        }

        private void LoadNextLevel()
        {
            _levelsManager.SwitchToNextLevel();
            StartGame();
        }

        private void RetryLevel()
        {
            _levelsManager.RestartLevel();
            StartGame();
        }
    }
}