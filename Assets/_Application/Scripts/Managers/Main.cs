using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Application.Scripts.Control;
using _Application.Scripts.Infrastructure;
using _Application.Scripts.Infrastructure.Services;
using _Application.Scripts.Infrastructure.Services.Progress;
using _Application.Scripts.Infrastructure.Services.Scriptables;
using _Application.Scripts.SavedData;
using UnityEngine;

namespace _Application.Scripts.Managers
{
    public enum GameStates
    {
        Gameplay,
        GameOver
    }

    public class Main : IProgressWriter, IProgressReader
    {
        private readonly UI _ui;
        private readonly AI.Core _core;
        private readonly Outlook _outlook;
        private readonly Levels _levelsManager;
        private readonly ObjectPool _objectPool;
        private readonly UserControl _userControl;
        private readonly TeamManager _teamManager;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly IReadWriterService _readWriterService;
        private readonly IScriptableService _scriptableService;
        
        private GameObject _planetsLay;
        private bool _isWin;
        private GameStates _currentGameState;
        
        private List<Planets.Base> _allPlanets;
        
        private int _money;

        public Main(Levels levelsManager, TeamManager teamManager, ICoroutineRunner coroutineRunner, AI.Core core, 
            ObjectPool pool ,Outlook  outlook, UI ui, UserControl userControl, IReadWriterService readWriterService,
            IScriptableService scriptableService)
        {
            _allPlanets = new List<Planets.Base>();
            
            _teamManager = teamManager;
            _coroutineRunner = coroutineRunner;
            _levelsManager = levelsManager;
            _core = core;
            _objectPool = pool;
            _outlook = outlook;
            _ui = ui;
            _userControl = userControl;
            _readWriterService = readWriterService;
            _scriptableService = scriptableService;

            Planets.Base.Conquered += _teamManager.UpdateObjectsCount;
            _teamManager.GameEnded += EndGame;

            _ui.SetUIBehaviours(_teamManager, RetryLevel, LoadNextLevel);
            //levelsManager.SetCurrentLevelNumber(_lastCompletedLevel + 1);
        }

        public void Destroy()
        {
            UI.RemoveBehaviours(_teamManager);
            _teamManager.GameEnded -= EndGame;
        }
        
        public void StartGame()
        {
            _currentGameState = GameStates.Gameplay;
            UpdateState();
        }

        public void WriteProgress(PlayerProgress playerProgress)
        {
            playerProgress.money = _money;
            playerProgress.levelInfo.lastCompletedLevel = _levelsManager.CurrentLevelNumber;
        }

        public void ReadProgress(PlayerProgress playerProgress)
        {
            _money = playerProgress.money;
            _levelsManager.CurrentLevelNumber = playerProgress.levelInfo.lastCompletedLevel + 1;
            //ok
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

        private void SaveProgress() => 
            _readWriterService.WriteProgress();

        private void EndGame(bool isWin)
        {
            _coroutineRunner.CancelAllInvoked();
            _isWin = isWin;

            AddReward();
            
            SaveProgress();

            _currentGameState = GameStates.GameOver;
            UpdateState();
        }

        private void AddReward()
        {
            int lastCompletedLevel = _levelsManager.CurrentLevelNumber;
            int rewardMoney = _isWin ? _scriptableService.RewardList.GetReward(lastCompletedLevel) : 0;
            _money += rewardMoney;
            _ui.UpdateMoneyText(_money);
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