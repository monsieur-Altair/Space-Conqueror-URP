using _Application.Scripts.Infrastructure.Services;
using _Application.Scripts.Infrastructure.Services.Progress;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Application.Scripts.UI.Windows
{
    [RequireComponent(typeof(GraphicRaycaster))]
    public class LoseWindow : Window
    {
        [SerializeField] 
        private Button _restartLevelButton;
        
        [SerializeField] 
        private Button _toUpgradeButton;

        [SerializeField]
        private TextMeshProUGUI _moneyText;

        private IProgressService _progressService;

        public Button RestartButton => _restartLevelButton;
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