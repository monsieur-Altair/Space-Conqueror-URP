using System.Collections.Generic;
using _Application.Scripts.Control;
using _Application.Scripts.Infrastructure.AssetManagement;
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
            
            List<Button> createdButtons = CreateSkillButtons();
            GameObject retryButton = CreateRetryButton();
            GameObject nextLevelButton = CreateNextLevelButton();
            GameObject scientificBar = CreateScientificBar();
            GameObject teamBar = CreateTeamBar();
            
            GameObject pool = _assetProvider.Instantiate(AssetPaths.PoolPath);
            GameObject outlookManager = _assetProvider.Instantiate(AssetPaths.OutlookPath);
            GameObject aiManager = _assetProvider.Instantiate(AssetPaths.AIPath);
            Managers.UI uiManager = _assetProvider.Instantiate(AssetPaths.UIPath).GetComponent<Managers.UI>();
            
            uiManager.SetButtons(retryButton, nextLevelButton);
            uiManager.SetBars(scientificBar, teamBar);
            
            GameObject userControl = _assetProvider.Instantiate(AssetPaths.UserControlPath);
            SkillController skillController = userControl.GetComponent<SkillController>();
            skillController.AdjustSkillButtons(createdButtons);

            Managers.Main mainManager = _assetProvider.Instantiate(AssetPaths.MainManagerPath).GetComponent<Managers.Main>();
            
            mainManager.StartGame();
        }

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

            Button first = _assetProvider.InstantiateUI(AssetPaths.FirstButtonPath, _canvas).GetComponent<Button>();
            Button second = _assetProvider.InstantiateUI(AssetPaths.SecondButtonPath, _canvas).GetComponent<Button>();
            Button third = _assetProvider.InstantiateUI(AssetPaths.ThirdButtonPath, _canvas).GetComponent<Button>();
            Button forth = _assetProvider.InstantiateUI(AssetPaths.ForthButtonPath, _canvas).GetComponent<Button>();
            
            buttons.Add(first);
            buttons.Add(second);
            buttons.Add(third);
            buttons.Add(forth);

            return buttons;
        }
    }
}