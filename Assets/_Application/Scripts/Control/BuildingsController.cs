using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Application.Scripts.Control
{
    public class BuildingsController
    {
        private event Action<Buildings.Base> CancelingSelection;
        private event Action<Buildings.Base> BuildingSelected;
        
        private readonly List<Buildings.Base> _selectableBuildings;
        private readonly Camera _mainCamera;

        public BuildingsController(Camera camera)
        {
            _mainCamera = camera;
            _selectableBuildings = new List<Buildings.Base>();
            CancelingSelection += DecreaseScale;
            BuildingSelected += IncreaseScale;
            BuildingSelected += AddToList;
        }

        public void HandleClick(Vector3 pos)
        {
            Buildings.Base building = RaycastForBuilding(pos);
            if (building != null)
                if (_selectableBuildings.Contains(building) == false && building.Team == Buildings.Team.Blue)
                    OnSelecting(building);
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
                StartLaunching(count);
        }

        public void HandleMultipleSelection(Vector3 pos)
        {
            Buildings.Base building = RaycastForBuilding(pos);
            if (building == null) 
                return;
            //1 - empty: only blue,
            //2 - not empty: if user has selected blue, check container + if the last isn't blue, delete them 

            Buildings.Team team = building.Team;
            int count = _selectableBuildings.Count;
            if (team == Buildings.Team.Blue)
            {
                if(count==0)
                    OnSelecting(building);
                else
                    MoveAllyToTheEnd(building);
            }
            else
            {
                if (count > 0) 
                    MoveEnemyToTheEnd(building, count);
            }
        }

        private void StartLaunching(int count)
        {
            Buildings.Base destination = _selectableBuildings[count - 1];
            _selectableBuildings.Remove(destination);
            OnCancelingSelection(destination);
            LaunchToDestination(destination);
            _selectableBuildings.Clear();
        }

        private void ClearAllSelection()
        {
            foreach (Buildings.Base building in _selectableBuildings)
                OnCancelingSelection(building);
            _selectableBuildings.Clear();
        }

        private void OnSelecting(Buildings.Base building) => 
            BuildingSelected?.Invoke(building);

        private void OnCancelingSelection(Buildings.Base building) => 
            CancelingSelection?.Invoke(building);

        private Buildings.Base RaycastForBuilding(Vector3 pos)
        {
            int layerMask = 1 << 0;
            layerMask = ~layerMask;
            Ray ray = _mainCamera.ScreenPointToRay(pos);
            return Physics.Raycast(ray, out RaycastHit hit,Mathf.Infinity, layerMask) 
                ? hit.collider.GetComponent<Buildings.Base>() : null;
        }

        private void MoveEnemyToTheEnd(Buildings.Base building, int count)
        {
            Buildings.Base lastBuilding = _selectableBuildings.Last();
            if (building == lastBuilding)
                return;

            if (lastBuilding.Team != Buildings.Team.Blue)
            {
                OnCancelingSelection(lastBuilding);
                _selectableBuildings.RemoveAt(count - 1);
            }

            OnSelecting(building);
        }

        private void MoveAllyToTheEnd(Buildings.Base building)
        {
            Buildings.Base lastBuilding = _selectableBuildings.Last();
            if (building == lastBuilding)
                return;

            if (lastBuilding.Team != Buildings.Team.Blue)
                RemoveLastFromList(lastBuilding);

            if (_selectableBuildings.Contains(building))
                PlaceInTheEnd(building);
            else
                OnSelecting(building);
        }

        private void PlaceInTheEnd(Buildings.Base building)
        {
            _selectableBuildings.Remove(building);
            _selectableBuildings.Add(building);
        }

        private void RemoveLastFromList(Buildings.Base lastBuilding)
        {
            OnCancelingSelection(lastBuilding);
            _selectableBuildings.RemoveAt(_selectableBuildings.Count - 1);
        }

        private void LaunchToDestination(Buildings.Base destination)
        {
            foreach (Buildings.Base building in _selectableBuildings)
            {
                OnCancelingSelection(building);
                if(building.Team==Buildings.Team.Blue)
                    building.LaunchUnit(destination);
            }
            
        }

        private static void DecreaseScale(Buildings.Base planet)
        {
            if(planet!=null)
                planet.transform.localScale /= 1.5f;
        }
    
        private static void IncreaseScale(Buildings.Base planet)
        {
            if(planet!=null) 
                planet.transform.localScale *= 1.5f;
        }
    
        private void AddToList(Buildings.Base planet) => 
            _selectableBuildings.Add(planet);
    }
}