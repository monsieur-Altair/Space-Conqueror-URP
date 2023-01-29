using _Application.Scripts.Infrastructure.Services;
using _Application.Scripts.Infrastructure.States;
using UnityEngine;
using UnityEngine.UI;

namespace _Application.Scripts.UI.Windows
{
    public class LobbyWindow : Window
    {
        [SerializeField] 
        private Button _toStatsButton;
        
        [SerializeField] 
        private Button _toUpgradeButton;
        
        [SerializeField] 
        private Button _startGameButton;

        [SerializeField] 
        private Transform _counterParent;

        public Transform CounterParent => _counterParent;

        private StateMachine _stateMachine;


        public override void GetDependencies()
        {
            base.GetDependencies();

            _stateMachine = AllServices.Get<StateMachine>();
        }

        protected override void OnOpened()
        {
            base.OnOpened();
            
            _toStatsButton.onClick.AddListener(OpenStats);
            _toUpgradeButton.onClick.AddListener(OpenUpgrades);
            _startGameButton.onClick.AddListener(StartGame);
        }

        protected override void OnClosed()
        {
            base.OnClosed();
            
            _toStatsButton.onClick.RemoveListener(OpenStats);
            _toUpgradeButton.onClick.RemoveListener(OpenUpgrades);
            _startGameButton.onClick.RemoveListener(StartGame);
        }

        private void StartGame()
        {
            _stateMachine.Enter<GameLoopState>();   
        }

        private void OpenUpgrades()
        {
            Close();
            UISystem.ShowWindow<UpgradeWindow>();
        }
        
        private void OpenStats()
        {
            Close();
            UISystem.ShowWindow<StatisticWindow>();
        }
    }
}