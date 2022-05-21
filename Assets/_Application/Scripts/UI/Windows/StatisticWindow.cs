using _Application.Scripts.Infrastructure.Services;
using _Application.Scripts.Infrastructure.Services.Progress;
using _Application.Scripts.SavedData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Application.Scripts.UI.Windows
{
    public class StatisticWindow : Window
    {
        [SerializeField] 
        private TextMeshProUGUI _gainedMana;

        [SerializeField] 
        private TextMeshProUGUI _spentMana;

        [SerializeField] 
        private TextMeshProUGUI _conqueredBuildings;

        [SerializeField] 
        private TextMeshProUGUI _missedBuildings;
        
        [SerializeField] 
        private TextMeshProUGUI _winCount;

        [SerializeField] 
        private TextMeshProUGUI _usedSpells;

        [SerializeField] 
        private Button _backToGameButton;
        
        private IProgressService _progressService;
        public Button BackToGameButton => _backToGameButton;

        public override void GetDependencies()
        {
            base.GetDependencies();

            _progressService = AllServices.Instance.GetSingle<IProgressService>();
        }

        protected override void OnOpened()
        {
            base.OnOpened();

            UpdateAllFields();
        }

        private void UpdateAllFields()
        {
            Statistic playerStatistic = _progressService.PlayerProgress.Statistic;

            _winCount.text = "wins: " + playerStatistic.WinCount;
            _gainedMana.text = "gained mana: " + playerStatistic.GainedMana;
            _spentMana.text  = "spent mana: " + playerStatistic.SpentMana;
            _usedSpells.text = "used spells: " + playerStatistic.UsedSpells;
            _missedBuildings.text    = "missed buildings: " + playerStatistic.MissedBuildings;
            _conqueredBuildings.text = "conquered \nbuildings: " + playerStatistic.ConqueredBuildings;
        }
    }
}