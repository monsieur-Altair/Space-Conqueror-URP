using System.Collections.Generic;
using _Application.Scripts.Control;
using _Application.Scripts.Infrastructure.Services;
using _Application.Scripts.Infrastructure.Services.AssetManagement;
using _Application.Scripts.Infrastructure.Services.Factory;
using _Application.Scripts.Infrastructure.Services.Progress;
using _Application.Scripts.Infrastructure.Services.Scriptables;
using _Application.Scripts.Managers;
using _Application.Scripts.Misc;
using _Application.Scripts.SavedData;
using UnityEngine;

namespace _Application.Scripts.Infrastructure.States
{
    public class BootstrapState : IState
    {
        private const string StartUp = "StartUp";
        private const string Main = "Main";
        private readonly StateMachine _stateMachine;
        private readonly SceneLoader _sceneLoader;
        private readonly IProgressService _progressService;
        private readonly IReadWriterService _readWriterService;
        private readonly CoreConfig _coreConfig;

        public BootstrapState(StateMachine stateMachine, SceneLoader sceneLoader, CoreConfig coreConfig)
        {
            _coreConfig = coreConfig;
            _stateMachine = stateMachine;
            _sceneLoader = sceneLoader;
            
            RegisterServices();
            RegisterMonoBehServices();


            _progressService = AllServices.Get<IProgressService>();
            _readWriterService = AllServices.Get<IReadWriterService>();
            
            AllServices.Register(new SkillController(_progressService, 
                AllServices.Get<ObjectPool>(), 
                AllServices.Get<ScriptableService>()));
        }

        private void RegisterMonoBehServices()
        {
            var gameFactory = AllServices.Get<GameFactory>();
            var servicePrefabs = _coreConfig.MonoBehaviourServices;

            AllServices.Register(gameFactory.CreateMonoBeh(servicePrefabs.AudioManager));
            AllServices.Register(gameFactory.CreateMonoBeh(servicePrefabs.CoroutineRunner));
            AllServices.Register(gameFactory.CreateMonoBeh(servicePrefabs.ObjectPool));
            AllServices.Register(gameFactory.CreateMonoBeh(servicePrefabs.LevelManager));
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
            _progressService.PlayerProgress = _readWriterService.ReadProgress() ?? new PlayerProgress(-1);


        private void RegisterServices()
        {
            AssetProvider assetProvider = AllServices.Register(new AssetProvider());

            ScriptableService scriptableService = AllServices.Register(new ScriptableService(assetProvider));
            scriptableService.LoadAllScriptables();

            GameFactory factory = AllServices.Register(new GameFactory(assetProvider, scriptableService));

            IProgressService progressService = AllServices.Register<IProgressService>(new ProgressService());

            AllServices.Register<IReadWriterService>(new ReadWriterService(progressService, factory));

            Camera camera = AllServices.Get<GameFactory>().CreateCamera();
            camera.GetComponent<CameraResolution>().Init(camera);
            
            AllServices.Register(_stateMachine);

            //register input service
        }

    }
}