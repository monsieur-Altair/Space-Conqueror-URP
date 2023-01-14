using _Application.Scripts.Infrastructure.Services;
using _Application.Scripts.Infrastructure.States;
using UnityEngine;
using UnityEngine.UI;

namespace _Application.Scripts.UI.Windows
{
    [RequireComponent(typeof(GraphicRaycaster))]
    public class StartUpWindow : Window
    {
        [SerializeField] 
        private Button _playButton;

        //[SerializeField] 
        //private Button _toStatsButton;

        private StateMachine _stateMachine;

        public override void GetDependencies()
        {
            base.GetDependencies();

            _stateMachine = AllServices.Get<StateMachine>();
        }

        protected override void OnOpened()
        {
            base.OnOpened();

            _playButton.onClick.AddListener(EnterLoadLevel);
            //_toStatsButton.onClick.AddListener(OnStatsClicked);
        }

        protected override void OnClosed()
        {
            base.OnClosed();
            
            _playButton.onClick.RemoveListener(EnterLoadLevel);
            //_toStatsButton.onClick.RemoveListener(OnStatsClicked);

        }

        private void EnterLoadLevel() => 
            _stateMachine.Enter<LoadLevelState, string>("Main");

        private static void OnStatsClicked()
        {
            UISystem.ReturnToPreviousWindow();
            UISystem.ShowWindow<StatisticWindow>();
        }
    }
}