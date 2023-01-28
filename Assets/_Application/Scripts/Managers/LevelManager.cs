using System.Collections;
using System.Collections.Generic;
using _Application.Scripts.Buildings;
using _Application.Scripts.Infrastructure.Services;
using _Application.Scripts.Infrastructure.Services.Progress;
using Pool_And_Particles;
using UnityEngine;
using UnityEngine.AI;

namespace _Application.Scripts.Managers
{
    public class LevelManager : MonoBehaviourService
    {
        [SerializeField] 
        private Level[] _levels;

        private readonly Vector3 _instantiatePos = new Vector3(0, 0, 0);
        private Level _currentLevel;
        private GlobalPool _globalPool;
        private ProgressService _progressService;

        public int CurrentLevelNumber { get;  set; }

        public override void Init()
        {
            base.Init();

            _globalPool = AllServices.Get<GlobalPool>();
            _progressService = AllServices.Get<ProgressService>();
        }

        public void SwitchToNextLevel()
        {
            StartCoroutine(DeleteLevel());
            CurrentLevelNumber++;
        }

        public void RestartLevel()
        {
            DeleteCurrentLevel();
        }

        public List<BaseBuilding> GetCurrentBuildings() => 
            _currentLevel.GetAllBuildings();
        
        public IEnumerator CreateLevel()
        {
            if (CurrentLevelNumber >= _levels.Length)
                CurrentLevelNumber = 0;

            yield return StartCoroutine(DeleteLevel());

            _currentLevel = Instantiate(_levels[CurrentLevelNumber], _instantiatePos, Quaternion.identity);
            _currentLevel.Prepare(_globalPool, _progressService.PlayerProgress.LobbyInfo);
            _currentLevel.gameObject.SetActive(true);
            _currentLevel.transform.SetParent(transform.parent);
        }

        public void DeleteCurrentLevel()
        {
            StartCoroutine(DeleteLevel());
        }

        private IEnumerator DeleteLevel()
        {
            if (_currentLevel != null)
            {
                _currentLevel.gameObject.SetActive(false);
                Destroy(_currentLevel.gameObject);
            }
            
            yield break;
        }
    }
}