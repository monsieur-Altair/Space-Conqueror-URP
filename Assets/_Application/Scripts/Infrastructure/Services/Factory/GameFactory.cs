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
using SkillController = _Application.Scripts.AI.SkillController;

namespace _Application.Scripts.Infrastructure.Services.Factory
{
    public class GameFactory : IGameFactory
    {
        public List<IProgressReader> ProgressReaders { get; } 
        public List<IProgressWriter> ProgressWriters { get; }

        private readonly AllServices _allServices;
        private readonly IAssetProvider _assetProvider;
        private readonly IScriptableService _scriptableService;

        public GameFactory(AllServices allServices ,IAssetProvider assetProvider, IScriptableService scriptableService)
        {
            ProgressReaders = new List<IProgressReader>();
            ProgressWriters = new List<IProgressWriter>();

            _allServices = allServices;
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
            IObjectPool objectPool = _allServices.GetSingle<IObjectPool>();
            ICoroutineRunner coroutineRunner = _allServices.GetSingle<ICoroutineRunner>();
            
            Warehouse warehouse = _assetProvider.Instantiate<Warehouse>(AssetPaths.Warehouse);

            SkillController aiSkillController = 
                new SkillController(objectPool,_scriptableService , objectPool); 
            
            Core aiManager = new Core(coroutineRunner, aiSkillController);
            
            Outlook outlookManager = new Outlook(warehouse);
            UserControl userControl = CreateUserControl();
            //Managers.UI uiManager = CreateUI(pool, warehouse, playerSkillController);
            CounterSpawner.Init(warehouse, objectPool, UISystem.GetWindow<GameplayWindow>().CounterContainer);
            
            
            Main mainManager = new Main(Levels.Instance, 
                new TeamManager(_allServices.GetSingle<IProgressService>()), 
                coroutineRunner,
                aiManager, 
                objectPool,
                outlookManager, 
                userControl,
                _allServices.GetSingle<IScriptableService>(),
                _allServices.GetSingle<IProgressService>(),
                AudioManager.Instance);
            
            ProgressReaders.Add(mainManager);
            ProgressWriters.Add(mainManager);

            return mainManager;
        }

        public IObjectPool CreatePool() => 
            _assetProvider.Instantiate<ObjectPool>(AssetPaths.PoolPath);

        public ICoroutineRunner CreateCoroutineRunner() => 
            _assetProvider.Instantiate<GlobalObject>(AssetPaths.GlobalObjectPath);

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
            userControl.Init(buildingsController, _allServices.GetSingle<ISkillController>());
            return userControl;
        }
    }
}