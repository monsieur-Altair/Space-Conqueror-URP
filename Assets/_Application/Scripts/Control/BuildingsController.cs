using System.Collections.Generic;
using System.Linq;
using _Application.Scripts.Managers;
using _Application.Scripts.UI;
using Pool_And_Particles;
using UnityEngine;

namespace _Application.Scripts.Control
{
    public class BuildingsController
    {
        private readonly List<Buildings.BaseBuilding> _selectableBuildings;
        private readonly Camera _mainCamera;
        private readonly WorldArrowManager _worldArrowManager;
        private readonly List<Color> _circleColors;

        public BuildingsController(Camera camera, CoreConfig coreConfig, GlobalPool globalPool)
        {
            _mainCamera = camera;
            _selectableBuildings = new List<Buildings.BaseBuilding>();

            _circleColors = coreConfig.Warehouse.circleColors;
            PooledBehaviour prefab = coreConfig.PoolObjects.PooledPrefabs.GetValue(PoolObjectType.WorldArrow).prefab;
            _worldArrowManager = new WorldArrowManager(globalPool, prefab.GetComponent<WorldArrow>(), _mainCamera);
        }

        public void HandleClick(Vector3 pos)
        {
            Buildings.BaseBuilding building = RaycastForBuilding(pos);
            if (building == null) 
                return;
            
            if (_selectableBuildings.Contains(building) == false && building.Team == Buildings.Team.Blue)
                Select(building);
        }

        public void HandleRelease(Vector3 pos)
        {
            if (RaycastForBuilding(pos) == null)
            {
                ClearAllSelection();
                return;
            }
        
            int count = _selectableBuildings.Count;
            if (count > 1)
            {
                StartLaunching(count);
                ClearAllSelection();
            }
        }

        public void HandleMultipleSelection(Vector3 screenPointPos)
        {
            Buildings.BaseBuilding building = RaycastForBuilding(screenPointPos);
            _worldArrowManager.UpdateAll(screenPointPos);

            if (building == null)
            {
                return;
            }
            //1 - empty: only blue,
            //2 - not empty: if user has selected blue, check container + if the last isn't blue, delete them 

            Buildings.Team team = building.Team;
            int count = _selectableBuildings.Count;
            if (team == Buildings.Team.Blue)
            {
                if(count==0)
                    Select(building);
                else
                    MoveAllyToTheEnd(building);
            }
            else
            {
                if (count > 0) 
                    MoveEnemyToTheEnd(building, count);
            }
        }
        
        private void CancelSelection(Buildings.BaseBuilding building)
        {
            //DecreaseScale(building);
            
            if (building.Team == Buildings.Team.Blue) 
                _worldArrowManager.DisableArrow(building.transform);
            
            building.Deselect();
        }

        private void Select(Buildings.BaseBuilding building)
        {
            AddToList(building);
            building.Select(_circleColors[(int) building.Team]);

            if (building.Team == Buildings.Team.Blue) 
                _worldArrowManager.AddArrow(building.transform);

            //IncreaseScale(building);
        }

        private void StartLaunching(int count)
        {
            Buildings.BaseBuilding destination = _selectableBuildings[count - 1];
            _selectableBuildings.Remove(destination);
            CancelSelection(destination);
            LaunchToDestination(destination);
        }

        public void ClearAllSelection()
        {
            foreach (Buildings.BaseBuilding building in _selectableBuildings)
                CancelSelection(building);
            
            _selectableBuildings.Clear();
            _worldArrowManager.DisableAll();
        }

        private Buildings.BaseBuilding RaycastForBuilding(Vector3 pos)
        {
            int layerMask = 1 << 0;
            layerMask = ~layerMask;
            Ray ray = _mainCamera.ScreenPointToRay(pos);
            return Physics.Raycast(ray, out RaycastHit hit,Mathf.Infinity, layerMask) 
                ? hit.collider.GetComponent<Buildings.BaseBuilding>() : null;
        }

        private void MoveEnemyToTheEnd(Buildings.BaseBuilding building, int count)
        {
            Buildings.BaseBuilding lastBuilding = _selectableBuildings.Last();
            if (building == lastBuilding)
                return;

            if (lastBuilding.Team != Buildings.Team.Blue)
            {
                CancelSelection(lastBuilding);
                _selectableBuildings.RemoveAt(count - 1);
            }

            Select(building);
        }

        private void MoveAllyToTheEnd(Buildings.BaseBuilding building)
        {
            Buildings.BaseBuilding lastBuilding = _selectableBuildings.Last();
            if (building == lastBuilding)
                return;

            if (lastBuilding.Team != Buildings.Team.Blue)
                RemoveLastFromList(lastBuilding);

            if (_selectableBuildings.Contains(building))
                PlaceInTheEnd(building);
            else
                Select(building);
        }

        private void PlaceInTheEnd(Buildings.BaseBuilding building)
        {
            _selectableBuildings.Remove(building);
            _selectableBuildings.Add(building);
        }

        private void RemoveLastFromList(Buildings.BaseBuilding lastBuilding)
        {
            CancelSelection(lastBuilding);
            _selectableBuildings.RemoveAt(_selectableBuildings.Count - 1);
        }

        private void LaunchToDestination(Buildings.BaseBuilding destination)
        {
            foreach (Buildings.BaseBuilding building in _selectableBuildings)
            {
                if(building.Team==Buildings.Team.Blue)
                    building.LaunchUnit(destination);
            }
        }

        private void AddToList(Buildings.BaseBuilding planet) => 
            _selectableBuildings.Add(planet);
        
        private static void DecreaseScale(Buildings.BaseBuilding planet)
        {
            if(planet!=null)
                planet.transform.localScale /= 1.5f;
        }
    
        private static void IncreaseScale(Buildings.BaseBuilding planet)
        {
            if(planet!=null) 
                planet.transform.localScale *= 1.5f;
        }
        
    }
}