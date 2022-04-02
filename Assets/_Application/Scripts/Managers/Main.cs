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
        private readonly IScriptableService _scriptableService;
        private readonly IReadWriterService _readWriterService;
        
        private GameObject _buildingsLay;
        private bool _isWin;
        private GameStates _currentGameState;
        
        private List<Buildings.Base> _allBuildings;
        
        public static int _money; /////////////////////////////////////////////////////////////////////////
        private int _lastCompletedLevel;

        public Main(Levels levelsManager, TeamManager teamManager, ICoroutineRunner coroutineRunner, AI.Core core, 
            ObjectPool pool ,Outlook  outlook, UI ui, UserControl userControl, IReadWriterService readWriterService,
            IScriptableService scriptableService)
        {
            _allBuildings = new List<Buildings.Base>();
            
            _teamManager = teamManager;
            _coroutineRunner = coroutineRunner;
            _levelsManager = levelsManager;
            _core = core;
            _objectPool = pool;
            _outlook = outlook;
            _ui = ui;
            _userControl = userControl;
            _scriptableService = scriptableService;

            _readWriterService = readWriterService;
            
            Buildings.Base.Conquered += _teamManager.UpdateObjectsCount;
            _teamManager.GameEnded += EndGame;

            _ui.SetUIBehaviours(_teamManager, RetryLevel, LoadNextLevel, ToUpgradeMenuBehaviour, BackToGame);
        }

        private void BackToGame()
        {
            _ui.HideUpgradeMenu();
            _ui.ShowSkillsButtons();
            _buildingsLay.SetActive(true);

            _money = AllServices.Instance.GetSingle<IProgressService>().PlayerProgress.money;///////////////////////////
            _ui.UpdateMoneyText(_money);////////////////////////////////////////////////////////////////////////////////
            
            _ui.ShowGameOverUI(_isWin);
        }

        private void ToUpgradeMenuBehaviour()
        {
            _ui.HideSkillsButtons();
            _ui.HideGameOverUI();
            _buildingsLay.SetActive(false);
            _ui.ShowUpgradeMenu();
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
            playerProgress.levelInfo.lastCompletedLevel = _lastCompletedLevel;
        }

        public void ReadProgress(PlayerProgress playerProgress)
        {
            _money = playerProgress.money;
            _lastCompletedLevel = playerProgress.levelInfo.lastCompletedLevel;
            _levelsManager.CurrentLevelNumber = _lastCompletedLevel + 1;
            //ok
        }

        private void PrepareLevel()
        {
            _allBuildings = _buildingsLay.GetComponentsInChildren<Buildings.Base>().ToList();
            
            foreach (Buildings.Base building in _allBuildings)
            {
                building.gameObject.SetActive(true);
                building.Init();
                building.LaunchingUnit += SpawnUnit;
            }
            
            _teamManager.FillTeamCount(_allBuildings);
        }

        private Units.Base SpawnUnit(PoolObjectType poolObjectType, Vector3 launchPos, Quaternion rotation) => 
            _objectPool.GetObject(poolObjectType, launchPos, rotation).GetComponent<Units.Base>();

        private void UpdateState()
        {
            switch (_currentGameState)
            { 
                case GameStates.Gameplay:
                {
                    _ui.HideGameOverUI();
                    _ui.ShowGameplayUI();
                    _coroutineRunner.StartCoroutine(StartGameplay());
                    break;
                }
                case GameStates.GameOver:
                {
                    _userControl.Refresh();
                    _userControl.Disable();
                    _objectPool.DisableAllUnitsInScene();
                    _core.Disable();
                    _ui.DisableSkillUI();
                    _ui.ShowGameOverUI(_isWin);
                    _ui.HideGameplayUI();
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

            if (_isWin)
                _lastCompletedLevel = _levelsManager.CurrentLevelNumber;
            
            AddReward();
            
            _currentGameState = GameStates.GameOver;
            UpdateState();
        }

        private void AddReward()
        {
            int rewardMoney = _isWin ? _scriptableService.RewardList.GetReward(_lastCompletedLevel) : 0;
            
            _money += rewardMoney;
            AllServices.Instance.GetSingle<IProgressService>().PlayerProgress.money = _money;//////////////////////////////
            _ui.UpdateMoneyText(_money);
        }

        private IEnumerator StartGameplay()
        {
            ClearLists();
            yield return _coroutineRunner.StartCoroutine(_levelsManager.InstantiateLevel());
            _buildingsLay = _levelsManager.GetCurrentLay();
            PrepareLevel();
            _core.Init(_allBuildings);
            _core.Enable();
            _outlook.PrepareLevel(_allBuildings);
            _ui.PrepareLevel(_allBuildings);
            
            Buildings.Altar.DischargeManaCount();//count = 0
            
            _ui.EnableSkillUI();
            _userControl.Reload();
            _userControl.Enable();
        }

        private void ClearLists()
        {
            foreach (Buildings.Base building in _allBuildings) 
                building.LaunchingUnit -= SpawnUnit;

            _teamManager.Clear();
            _allBuildings.Clear();
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