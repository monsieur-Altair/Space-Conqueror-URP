using System;
using _Application.Scripts.Buildings;
using UnityEngine;
using UnityEngine.UI;

namespace _Application.Scripts.UI.Windows
{
    public class BuyBuildingCell : MonoBehaviour
    {
        [SerializeField] private BuildingType _buildingType;
        [SerializeField] private Button _buyButton;
        [SerializeField] private int _cost;
        [SerializeField] private GameObject _fade;

        public BuildingType BuildingType => _buildingType;
        public event Action<BuildingType, int> Clicked = delegate {  };
        

        public void OnOpened(bool isLocked)
        {
            if(isLocked)
                Lock();
            else
                Unlock();
        }

        public void OnClosed()
        {
            _buyButton.onClick.RemoveAllListeners();
        }
        
        private void OnClick()
        {
            Clicked(_buildingType, _cost);
        }

        private void Lock()
        {
            _fade.SetActive(true);
        }

        private void Unlock()
        {
            _fade.SetActive(false);
            
            _buyButton.onClick.AddListener(OnClick);
        }
    }
}