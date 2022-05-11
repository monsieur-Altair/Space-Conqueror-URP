#nullable enable
using System;
using System.Collections;
using _Application.Scripts.Infrastructure;
using _Application.Scripts.Infrastructure.Services.Progress;
using _Application.Scripts.Infrastructure.Services.Scriptables;
using _Application.Scripts.SavedData;
using _Application.Scripts.Scriptables.Upgrades;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#pragma warning disable 8618
#pragma warning disable 8629

namespace _Application.Scripts.Upgrades
{
    public class UpgradeController : MonoBehaviour, IProgressReader, IProgressWriter
    {
        public event Action<UpgradeController ,int> TriedPurchaseUpgrade = delegate { };
        
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

        private int _numberOfCompletedCells;
        private int _cost;
        private UpgradeInfo _upgradeInfo;
        private ICoroutineRunner _coroutineRunner;

        public void Init(IScriptableService scriptableService, ICoroutineRunner coroutineRunner)
        {
            _upgradeInfo = scriptableService.GetUpgradeInfo(upgradeType);
            _coroutineRunner = coroutineRunner;
            addButton.onClick.AddListener(ButtonClickHandler);
        }

        public void Refresh()
        {
            improvedBar.fillAmount = _numberOfCompletedCells / (float) cellCount;

            if(_cost<0)
                addButton.gameObject.SetActive(false);
            else
                costText.text = _cost.ToString();
        }

        public void ReadProgress(PlayerProgress playerProgress)
        {
            AchievedUpgrades achievedUpgrades = playerProgress.GetAchievedUpgrade(upgradeType);
            _numberOfCompletedCells = achievedUpgrades.numberOfCompletedCells;
            SingleUpgradeStats? stats = _upgradeInfo.GetUpgradeStats(_numberOfCompletedCells);
            _cost = stats?.cost ?? -1;
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

            SingleUpgradeStats? stats = _upgradeInfo.GetUpgradeStats(_numberOfCompletedCells);
            
            if (stats != null)
            {
                _cost = (int) (stats?.cost);
                costText.text = _cost.ToString();
            }
            else
            {
                addButton.gameObject.SetActive(false);
            }
            

            _coroutineRunner.StartCoroutine(PurchaseAnimation(startFill, lastFill));
        }

        private IEnumerator PurchaseAnimation(float startFill, float lastFill)
        {
            int countOfCalls = 20;
            float delta = (lastFill - startFill) / countOfCalls;
            while (countOfCalls != 0)
            {
                countOfCalls--;
                improvedBar.fillAmount += delta;
                yield return new WaitForSeconds(0.025f);
            }
            //////////////////////////
        }

        private void ButtonClickHandler()
        {
            if (_numberOfCompletedCells < cellCount)
                TriedPurchaseUpgrade(this ,_cost);
        }

        private float GetAdditionalCoefficient()
        {
            if (_numberOfCompletedCells == 0)
                return 0.0f;
            
            SingleUpgradeStats? stats = _upgradeInfo.GetUpgradeStats(_numberOfCompletedCells - 1);
            return stats?.upgradeCoefficient ?? 0.0f;
        }
    }
}