using _Application.Scripts.Control;
using UnityEngine;

namespace _Application.Scripts.Infrastructure.States
{
    public class BootstrapState : IState
    {
        private const string StartUp = "StartUp";
        private const string Main = "Main";
        private readonly StateMachine _stateMachine;
        private readonly SceneLoader _sceneLoader;

        public BootstrapState(StateMachine stateMachine, SceneLoader sceneLoader)
        {
            _stateMachine = stateMachine;
            _sceneLoader = sceneLoader;
        }

        public void Enter()
        {
            RegisterServices();
            //_sceneLoader.Load(Initial, onLoaded: EnterLoadLevel);
            _sceneLoader.Load(StartUp);
        }

        public void Exit()
        {
            
        }

        private void EnterLoadLevel() => 
            _stateMachine.Enter<LoadLevelState, string>("Main");

        private void RegisterServices()
        {
            //Game.InputService = RegisterInputService();
        }

        private IInputService RegisterInputService()
        {
            if (Application.isEditor)
                return new StandaloneInput();
            else
                return new MobileInput();
        }

    }
}