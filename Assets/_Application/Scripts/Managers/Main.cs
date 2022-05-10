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
using _Application.Scripts.UI;
using _Application.Scripts.UI.Windows;
using _Application.Scripts.UI.Windows.Tutorial;
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
        private readonly AI.Core _core;
        private readonly Outlook _outlook;
        private readonly Levels _levelsManager;
        private readonly IObjectPool _objectPool;
        private readonly UserControl _userControl;
        private readonly TeamManager _teamManager;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly IScriptableService _scriptableService;
        private readonly IProgressService _progressService;
        private readonly AudioManager _audioManager;

        private GameObject _buildingsLay;
        private bool _isWin;
        private GameStates _currentGameState;

        private List<Buildings.Base> _allBuildings;

        private int _lastCompletedLevel;

        public Main(Levels levelsManager, TeamManager teamManager, ICoroutineRunner coroutineRunner, AI.Core core,
            IObjectPool pool, Outlook outlook, UserControl userControl, IScriptableService scriptableService,
            IProgressService progressService, AudioManager audioManager)
        {
            _allBuildings = new List<Buildings.Base>();
            
            _teamManager = teamManager;
            _coroutineRunner = coroutineRunner;
            _levelsManager = levelsManager;
            _core = core;
            _objectPool = pool;
            _outlook = outlook;
            _userControl = userControl;
            _scriptableService = scriptableService;
            _progressService = progressService;
            _audioManager = audioManager;

            Buildings.Base.Conquered += _teamManager.UpdateObjectsCount;
            TeamManager.GameEnded += EndGame;
            AnimatedWindow.FadeCompleted += AnimatedWindow_FadeCompleted;

            UISystem.GetWindow<WinWindow>().NextLevelButton.onClick.AddListener(NextLevelButton_Clicked);
            UISystem.GetWindow<WinWindow>().ToUpgradeMenuButton.onClick.AddListener(ToUpgradeMenuButton_Clicked);
            UISystem.GetWindow<LoseWindow>().RestartButton.onClick.AddListener(RestartLevelButton_Clicked);
            UISystem.GetWindow<LoseWindow>().ToUpgradeMenuButton.onClick.AddListener(ToUpgradeMenuButton_Clicked);
            UISystem.GetWindow<UpgradeWindow>().BackToGameButton.onClick.AddListener(BackToGameButton_Clicked);
       }

        public void StartGame()
        {
            _currentGameState = GameStates.Gameplay;
            UpdateState();
        }

        public void WriteProgress(PlayerProgress playerProgress) => 
            playerProgress.levelInfo.lastCompletedLevel = _lastCompletedLevel;

        public void ReadProgress(PlayerProgress playerProgress)
        {
            _lastCompletedLevel = playerProgress.levelInfo.lastCompletedLevel;
            _levelsManager.CurrentLevelNumber = _lastCompletedLevel + 1;
        }

        private void PrepareLevel()
        {
            _allBuildings = _buildingsLay.GetComponentsInChildren<Buildings.Base>().ToList();
            
            foreach (Buildings.Base building in _allBuildings)
            {
                building.gameObject.SetActive(true);
                building.Construct(_scriptableService, _progressService);
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
                    UISystem.ReturnToPreviousWindow();
                    UISystem.ShowWindow<GameplayWindow>();
                    
                    _coroutineRunner.StartCoroutine(StartGameplay());

                    int currentLevelNumber = _levelsManager.CurrentLevelNumber;

                    if (currentLevelNumber <= 5) 
                        UISystem.ShowTutorialWindow(currentLevelNumber);

                    break;
                }
                case GameStates.GameOver:
                {
                    _userControl.Refresh();
                    _userControl.Disable();
                    _objectPool.DisableAllUnitsInScene();
                    _core.Disable();
                    
                    if(UISystem.IsTutorialDisplayed)
                        UISystem.ReturnToPreviousWindow();
                    
                    ShowEndGameWindow();

                    _audioManager.PlayEndgame(_isWin);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ShowEndGameWindow()
        {
            UISystem.ReturnToPreviousWindow();

            if (_isWin)
                UISystem.ShowWindow<WinWindow>();
            else
                UISystem.ShowWindow<LoseWindow>();
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
            int money = AllServices.Instance.GetSingle<IProgressService>().PlayerProgress.money;
            int rewardMoney = _isWin ? _scriptableService.RewardList.GetReward(_lastCompletedLevel) : 0;
            
            money += rewardMoney;
            AllServices.Instance.GetSingle<IProgressService>().PlayerProgress.money = money;
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
            
            CounterSpawner.ClearList();
            CounterSpawner.FillLists(_allBuildings);
            
            Buildings.Altar.DischargeManaCount();//count = 0
            
            
            
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

        private void NextLevelButton_Clicked()
        {
            _audioManager.PlayBackgroundAgain();
            _levelsManager.SwitchToNextLevel();
            StartGame();
        }

        private void RestartLevelButton_Clicked()
        {
            _audioManager.PlayBackgroundAgain();
            _levelsManager.RestartLevel();
            StartGame();
        }

        private void ToUpgradeMenuButton_Clicked()
        {
            UISystem.ReturnToPreviousWindow();
            UISystem.ShowWindow<UpgradeWindow>();
            
            _buildingsLay.SetActive(false);
        }

        private static void AnimatedWindow_FadeCompleted() => 
            UISystem.ReturnToPreviousWindow();

        private void BackToGameButton_Clicked()
        {
            _buildingsLay.SetActive(true);

            ShowEndGameWindow();
        }
    }
}