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
    public class WinWindow : Window
    {
        [SerializeField] 
        private Button _nextLevelButton;
        
        [SerializeField] 
        private Button _toLobby;
        
        [SerializeField] 
        private TextMeshProUGUI _moneyText;

        private ProgressService _progressService;
        private StateMachine _stateMachine;
        private AudioManager _audioManager;
        private LevelManager _levelManager;

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
            
            _nextLevelButton.onClick.AddListener(GoNextLevel);
            _toLobby.onClick.AddListener(GoToLobby);
        }

        protected override void OnClosed()
        {
            base.OnClosed();
            
            _nextLevelButton.onClick.RemoveListener(GoNextLevel);
            _toLobby.onClick.RemoveListener(GoToLobby);
        }

        private void GoToLobby()
        {
            Close();
            _stateMachine.Enter<LobbyState>();
        }

        private void GoNextLevel()
        {
            _audioManager.PlayBackgroundAgain();
            _levelManager.SwitchToNextLevel();
            _stateMachine.Enter<GameLoopState>();
            
            Close();
        }
    }
}