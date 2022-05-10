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
        private readonly AllServices _allServices;
        private readonly IProgressService _progressService;
        private readonly IReadWriterService _readWriterService;

        public BootstrapState(StateMachine stateMachine, SceneLoader sceneLoader, AllServices allServices)
        {
            _stateMachine = stateMachine;
            _sceneLoader = sceneLoader;
            _allServices = allServices;
            
            RegisterServices();
            
            _progressService = allServices.GetSingle<IProgressService>();
            _readWriterService = allServices.GetSingle<IReadWriterService>();
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
            IAssetProvider assetProvider = _allServices.RegisterSingle<IAssetProvider>(
                new AssetProvider());

            IScriptableService scriptableService = _allServices.RegisterSingle<IScriptableService>(
                new ScriptableService(assetProvider));
            scriptableService.LoadAllScriptables();

            IGameFactory factory = _allServices.RegisterSingle<IGameFactory>(
                new GameFactory(_allServices , assetProvider, scriptableService));

            ICoroutineRunner coroutineRunner =
                _allServices.RegisterSingle<ICoroutineRunner>(factory.CreateCoroutineRunner());
            coroutineRunner.Init();

            IObjectPool objectPool = _allServices.RegisterSingle<IObjectPool>(factory.CreatePool());
            objectPool.Init();

            IProgressService progressService = _allServices.RegisterSingle<IProgressService>(
                new ProgressService());
            
            _allServices.RegisterSingle<IReadWriterService>(
                new ReadWriterService(progressService, factory));
            
            Camera camera = AllServices.Instance.GetSingle<IGameFactory>().CreateCamera();
            camera.GetComponent<CameraResolution>().Init(camera);

            _allServices.RegisterSingle<ISkillController>(new SkillController(progressService, objectPool,
                scriptableService, objectPool));

            //register input service
        }

    }
}