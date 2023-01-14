using _Application.Scripts.Infrastructure.Services;
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

        //[SerializeField] 
        //private Button _toStatistic;
        
        [SerializeField] 
        private TextMeshProUGUI _moneyText;

        private IProgressService _progressService;

        public Button NextLevelButton => _nextLevelButton;
        //public Button ToStatisticButton => _toStatistic;
        public Button ToUpgradeMenuButton => _toUpgradeButton;
        
        public override void GetDependencies()
        {
            base.GetDependencies();
            
            _progressService = AllServices.Get<IProgressService>();
        }

        protected override void OnOpened()
        {
            base.OnOpened();
            
            _moneyText.text = _progressService.PlayerProgress.Money.ToString();
        }
    }
}