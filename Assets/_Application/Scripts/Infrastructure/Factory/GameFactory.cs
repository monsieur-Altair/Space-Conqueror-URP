using System.Collections.Generic;
using _Application.Scripts.Control;
using _Application.Scripts.Infrastructure.AssetManagement;
using _Application.Scripts.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace _Application.Scripts.Infrastructure.Factory
{
    public class GameFactory : IGameFactory
    {
        private readonly IAssetProvider _assetProvider;
        private Canvas _canvas;

        public GameFactory(IAssetProvider assetProvider) => 
            _assetProvider = assetProvider;

        public void CreateWorld()
        {
            _canvas = GameObject.FindGameObjectWithTag("CanvasTag").GetComponent<Canvas>();
            
            List<Button> skillButtons = CreateSkillButtons();
            GameObject retryButton = CreateRetryButton();
            GameObject nextLevelButton = CreateNextLevelButton();
            GameObject scientificBar = CreateScientificBar();
            GameObject teamBar = CreateTeamBar();

            Warehouse warehouse = _assetProvider.Instantiate<Warehouse>(AssetPaths.Warehouse);
            ObjectPool pool = _assetProvider.Instantiate<ObjectPool>(AssetPaths.PoolPath);
            GameObject aiManager = _assetProvider.Instantiate(AssetPaths.AIPath);
            
            UI uiManager = new UI(_canvas,pool,warehouse);
            Outlook outlookManager = new Outlook(warehouse);
            uiManager.SetButtons(retryButton, nextLevelButton);
            uiManager.SetBars(scientificBar, teamBar);
            
            UserControl userControl = _assetProvider.Instantiate<UserControl>(AssetPaths.UserControlPath);
            SkillController skillController = userControl.GetComponent<SkillController>();
            skillController.AdjustSkillButtons(skillButtons);

            Main mainManager = _assetProvider.Instantiate<Main>(AssetPaths.MainManagerPath);
            mainManager.Construct(outlookManager, uiManager, userControl);
            mainManager.StartGame();
        }

        public GameObject CreateAcid() => 
            _assetProvider.Instantiate(AssetPaths.AcidPrefabPath);

        public GameObject CreateIndicator() =>  
            _assetProvider.Instantiate(AssetPaths.IndicatorPrefabPath);

        public GameObject CreateIce() => 
            _assetProvider.Instantiate(AssetPaths.IcePrefabPath);

        public Scriptables.Skill CreateSkillResource(string path) => 
            _assetProvider.InstantiateScriptable<Scriptables.Skill>(path);

        private GameObject CreateScientificBar() => 
            _assetProvider.InstantiateUI(AssetPaths.ScientificBarPath, _canvas);

        private GameObject CreateTeamBar() => 
            _assetProvider.InstantiateUI(AssetPaths.TeamBarPath, _canvas);

        private GameObject CreateNextLevelButton() => 
            _assetProvider.InstantiateUI(AssetPaths.NextLevelButtonsPath, _canvas);

        private GameObject CreateRetryButton() => 
            _assetProvider.InstantiateUI(AssetPaths.RetryButtonsPath, _canvas);

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
    }
}