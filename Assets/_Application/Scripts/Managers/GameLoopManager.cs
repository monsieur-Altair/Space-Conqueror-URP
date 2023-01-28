﻿using System;
using System.Collections;
using System.Collections.Generic;
using _Application.Scripts.Control;
using _Application.Scripts.Infrastructure.Services;
using _Application.Scripts.Infrastructure.Services.Progress;
using _Application.Scripts.Infrastructure.Services.Scriptables;
using _Application.Scripts.SavedData;
using _Application.Scripts.UI;
using _Application.Scripts.UI.Windows;
using Pool_And_Particles;

namespace _Application.Scripts.Managers
{
    public enum GameStates
    {
        GameStarted,
        GameEnded, 
        Exit
    }

    public class GameLoopManager : IProgressWriter, IProgressReader
    {
        private const int MaxTutorialCount = 5;
        
        private readonly AI.Core _ai;
        private readonly OutlookService _outlookService;
        private readonly LevelManager _levelsManager;
        private readonly GlobalPool _objectPool;
        private readonly UserControl _userControl;
        private readonly TeamManager _teamManager;
        private readonly CoroutineRunner _coroutineRunner;
        private readonly ScriptableService _scriptableService;
        private readonly ProgressService _progressService;
        private readonly AudioManager _audioManager;
        private readonly bool _useTutorial;

        private bool _isWin;
        private GameStates _currentGameState;

        private List<Buildings.BaseBuilding> _allBuildings;

        private int _lastCompletedLevel;
        private bool _hasUpgradeTutorialShown;
        private readonly UnitSupervisor _unitSupervisor;
        private CounterSpawner _counterSpawner;

        public GameLoopManager(LevelManager levelsManager, TeamManager teamManager, CoroutineRunner coroutineRunner, AI.Core ai,
            GlobalPool pool, OutlookService outlookService, UserControl userControl, ScriptableService scriptableService,
            ProgressService progressService, AudioManager audioManager, CoreConfig coreConfig, CounterSpawner counterSpawner)
        {
            _unitSupervisor = new UnitSupervisor(pool);
            _allBuildings = new List<Buildings.BaseBuilding>();

            _useTutorial = AllServices.Get<CoreConfig>().UseTutorial;
            _teamManager = teamManager;
            _coroutineRunner = coroutineRunner;
            _levelsManager = levelsManager;
            _ai = ai;
            _objectPool = pool;
            _outlookService = outlookService;
            _userControl = userControl;
            _scriptableService = scriptableService;
            _progressService = progressService;
            _audioManager = audioManager;
            _counterSpawner = counterSpawner;

            Buildings.BaseBuilding.Conquered += _teamManager.UpdateObjectsCount;
            TeamManager.GameEnded += EndGame;
       }

        public void StartGame()
        {
            _currentGameState = GameStates.GameStarted;
            UpdateState();
        }

        public void WriteProgress(PlayerProgress playerProgress)
        {
            playerProgress.LevelInfo.lastCompletedLevel = _lastCompletedLevel;
        }

        public void ReadProgress(PlayerProgress playerProgress)
        {
            _lastCompletedLevel = playerProgress.LevelInfo.lastCompletedLevel;
            _levelsManager.CurrentLevelNumber = _lastCompletedLevel + 1;
        }

        public void Clear()
        {
            _levelsManager.DeleteCurrentLevel();
        }

        private void PrepareLevel()
        {
            _allBuildings = _levelsManager.GetCurrentBuildings();
            
            foreach (Buildings.BaseBuilding building in _allBuildings)
            {
                building.gameObject.SetActive(true);
                PoolObjectType poolObjectType = (PoolObjectType)((int) building.BuildingType);
                var unitPrefab = _objectPool.GetPooledBehaviourPrefab(poolObjectType);
                building.Construct(_scriptableService, _progressService, unitPrefab.GetComponent<Units.BaseUnit>(), _objectPool);
            }
            
            _teamManager.FillTeamCount(_allBuildings);
        }


        private void UpdateState()
        {
            switch (_currentGameState)
            { 
                case GameStates.GameStarted:
                {
                    _coroutineRunner.StartCoroutine(StartGameplay());
                    
                    UISystem.ShowWindow<GameplayWindow>();

                    int currentLevelNumber = _levelsManager.CurrentLevelNumber;

                    if (currentLevelNumber <= MaxTutorialCount && _useTutorial) 
                        UISystem.ShowTutorialWindow(currentLevelNumber);

                    break;
                }
                case GameStates.GameEnded:
                {
                    _userControl.Refresh();
                    _userControl.Disable();
                    _userControl.InputService.BuildingsController.ClearAllSelection();
                    _unitSupervisor.DisableAll();
                    _counterSpawner.ClearList();
                    _ai.Disable();

                    int currentLevelNumber = _levelsManager.CurrentLevelNumber;
                    if (currentLevelNumber <= MaxTutorialCount && _useTutorial) 
                        UISystem.CloseTutorialWindow(currentLevelNumber);
                    
                    ShowEndGameWindow();

                    _audioManager.PlayEndgame(_isWin);
                    break;
                }
                case GameStates.Exit:
                    
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ShowEndGameWindow()
        {
            UISystem.CloseWindow<GameplayWindow>();

            if (_isWin)
                UISystem.ShowWindow<WinWindow>();
            else
                UISystem.ShowWindow<LoseWindow>();
        }

        private void EndGame(bool isWin)
        {
            _coroutineRunner.CancelAllInvoked();
            _isWin = isWin;

            _progressService.PlayerProgress.Statistic.GainedMana += (int)Buildings.Altar.SavedManaCount;

            if (_isWin)
            {
                _lastCompletedLevel = _levelsManager.CurrentLevelNumber;
                _progressService.PlayerProgress.Statistic.WinCount++;
            }
            
            AddReward();
            
            _currentGameState = GameStates.GameEnded;
            UpdateState();
        }

        private void AddReward()
        {
            int money = AllServices.Get<ProgressService>().PlayerProgress.Money;
            int rewardMoney = _isWin ? _scriptableService.RewardList.GetReward(_lastCompletedLevel) : 0;
            
            money += rewardMoney;
            AllServices.Get<ProgressService>().PlayerProgress.Money = money;
        }

        private IEnumerator StartGameplay()
        {
            ClearLists();
            yield return _coroutineRunner.StartCoroutine(_levelsManager.CreateLevel());
            PrepareLevel();
            _ai.Init(_allBuildings);
            _ai.Enable();
            _outlookService.PrepareLevel(_allBuildings);
            
            _counterSpawner.FillLists(_allBuildings);
            
            Buildings.Altar.DischargeManaCount();//count = 0
            Buildings.Altar.DischargeSavedManaCount();
            
            _userControl.Reload();
            _userControl.Enable();
        }

        private void ClearLists()
        {
            _teamManager.Clear();
            _allBuildings.Clear();
        }
    }
}