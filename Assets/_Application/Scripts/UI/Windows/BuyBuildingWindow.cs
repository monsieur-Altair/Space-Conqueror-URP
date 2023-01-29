using System.Collections.Generic;
using _Application.Scripts.Buildings;
using _Application.Scripts.Infrastructure.Services;
using _Application.Scripts.Infrastructure.Services.Progress;
using _Application.Scripts.Infrastructure.States;
using _Application.Scripts.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Application.Scripts.UI.Windows
{
    public class BuyBuildingWindow : PayloadedWindow<BuyBuildingWindowPayload>
    {
        [SerializeField] private TextMeshProUGUI _moneyText;
        [SerializeField] private List<BuyBuildingCell> _cells;
        [SerializeField] private Button _closeButton;
        
        private ProgressService _progressService;
        private LobbyManager _lobbyManager;

        public override void GetDependencies()
        {
            base.GetDependencies();

            _progressService = AllServices.Get<ProgressService>();
            _lobbyManager = AllServices.Get<LobbyManager>();
        }

        protected override void OnOpened()
        {
            base.OnOpened();

            UpdateMoneyText();

            foreach (BuyBuildingCell cell in _cells)
            {
                cell.OnOpened(cell.BuildingType == _payload.BuildingType || cell.Cost > _progressService.PlayerProgress.Money);
                cell.Clicked += TryBuyBuilding;
            }
            
            _closeButton.onClick.AddListener(CloseWindow);
        }

        private void CloseWindow()
        {
            Close();
            
            //_lobbyManager.SwitchBuildingsTo(true);
            UISystem.ShowWindow<LobbyWindow>();
        }

        protected override void OnClosed()
        {
            base.OnClosed();
            
            foreach (BuyBuildingCell cell in _cells)
            {
                cell.OnClosed();
                cell.Clicked -= TryBuyBuilding;
            }
            
            _closeButton.onClick.RemoveListener(CloseWindow);
        }

        private void TryBuyBuilding(BuildingType type, int cost)
        {
            if (cost > _progressService.PlayerProgress.Money)
                return;
            
            _progressService.PlayerProgress.Money -= cost;
            UpdateMoneyText();
            _lobbyManager.UpdateBuilding(type, _payload.Index);
            
            OnClosed();
            _payload.BuildingType = type;
            OnOpened();
        }

        private void UpdateMoneyText()
        {
            _moneyText.text = _progressService.PlayerProgress.Money.ToString();
        }
    }
}