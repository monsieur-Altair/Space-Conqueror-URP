using System.Collections.Generic;
using _Application.Scripts.Infrastructure.Services;
using _Application.Scripts.Infrastructure.Services.AssetManagement;
using _Application.Scripts.Infrastructure.Services.Progress;
using _Application.Scripts.Managers;
using _Application.Scripts.Misc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Application.Scripts.Upgrades
{
    public class UpgradeMenuController : MonoBehaviour
    {
        [SerializeField] 
        private UpgradeController[] upgradeControllers;

        public Button backButton;
        
        [SerializeField] 
        private TextMeshProUGUI moneyText;
        private int _money;
        
        private IProgressService _progressService;
        
        private readonly List<IProgressReader> _progressReaders = new List<IProgressReader>();
        private readonly List<IProgressWriter> _progressWriters = new List<IProgressWriter>();
        private IReadWriterService _readWriterService;

        private void Awake()
        {
            _readWriterService = AllServices.Instance.GetSingle<IReadWriterService>();

            foreach (UpgradeController upgradeController in upgradeControllers)
            {
                _progressReaders.Add(upgradeController);
                _progressWriters.Add(upgradeController);
                upgradeController.TriedPurchaseUpgrade += (cost) =>
                {
                    if ((_money - cost) >= 0)
                    {
                        _money -= cost;
                        AllServices.Instance.GetSingle<IProgressService>().PlayerProgress.money = _money;//////////////////////////////
                        moneyText.text = $"money: {_money}";
                        upgradeController.ApplyPurchase();
                    }
                };

                upgradeController.Init();
                //add to lists in game factory
            }

            _progressService = AllServices.Instance.GetSingle<IProgressService>();
        }

        private void OnEnable()
        {
            foreach (IProgressReader progressReader in _progressReaders)
                progressReader.ReadProgress(_progressService.PlayerProgress);

            foreach (UpgradeController upgradeController in upgradeControllers)
            {
                //upgradeController.gameObject.SetActive(true);
                upgradeController.Refresh();
            }

            _money = _progressService.PlayerProgress.money;//ошибка, старое значение
            moneyText.text = $"money: {_money}";
        }

        private void OnDisable()
        {
            foreach (IProgressWriter progressWriter in _progressWriters)
                progressWriter.WriteProgress(_progressService.PlayerProgress);
//вызывается ли при выходе из игры??????
            _progressService.PlayerProgress.money = _money;
            
            GlobalObject.Instance.StopAllCoroutines();
        }
        
        private void OnDestroy()
        {
            _readWriterService.WriteProgress();
            //PlayerPrefs.SetString("PlayerProgress", _progressService.PlayerProgress.ConvertToJson());//////////////////
        }
    }
}