using _Application.Scripts.Infrastructure.Services;
using _Application.Scripts.Infrastructure.Services.Progress;
using _Application.Scripts.Infrastructure.States;
using _Application.Scripts.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Application.Scripts.UI.Windows
{
    [RequireComponent(typeof(GraphicRaycaster))]
    public class LoseWindow : Window
    {
        [SerializeField] 
        private Button _restartLevelButton;
        
        [SerializeField] 
        private Button _toUpgradeButton;

        [SerializeField]
        private TextMeshProUGUI _moneyText;

        private ProgressService _progressService;
        private LevelManager _levelManager;
        private AudioManager _audioManager;
        private StateMachine _stateMachine;


        public override void GetDependencies()
        {
            base.GetDependencies();

            _levelManager = AllServices.Get<LevelManager>();
            _audioManager = AllServices.Get<AudioManager>();
            _progressService = AllServices.Get<ProgressService>();
            _stateMachine = AllServices.Get<StateMachine>();
        }

        protected override void OnOpened()
        {
            base.OnOpened();
            
            _moneyText.text = _progressService.PlayerProgress.Money.ToString();
            
            _restartLevelButton.onClick.AddListener(RestartLevel);
            _toUpgradeButton.onClick.AddListener(OpenUpgrades);
        }

        protected override void OnClosed()
        {
            base.OnClosed();
            
            _restartLevelButton.onClick.RemoveListener(RestartLevel);
            _toUpgradeButton.onClick.RemoveListener(OpenUpgrades);
        }
        
        private void OpenUpgrades()
        {
            Close();
            UISystem.ShowPayloadedWindow<UpgradeWindow, bool>(false);
        }

        private void RestartLevel()
        {
            Close();
            
            _audioManager.PlayBackgroundAgain();
            _levelManager.RestartLevel();
            _stateMachine.Enter<GameLoopState>();
        }
    }
}