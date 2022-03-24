using System.Collections.Generic;
using _Application.Scripts.Units;
using UnityEngine;
using Type = _Application.Scripts.Buildings.Type;

namespace _Application.Scripts.Managers
{    
    public class Outlook
    {
        private readonly List<Texture> _altarTextures;
        private readonly List<Texture> _attackerTextures;
        private readonly List<Texture> _spawnerTextures;

        private readonly List<Texture> _warriorsTextures;

        private readonly Material _buffedBuildingMaterial;
        private readonly Material _buffedWarriorMaterial;

        private readonly Material _baseBuildingMaterial;
        private readonly Material _baseWarriorMaterial;

        private readonly Warehouse _warehouse;
        
        private readonly List<List<Texture>> _allTextures=new List<List<Texture>>();
        private readonly Dictionary<int, MeshRenderer> _buildingsRenderer = new Dictionary<int, MeshRenderer>();
        private List<Buildings.Base> _allBuildings = new List<Buildings.Base>();

        private const int BuffTexIndex = 1;


        public Outlook(Warehouse warehouse)
        {
            _warehouse = warehouse;

            _altarTextures = _warehouse.altarTextures;
            _attackerTextures = _warehouse.attackerTextures;
            _spawnerTextures = _warehouse.spawnerTextures;
            _warriorsTextures = _warehouse.warriorsTextures;

            _buffedBuildingMaterial = _warehouse.buffedBuildingMaterial;
            _buffedWarriorMaterial = _warehouse.buffedWarriorMaterial;
            _baseBuildingMaterial = _warehouse.baseBuildingMaterial;
            _baseWarriorMaterial = _warehouse.baseWarriorMaterial;
            
            _allTextures.Add(_altarTextures);
            _allTextures.Add(_spawnerTextures);
            _allTextures.Add(_attackerTextures);
        }
       
        public void PrepareLevel(List<Buildings.Base> planets)
        {
            ClearLists();
            _allBuildings = new List<Buildings.Base>(planets);
            FillList();
        }

        private void ClearLists()
        {
            foreach (Buildings.Base building in _allBuildings)
            {
                building.Buffed -= SetBuff;
                building.UnBuffed -= UnSetBuff;
                building.LaunchedUnit -= SetUnitOutlook;
                building.TeamChanged -= SetOutlook;
            }
            _allBuildings.Clear();
            _buildingsRenderer.Clear();
        }

        private void FillList()
        {
            foreach (Buildings.Base building in _allBuildings)
            {
                //building.SetOutlookManager();
                building.Buffed += SetBuff;
                building.UnBuffed += UnSetBuff;
                building.LaunchedUnit += SetUnitOutlook;
                building.TeamChanged += SetOutlook;
                
                Decompose(building);
                
                SetOutlook(building);
            }
        }

        private void Decompose(Buildings.Base building)
        {
            int index = building.ID.GetHashCode();
            Transform circle = building.transform.GetChild(0);
            _buildingsRenderer.Add(index, circle.GetComponent<MeshRenderer>());
        }

        private void SetOutlook(Buildings.Base building)
        {
            int team = (int)building.Team;
            int type = (int) building.Type;
            int index = building.ID.GetHashCode();
            Material mainMaterial = new Material(_baseBuildingMaterial)
            {
                mainTexture = _allTextures[type][team]
            };
            Material[] materials = {mainMaterial, null};
            _buildingsRenderer[index].materials = materials;
        }

        private void SetUnitOutlook(Buildings.Base building, Base unit)
        {
            int team = (int) building.Team;
            //also we can add all rockets materials to list 
            Material buffedMaterial = building.IsBuffed ? _buffedWarriorMaterial : null;
            
            Material mainMaterial = new Material(_baseWarriorMaterial)
            {
                mainTexture = _warriorsTextures[team]
            };

            Material[] materials = {mainMaterial, buffedMaterial};
            unit.transform.GetChild(0).GetComponent<MeshRenderer>().materials = materials;
        }

        private void SetBuff(Buildings.Base building)
        {
            int index = building.ID.GetHashCode();
            
            Material[] materials = _buildingsRenderer[index].materials;
            materials[BuffTexIndex] = _buffedBuildingMaterial;
            _buildingsRenderer[index].materials = materials;
        }

        private void UnSetBuff(Buildings.Base building)
        {
            int index = building.ID.GetHashCode();
            
            if (_buildingsRenderer.TryGetValue(index,out MeshRenderer value))
            {
                Material[] materials = value.materials;
                materials[BuffTexIndex] = null;
                value.materials = materials;
            }
        }
        
    }
}