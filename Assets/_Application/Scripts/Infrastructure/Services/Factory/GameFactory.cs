﻿using System.Collections.Generic;
using _Application.Scripts.Infrastructure.Services.AssetManagement;
using _Application.Scripts.Infrastructure.Services.Progress;
using _Application.Scripts.Infrastructure.Services.Scriptables;
using _Application.Scripts.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Application.Scripts.Infrastructure.Services.Factory
{
    public class GameFactory : IGameFactory
    {
        public List<IProgressReader> ProgressReaders { get; } 
        public List<IProgressWriter> ProgressWriters { get; }

        private readonly IAssetProvider _assetProvider;
        private readonly IScriptableService _scriptableService;
        private Canvas _canvas;

        public GameFactory(IAssetProvider assetProvider, IScriptableService scriptableService)
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

        public Main CreateWorld()
        {
            _canvas = GameObject.FindGameObjectWithTag("CanvasTag").GetComponent<Canvas>();
            
            ICoroutineRunner coroutineRunner = GlobalObject.Instance;
            Warehouse warehouse = _assetProvider.Instantiate<Warehouse>(AssetPaths.Warehouse);
            ObjectPool pool = _assetProvider.Instantiate<ObjectPool>(AssetPaths.PoolPath);

            AI.SkillController aiSkillController = new AI.SkillController(this,_scriptableService , pool);
            AI.Core aiManager = new AI.Core(coroutineRunner, aiSkillController);
            
            Outlook outlookManager = new Outlook(warehouse);
            Control.UserControl userControl = CreateUserControl(pool, _scriptableService , out var playerSkillController);
            UI uiManager = CreateUI(pool, warehouse, playerSkillController);

            Main mainManager = new Main(Levels.Instance, 
                new TeamManager(), 
                coroutineRunner,
                aiManager, 
                pool,
                outlookManager, 
                uiManager, 
                userControl,
                AllServices.Instance.GetSingle<IReadWriterService>(),
                AllServices.Instance.GetSingle<IScriptableService>());
            
            ProgressReaders.Add(mainManager);
            ProgressWriters.Add(mainManager);

            return mainManager;
        }

        public GameObject CreateAcid() => 
            _assetProvider.Instantiate(AssetPaths.AcidPrefabPath);

        public GameObject CreateIndicator() =>  
            _assetProvider.Instantiate(AssetPaths.IndicatorPrefabPath);

        public GameObject CreateIce() => 
            _assetProvider.Instantiate(AssetPaths.IcePrefabPath);

        private UI CreateUI(ObjectPool pool, Warehouse warehouse, Control.SkillController skillController)
        {
            UI uiManager = new UI(_canvas, pool, warehouse, skillController);
            uiManager.SetButtons(CreateSkillButtons(),CreateRetryButton(), CreateNextLevelButton());
            uiManager.SetBars(CreateScientificBar(), CreateTeamBar());
            uiManager.SetText(CreateMoneyText());
            return uiManager;
        }

        private Control.UserControl CreateUserControl(ObjectPool pool, IScriptableService scriptableService, out Control.SkillController skillController)
        {
            Control.UserControl userControl = _assetProvider.Instantiate<Control.UserControl>(AssetPaths.UserControlPath);
            skillController = new Control.SkillController(this, scriptableService, pool);
            Control.PlanetController planetController = new Control.PlanetController(Camera.main);
            userControl.Init(planetController, skillController);
            return userControl;
        }

        private TextMeshProUGUI CreateMoneyText() => 
            _assetProvider.InstantiateUI<TextMeshProUGUI>(AssetPaths.MoneyTextPath, _canvas);

        private List<Button> CreateSkillButtons()
        {
            List<Button> buttons = new List<Button>();

            Button first = _assetProvider.InstantiateUI<Button>(AssetPaths.FirstButtonPath, _canvas);
            Button second = _assetProvider.InstantiateUI<Button>(AssetPaths.SecondButtonPath, _canvas);
            Button third = _assetProvider.InstantiateUI<Button>(AssetPaths.ThirdButtonPath, _canvas);
            Button forth = _assetProvider.InstantiateUI<Button>(AssetPaths.ForthButtonPath, _canvas);
            
            buttons.Add(first);
            buttons.Add(second);
            buttons.Add(third);
            buttons.Add(forth);

            return buttons;
        }

        private GameObject CreateScientificBar() => 
            _assetProvider.InstantiateUI(AssetPaths.ScientificBarPath, _canvas);

        private GameObject CreateTeamBar() => 
            _assetProvider.InstantiateUI(AssetPaths.TeamBarPath, _canvas);

        private GameObject CreateNextLevelButton() => 
            _assetProvider.InstantiateUI(AssetPaths.NextLevelButtonsPath, _canvas);

        private GameObject CreateRetryButton() => 
            _assetProvider.InstantiateUI(AssetPaths.RetryButtonsPath, _canvas);
    }
}