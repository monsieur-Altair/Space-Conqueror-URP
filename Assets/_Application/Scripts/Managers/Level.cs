using System;
using System.Collections.Generic;
using _Application.Scripts.Buildings;
using _Application.Scripts.SavedData;
using Pool_And_Particles;
using UnityEngine;

namespace _Application.Scripts.Managers
{
    public class Level : MonoBehaviour
    {
        [SerializeField] private PlayerBuildingManager _playerBuildingManager;
        [SerializeField] private List<BaseBuilding> _allBuildings;

        public void Prepare(GlobalPool globalPool, LobbyInfo info)
        {
            _playerBuildingManager.Prepare(globalPool, info);
        }

        public List<BaseBuilding> GetAllBuildings()
        {
            List<BaseBuilding> all = new List<BaseBuilding>(_allBuildings);
            all.AddRange(_playerBuildingManager.PlayersBuildings);
            return all;
        }

        public void OnDestroy()
        {
            _playerBuildingManager.FreeBuildings();
        }
    }
}