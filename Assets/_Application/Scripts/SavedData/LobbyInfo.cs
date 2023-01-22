using System;
using System.Collections.Generic;
using _Application.Scripts.Buildings;
using _Application.Scripts.Managers;
using UnityEngine;

namespace _Application.Scripts.SavedData
{
    [Serializable]
    public class LobbyInfo
    {
        [SerializeField] private List<BuildingType> _boughtBuilding;

        public List<BuildingType> BoughtBuilding => _boughtBuilding;

        public LobbyInfo()
        {
            _boughtBuilding = new List<BuildingType>();
            for (int i = 0; i < LobbyManager.MaxPlaceCount; i++) 
                _boughtBuilding.Add(BuildingType.None);
        }
    }
}