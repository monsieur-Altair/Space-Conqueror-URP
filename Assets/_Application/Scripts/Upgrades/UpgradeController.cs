using System;
using System.Collections;
using _Application.Scripts.Infrastructure.Services;
using _Application.Scripts.Infrastructure.Services.Progress;
using _Application.Scripts.Infrastructure.Services.Scriptables;
using _Application.Scripts.Managers;
using _Application.Scripts.SavedData;
using _Application.Scripts.Scriptables.Upgrades;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Application.Scripts.Upgrades
{
    public class UpgradeController : MonoBehaviour, IProgressReader, IProgressWriter
    {
        public event Action<int> TriedPurchaseUpgrade = delegate { };
        
        [SerializeField]
        private Button addButton;
        
        [SerializeField] 
        private TextMeshProUGUI costText;

        [SerializeField]
        private Image improvedBar;

        [SerializeField] 
        private int cellCount;

        [SerializeField] 
        private UpgradeType upgradeType;

        private int _numberOfCompletedCells;//starts from 0
        private int _cost;
        private UpgradeInfo _upgradeInfo;
        
        
        // on enable - read progress, update improved
        // on disable - write progress
        
        // need player progress, money, 
        

        public void Init()
        {
            _upgradeInfo = AllServices.Instance.GetSingle<IScriptableService>().GetUpgradeInfo(upgradeType);
            addButton.onClick.AddListener(ButtonClickHandler);
        }

        public void Refresh()
        {
            improvedBar.fillAmount = _numberOfCompletedCells / (float) cellCount;
            costText.text = _cost.ToString();
        }

        public void ReadProgress(PlayerProgress playerProgress)
        {
            _numberOfCompletedCells = playerProgress.GetAchievedUpgrade(upgradeType).numberOfCompletedCells;
            _cost = _upgradeInfo.GetUpgradeStats(_numberOfCompletedCells).cost;
        }

        public void WriteProgress(PlayerProgress playerProgress)
        {
            AchievedUpgrades achievedUpgrades = playerProgress.GetAchievedUpgrade(upgradeType);
            achievedUpgrades.numberOfCompletedCells = _numberOfCompletedCells;
            achievedUpgrades.upgradeCoefficient = 1.0f + GetAdditionalCoefficient();
        }

        public void ApplyPurchase()
        {
            _numberOfCompletedCells++;
            float startFill = improvedBar.fillAmount;
            float lastFill = _numberOfCompletedCells / (float) cellCount;
            
            _cost = _upgradeInfo.GetUpgradeStats(_numberOfCompletedCells).cost;
            costText.text = _cost.ToString();

            GlobalObject.Instance.StartCoroutine(PurchaseAnimation(startFill, lastFill));
        }

        private IEnumerator PurchaseAnimation(float startFill, float lastFill)
        {
            int countOfCalls = 100;
            float delta = (lastFill - startFill) / countOfCalls;
            while (countOfCalls != 0)
            {
                countOfCalls--;
                improvedBar.fillAmount += delta;
                yield return null;
            }
        }

        private void ButtonClickHandler()
        {
            if (_numberOfCompletedCells < cellCount)
                TriedPurchaseUpgrade(_cost);
        }

        private float GetAdditionalCoefficient() =>
            (_numberOfCompletedCells == 0) 
                ? 0.0f 
                : _upgradeInfo.GetUpgradeStats(_numberOfCompletedCells - 1).upgradeCoefficient;
    }
}