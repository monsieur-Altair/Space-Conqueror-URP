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
        }

        protected override void OnClosed()
        {
            base.OnClosed();
            
            _playButton.onClick.RemoveListener(EnterLoadLevel);
        }

        private void EnterLoadLevel()
        {
            Close();
            _stateMachine.Enter<LoadLevelState, string>("Main");
        }
        
    }
}