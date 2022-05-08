﻿using _Application.Scripts.Infrastructure.Services;
using _Application.Scripts.Infrastructure.Services.Progress;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Application.Scripts.UI.Windows
{
    [RequireComponent(typeof(GraphicRaycaster))]
    public class WinWindow : Window
    {
        [SerializeField] 
        private Button _nextLevelButton;
        
        [SerializeField] 
        private Button _toUpgradeButton;

        [SerializeField] 
        private TextMeshProUGUI _moneyText;

        private IProgressService _progressService;

        public Button NextLevelButton => _nextLevelButton;
        public Button ToUpgradeMenuButton => _toUpgradeButton;
        
        public override void GetDependencies()
        {
            base.GetDependencies();
            
            _progressService = AllServices.Instance.GetSingle<IProgressService>();
        }

        protected override void OnOpened()
        {
            base.OnOpened();
            
            _moneyText.text = $"money: {_progressService.PlayerProgress.money}";
        }
    }
}