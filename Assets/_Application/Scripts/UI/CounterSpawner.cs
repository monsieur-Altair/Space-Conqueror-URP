using System;
using System.Collections.Generic;
using _Application.Scripts.Buildings;
using _Application.Scripts.Infrastructure.Services;
using _Application.Scripts.Managers;
using _Application.Scripts.Units;
using Pool_And_Particles;
using UnityEngine;

namespace _Application.Scripts.UI
{
    public class CounterSpawner
    {
        private readonly Transform _container;
        private readonly GlobalPool _pool;
        private readonly GlobalCamera _globalCamera;
        private readonly Warehouse _warehouse;
        private readonly Counter _counterPrefab;

        private readonly Dictionary<BaseBuilding, Counter> _buildingsCounters = new Dictionary<BaseBuilding, Counter>();
        private readonly Dictionary<BaseUnit, Counter> _unitsCounters = new Dictionary<BaseUnit, Counter>();

        public CounterSpawner(Warehouse warehouse, GlobalPool pool, Transform container, GlobalCamera globalCamera)
        {
            _globalCamera = globalCamera;
            _warehouse = warehouse;
            _container = container;
            _pool = pool;

            _counterPrefab = _pool.GetPooledBehaviourPrefab(PoolObjectType.Counter).GetComponent<Counter>();
        }
        
        public void FillLists(List<BaseBuilding> allBuildings)
        {
            foreach (BaseBuilding building in allBuildings)
            {
                building.CountChanged += SetCounter;
                building.TeamChanged += SetCounterColor;

                Counter counter = SpawnCounterTo(building);
                _buildingsCounters.Add(building, counter);
                
                SetCounterColor(building);
                SetCounter(building, (int) building.Count);
            }

            BaseUnit.Launched += SetCounter;
            BaseUnit.Updated += UpdateCounterPos;
            BaseUnit.Approached += FreeCounter;
        }

        private void FreeCounter(BaseUnit unit)
        {
            Counter counter = _unitsCounters[unit];
            _pool.Free(counter);
            _unitsCounters.Remove(unit);
        }

        private void UpdateCounterPos(BaseUnit unit)
        {
            Vector2 counterPos = UISystem.GetUIPosition(_globalCamera.MainCamera, unit.CounterPoint.position);
            _unitsCounters[unit].SetAnchorPos(counterPos);
        }

        private void SetCounter(BaseUnit unit)
        { 
            Counter counter = _pool.Get(_counterPrefab, parent: _container);
            int team = (int) unit.UnitInf.UnitTeam;
            counter.SetColors(_warehouse.counterForeground[team], _warehouse.counterBackground[team]);
            counter.SetText(Mathf.RoundToInt(unit.UnitInf.UnitCount).ToString());
            _unitsCounters.Add(unit, counter);
            UpdateCounterPos(unit);
        }

        public void ClearList()
        {
            foreach (KeyValuePair<BaseBuilding, Counter> pair in _buildingsCounters)
            {
                BaseBuilding building = pair.Key;
                building.CountChanged -= SetCounter;
                building.TeamChanged -= SetCounterColor;

                Counter counter = pair.Value;
                _pool.Free(counter);
            }
            
            BaseUnit.Launched -= SetCounter;
            BaseUnit.Updated -= UpdateCounterPos;
            BaseUnit.Approached -= FreeCounter;
            
            _buildingsCounters.Clear();

            foreach (Counter counter in _unitsCounters.Values)
            {
                _pool.Free(counter);
            }
            
            _unitsCounters.Clear();
        }
        
        private Counter SpawnCounterTo(BaseBuilding building)
        {
            Vector3 pos = building.CounterPoint.position;
            Vector2 counterPos = UISystem.GetUIPosition(_globalCamera.MainCamera, pos);
            Counter counter = _pool.Get(_counterPrefab, parent: _container);
            counter.SetAnchorPos(counterPos);
            return counter;
        }

        private void SetCounterColor(BaseBuilding building)
        {
            int team = (int) building.Team;
            _buildingsCounters[building].SetColors(_warehouse.counterForeground[team], _warehouse.counterBackground[team]);
        }

        private void SetCounter(BaseBuilding building, int value)
        {
            _buildingsCounters[building].SetText(value.ToString());
        }
    }
}