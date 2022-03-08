using _Application.Scripts.Control;
using _Application.Scripts.Infrastructure.AssetManagement;
using _Application.Scripts.Infrastructure.Factory;
using _Application.Scripts.Infrastructure.Services;
using UnityEngine;

namespace _Application.Scripts.Infrastructure.States
{
    public class BootstrapState : IState
    {
        private const string StartUp = "StartUp";
        private const string Main = "Main";
        private readonly StateMachine _stateMachine;
        private readonly SceneLoader _sceneLoader;
        private readonly AllServices _allServices; 

        public BootstrapState(StateMachine stateMachine, SceneLoader sceneLoader, AllServices allServices)
        {
            _stateMachine = stateMachine;
            _sceneLoader = sceneLoader;
            _allServices = allServices;

            RegisterServices();
        }

        public void Enter()
        {
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
            _allServices.RegisterSingle<IAssetProvider>(new AssetProvider());
            _allServices.RegisterSingle<IGameFactory>(new GameFactory(_allServices.GetSingle<IAssetProvider>()));
            //register input service
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