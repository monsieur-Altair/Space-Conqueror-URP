using System.Collections.Generic;
using _Application.Scripts.AI;
using _Application.Scripts.Control;
using _Application.Scripts.Infrastructure.Services.AssetManagement;
using _Application.Scripts.Infrastructure.Services.Progress;
using _Application.Scripts.Infrastructure.Services.Scriptables;
using _Application.Scripts.Managers;
using _Application.Scripts.UI;
using _Application.Scripts.UI.Windows;
using UnityEngine;

namespace _Application.Scripts.Infrastructure.Services.Factory
{
    public class GameFactory : IService
    {
        public List<IProgressReader> ProgressReaders { get; } 
        public List<IProgressWriter> ProgressWriters { get; }

        private readonly AssetProvider _assetProvider;
        private readonly ScriptableService _scriptableService;

        public GameFactory(AssetProvider assetProvider, ScriptableService scriptableService)
        {
            ProgressReaders = new List<IProgressReader>();
            ProgressWriters = new List<IProgressWriter>();

            _assetProvider = assetProvider;
            _scriptableService = scriptableService;
        }

        public void CleanUp()
        {
            ProgressReaders.Clear();
            ProgressWriters.Clear();
        }

        public UISystem CreateUISystem() => 
            _assetProvider.Instantiate<UISystem>(AssetPaths.UISystemPath);

        public Main CreateWorld()
        {
            ObjectPool objectPool = AllServices.Get<ObjectPool>();
            CoroutineRunner coroutineRunner = AllServices.Get<CoroutineRunner>();
            
            Warehouse warehouse = _assetProvider.Instantiate<Warehouse>(AssetPaths.Warehouse);

            AISkillController aiAISkillController = 
                new AISkillController(objectPool,_scriptableService , objectPool); 
            
            Core aiManager = new Core(coroutineRunner, aiAISkillController);
            
            Outlook outlookManager = new Outlook(warehouse);
            UserControl userControl = CreateUserControl();
            //Managers.UI uiManager = CreateUI(pool, warehouse, playerSkillController);
            CounterSpawner.Init(warehouse, objectPool, UISystem.GetWindow<GameplayWindow>().CounterContainer);
            
            
            Main mainManager = new Main(AllServices.Get<LevelManager>(), 
                new TeamManager(AllServices.Get<IProgressService>()), 
                coroutineRunner,
                aiManager, 
                objectPool,
                outlookManager, 
                userControl,
                AllServices.Get<ScriptableService>(),
                AllServices.Get<IProgressService>(),
                AllServices.Get<AudioManager>());
            
            ProgressReaders.Add(mainManager);
            ProgressWriters.Add(mainManager);

            return mainManager;
        }

        public T CreateMonoBeh<T>(T prefab) where T : MonoBehaviourService
        {
            T service = Object.Instantiate(prefab);
            service.Init();
            return service;
        }

        public Camera CreateCamera() => 
            _assetProvider.Instantiate<Camera>(AssetPaths.MainCameraPath);

        public GameObject CreateAcid() => 
            _assetProvider.Instantiate(AssetPaths.AcidPrefabPath);

        public GameObject CreateIndicator() =>
            _assetProvider.Instantiate(AssetPaths.IndicatorPrefabPath);

        public GameObject CreateIce() => 
            _assetProvider.Instantiate(AssetPaths.IcePrefabPath);

        private UserControl CreateUserControl()
        {
            UserControl userControl = _assetProvider.Instantiate<UserControl>(AssetPaths.UserControlPath);
            BuildingsController buildingsController = new BuildingsController(Camera.main);
            userControl.Init(buildingsController, AllServices.Get<SkillController>());
            return userControl;
        }
    }
}