using System.Collections.Generic;
using _Application.Scripts.Buildings;
using _Application.Scripts.SavedData;
using Pool_And_Particles;
using UnityEngine;

namespace _Application.Scripts.Managers
{
    public class PlayerBuildingManager : MonoBehaviour
    {
        [SerializeField] private List<BuildingPoint> _buildingPoints;
        private GlobalPool _globalPool;

        public List<BaseBuilding> PlayersBuildings { get; } = new List<BaseBuilding>();
        
        public void Prepare(GlobalPool globalPool, LobbyInfo info)
        {
            _globalPool = globalPool;
            
            for (int i = 0; i< info.BoughtBuilding.Count; i++)
            {
                BuildingPoint buildingPoint = _buildingPoints[i];
                buildingPoint.SetOutlook(false);
                buildingPoint.DisableCollider();
                BuildingType buildingType = info.BoughtBuilding[i];

                if (buildingType == BuildingType.None)
                    continue;

                Vector3 position = buildingPoint.transform.position;
                PoolObjectType poolObjectType = GlobalPool.GetBuildingPrefabType(buildingType);
                Quaternion rot = Quaternion.Euler(Vector3.zero);
                BaseBuilding playerBuilding = globalPool.GetObject(poolObjectType, position, rot)
                    .GetComponent<BaseBuilding>();
                playerBuilding.SetTeam(Team.Blue);
                
                PlayersBuildings.Add(playerBuilding);
            }
        }

        public void FreeBuildings()
        {
            foreach (BaseBuilding building in PlayersBuildings)
            {
                _globalPool.Free(building);
            }
            
            PlayersBuildings.Clear();
        }
    }
}