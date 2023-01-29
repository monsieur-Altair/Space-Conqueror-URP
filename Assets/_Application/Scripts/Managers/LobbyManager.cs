using System;
using System.Collections.Generic;
using System.Linq;
using _Application.Scripts.Buildings;
using _Application.Scripts.Infrastructure.Services;
using _Application.Scripts.Infrastructure.Services.Factory;
using _Application.Scripts.Infrastructure.Services.Progress;
using _Application.Scripts.Infrastructure.Services.Scriptables;
using _Application.Scripts.SavedData;
using _Application.Scripts.UI;
using _Application.Scripts.UI.Windows;
using Pool_And_Particles;
using UnityEngine;

namespace _Application.Scripts.Managers
{
    

    public class BuildingInfo
    {
        public BuildingType BuildingType;
        public BaseBuilding BaseBuilding;
        public Transform Point;
    }
    
    public class LobbyManager : MonoBehaviourService, IProgressWriter
    {
        public const int MaxPlaceCount = 5;
        
        public event Action<BuildingType, int> PointClicked = delegate {  };
        
        [SerializeField] private List<BuildingPoint> _buildingPoints;
        
        private GlobalPool _globalPool;

        private Dictionary<BuildingType, BaseBuilding> _buildingPrefabs;
        private List<BuildingInfo> _buildingInfos;
        private GameFactory _gameFactory;
        private ProgressService _progressService;
        private OutlookService _outlookService;
        private ScriptableService _scriptableService;
        private CounterSpawner _counterSpawner;
        private LobbyInfo LobbyInfo => _progressService.PlayerProgress.LobbyInfo;

        public override void Init()
        {
            base.Init();

            _globalPool = AllServices.Get<GlobalPool>();
            _progressService = AllServices.Get<ProgressService>();
            _buildingInfos = new List<BuildingInfo>();
            _gameFactory = AllServices.Get<GameFactory>();
            _outlookService = AllServices.Get<OutlookService>();
            _scriptableService = AllServices.Get<ScriptableService>();

            _gameFactory.ProgressWriters.Add(this);

            _buildingPrefabs = new Dictionary<BuildingType, BaseBuilding>
            {
                {BuildingType.Altar, GetBuilding(BuildingType.Altar)},
                {BuildingType.Attacker, GetBuilding(BuildingType.Attacker)},
                {BuildingType.Spawner, GetBuilding(BuildingType.Spawner)}
            };
            
            for (int i = 0; i < MaxPlaceCount; i++)
            {
                _buildingInfos.Add(new BuildingInfo()
                {
                    BuildingType = LobbyInfo.BoughtBuilding[i],
                    BaseBuilding = null,
                    Point = _buildingPoints[i].transform
                });
            }
        }
        
        public void UpdateBuilding(BuildingType newType, int index)
        {
            if (_buildingInfos[index].BuildingType != BuildingType.None)
            {
                _counterSpawner.FreeCounter(_buildingInfos[index].BaseBuilding);
                DeleteBuilding(index);
            }
            
            CreateBuilding(newType, index);
            _counterSpawner.SetCounter(_buildingInfos[index].BaseBuilding);
            
            UpdateLobbyInfo();
        }

        public void OnEnter(CounterSpawner counterSpawner)
        {
            _counterSpawner = counterSpawner;
            gameObject.SetActive(true);
            
            for (int i = 0; i < MaxPlaceCount; i++) 
                CreateBuilding(_buildingInfos[i].BuildingType, i);

            foreach (BuildingPoint point in _buildingPoints) 
                point.Clicked += OnPointClicked;
            
            Transform counterParent = UISystem.GetWindow<LobbyWindow>().CounterParent;
            _counterSpawner.FillLists(_buildingInfos.Select(info=> info.BaseBuilding).ToList(), counterParent);
        }

        public void OnExit()
        {
            for (int i = 0; i < MaxPlaceCount; i++) 
                DeleteBuilding(i);
            
            foreach (BuildingPoint point in _buildingPoints) 
                point.Clicked -= OnPointClicked;
            
            gameObject.SetActive(false);
            
            _counterSpawner.ClearLists();
        }

        public void WriteProgress(PlayerProgress playerProgress)
        {
            UpdateLobbyInfo();
        }

        private void UpdateLobbyInfo()
        {
            for (int i = 0; i < MaxPlaceCount; i++)
            {
                LobbyInfo.BoughtBuilding[i] = _buildingInfos[i].BuildingType;
            }
        }

        // public void SwitchBuildingsTo(bool isActive)
        // {
        //     foreach (BuildingInfo info in _buildingInfos) 
        //         info.BaseBuilding.gameObject.SetActive(isActive);
        // }

        private void OnPointClicked(BuildingPoint point)
        {
            int index = _buildingPoints.IndexOf(point);
            PointClicked(_buildingInfos[index].BuildingType, index);
        }

        private void CreateBuilding(BuildingType buildingType, int index)
        {
            BuildingInfo buildingInfo = _buildingInfos[index];
            buildingInfo.BuildingType = buildingType;
            
            _buildingPoints[index].SetOutlook(true);
            
            if(buildingType == BuildingType.None)
                return;

            _buildingPoints[index].SetOutlook(false);

            BaseBuilding building = _globalPool.Get(_buildingPrefabs[buildingType]);
            
            Transform buildingPoint = buildingInfo.Point;
            building.transform.position = buildingPoint.position;
            building.transform.rotation = buildingPoint.rotation;
            
            buildingInfo.BaseBuilding = building;
            building.DisableCollider();
            _outlookService.SetOutlook(building, (int)Team.Blue);

            float maxCount = _scriptableService.GetBuildingInfo(Team.Blue, buildingType).maxCount;
            float buildingMaxCountCoefficient = _progressService.PlayerProgress
                .GetAchievedUpgrade(UpgradeType.BuildingMaxCount).upgradeCoefficient;
            building.SetCount(maxCount * buildingMaxCountCoefficient);
            building.SetTeam(Team.Blue);
        }

        private void DeleteBuilding(int index)
        {
            if (_buildingInfos[index].BaseBuilding != null)
                _globalPool.Free(_buildingInfos[index].BaseBuilding);
            _buildingInfos[index].BaseBuilding = null;
        }

        private BaseBuilding GetBuilding(BuildingType type)
        {
            return _globalPool.GetPooledBehaviourPrefab(GlobalPool.GetBuildingPrefabType(type))
                .GetComponent<BaseBuilding>();
        }
    }
}