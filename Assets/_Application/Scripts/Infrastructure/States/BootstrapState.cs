using _Application.Scripts.Control;
using _Application.Scripts.Infrastructure.Services;
using _Application.Scripts.Infrastructure.Services.Factory;
using _Application.Scripts.Infrastructure.Services.Progress;
using _Application.Scripts.Infrastructure.Services.Scriptables;
using _Application.Scripts.Managers;
using _Application.Scripts.SavedData;
using Pool_And_Particles;
using UnityEngine;

namespace _Application.Scripts.Infrastructure.States
{
    public class BootstrapState : IState
    {
        private const string StartUp = "StartUp";
        private const string Main = "Main";
        private readonly StateMachine _stateMachine;
        private readonly SceneLoader _sceneLoader;
        private readonly ProgressService _progressService;
        private readonly ReadWriterService _readWriterService;
        private readonly CoreConfig _coreConfig;
        private GameFactory _gameFactory;
        private MonoBehaviour _monoBehaviour;

        public BootstrapState(MonoBehaviour monoBehaviour, StateMachine stateMachine, SceneLoader sceneLoader,
            CoreConfig coreConfig)
        {
            _monoBehaviour = monoBehaviour;
            _coreConfig = coreConfig;
            _stateMachine = stateMachine;
            _sceneLoader = sceneLoader;
            
            RegisterServices();
            RegisterMonoBehServices();


            _progressService = AllServices.Get<ProgressService>();
            _readWriterService = AllServices.Get<ReadWriterService>();
            
            AllServices.Register(new SkillController(_progressService, 
                AllServices.Get<GlobalPool>(), 
                AllServices.Get<ScriptableService>(), 
                AllServices.Get<GlobalCamera>()));


            _gameFactory.CreateAndRegisterMonoBeh(_coreConfig.MonoBehaviourServices.UserControl);
            _gameFactory.CreateAndRegisterMonoBeh(_coreConfig.MonoBehaviourServices.UISystem);
        }

        private void RegisterMonoBehServices()
        {
            _gameFactory = AllServices.Get<GameFactory>();
            var servicePrefabs = _coreConfig.MonoBehaviourServices;

            _gameFactory.CreateAndRegisterMonoBeh(servicePrefabs.AudioManager);
            _gameFactory.CreateAndRegisterMonoBeh(servicePrefabs.CoroutineRunner);
            _gameFactory.CreateAndRegisterMonoBeh(servicePrefabs.LevelManager);
            _gameFactory.CreateAndRegisterMonoBeh(servicePrefabs.GlobalCamera);
        }

        public void Enter()
        {
            _sceneLoader.Load(StartUp, onLoaded: ReadProgress);
        }

        public void Exit()
        {
            
        }

        private void ReadProgress() => 
            _progressService.PlayerProgress = _readWriterService.ReadProgress() ?? new PlayerProgress(-1);


        private void RegisterServices()
        {
            AllServices.Register(_coreConfig);
            RegisterPool();

            ScriptableService scriptableService = AllServices.Register(new ScriptableService(_coreConfig));
            scriptableService.LoadAllScriptables();

            GameFactory factory = AllServices.Register(new GameFactory(scriptableService, _coreConfig));

            ProgressService progressService = AllServices.Register(new ProgressService());

            AllServices.Register(new ReadWriterService(progressService, factory));
            AllServices.Register(_stateMachine);
        }

        private void RegisterPool()
        {
            GlobalPool globalPool = new GlobalPool(_coreConfig, _monoBehaviour.transform);

            foreach (var pair in _coreConfig.PoolObjects.PooledPrefabs.Pairs)
            {
                globalPool.Prepare(pair.Value.prefab, pair.Value.size);
            }

            AllServices.Register(globalPool);
        }
    }
}