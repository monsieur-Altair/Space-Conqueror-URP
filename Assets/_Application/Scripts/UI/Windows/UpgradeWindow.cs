using System.Collections.Generic;
using _Application.Scripts.Infrastructure;
using _Application.Scripts.Infrastructure.Services;
using _Application.Scripts.Infrastructure.Services.Progress;
using _Application.Scripts.Infrastructure.Services.Scriptables;
using _Application.Scripts.Upgrades;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Application.Scripts.UI.Windows
{
    [RequireComponent(typeof(GraphicRaycaster))]
    public class UpgradeWindow : Window
    {
        [SerializeField] 
        private UpgradeController[] upgradeControllers;

        [SerializeField]
        private Button backButton;
        
        [SerializeField] 
        private TextMeshProUGUI moneyText;
        private int _money;
        
        private IProgressService _progressService;
        private IScriptableService _scriptableService;
        private ICoroutineRunner _coroutineRunner;

        private readonly List<IProgressReader> _progressReaders = new List<IProgressReader>();
        private readonly List<IProgressWriter> _progressWriters = new List<IProgressWriter>();

        public Button BackToGameButton => backButton;
        
        public override void GetDependencies()
        {
            _progressService = AllServices.Instance.GetSingle<IProgressService>();
            _scriptableService = AllServices.Instance.GetSingle<IScriptableService>();
            _coroutineRunner = AllServices.Instance.GetSingle<ICoroutineRunner>();
            
            foreach (UpgradeController upgradeController in upgradeControllers) 
                InitController(upgradeController);
        }

        protected override void OnOpened()
        {
            base.OnOpened();
            
            foreach (IProgressReader progressReader in _progressReaders)
                progressReader.ReadProgress(_progressService.PlayerProgress);

            foreach (UpgradeController upgradeController in upgradeControllers) 
                upgradeController.Refresh();

            _money = _progressService.PlayerProgress.money;
            moneyText.text = $"money: {_money}";
        }

        protected override void OnClosed()
        {
            base.OnClosed();
            
            foreach (IProgressWriter progressWriter in _progressWriters)
                progressWriter.WriteProgress(_progressService.PlayerProgress);

            _progressService.PlayerProgress.money = _money;
            
            _coroutineRunner.StopAllCoroutines();
        }

        private void OnDestroy()
        {  
            foreach (UpgradeController upgradeController in upgradeControllers)
                upgradeController.TriedPurchaseUpgrade -= UpgradeController_TriedPurchaseUpgrade;
        }


        private void InitController(UpgradeController upgradeController)
        {
            _progressReaders.Add(upgradeController);
            _progressWriters.Add(upgradeController);

            upgradeController.TriedPurchaseUpgrade += UpgradeController_TriedPurchaseUpgrade;
            upgradeController.Init(_scriptableService, _coroutineRunner);
            //add to lists in game factory
        }

        private void UpgradeController_TriedPurchaseUpgrade(UpgradeController upgradeController, int cost)
        {
            if (_money >= cost)
            {
                _money -= cost;
                _progressService.PlayerProgress.money = _money; 
                moneyText.text = $"money: {_money}";
                upgradeController.ApplyPurchase();
                upgradeController.WriteProgress(_progressService.PlayerProgress);
            }
        }
    }
}