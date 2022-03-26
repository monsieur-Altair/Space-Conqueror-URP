using _Application.Scripts.Control;
using _Application.Scripts.Infrastructure.Services;
using _Application.Scripts.Infrastructure.Services.AssetManagement;
using _Application.Scripts.Infrastructure.Services.Factory;
using _Application.Scripts.Infrastructure.Services.Progress;
using _Application.Scripts.Infrastructure.Services.Scriptables;
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
            _sceneLoader.Load(StartUp, onLoaded: ReadProgress);
            //_sceneLoader.Load(StartUp);
        }

        public void Exit()
        {
            
        }

        private void ReadProgress() => 
            _stateMachine.Enter<ReadProgressState>();

        private void RegisterServices()
        {
            IAssetProvider assetProvider = _allServices.RegisterSingle<IAssetProvider>(
                new AssetProvider());
            
            IScriptableService scriptableService = _allServices.RegisterSingle<IScriptableService>(
                new ScriptableService(assetProvider));
            scriptableService.LoadAllScriptables();
            
            IGameFactory factory = _allServices.RegisterSingle<IGameFactory>(
                new GameFactory(assetProvider, scriptableService));
            
            IProgressService progressService = _allServices.RegisterSingle<IProgressService>(
                new ProgressService());
            
            _allServices.RegisterSingle<IReadWriterService>(
                new ReadWriterService(progressService, factory));
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