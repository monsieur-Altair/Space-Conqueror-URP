using System;
using System.Collections.Generic;
using _Application.Scripts.Units;
using UnityEngine;
using Base = _Application.Scripts.Units.Base;
using Type = _Application.Scripts.Buildings.Type;

namespace _Application.Scripts.Managers
{    
    public class Outlook
    {
        private const int BuffTexIndex = 0;
        private const int CrystalBaseHash = 1;
        private const int CrystalEmissionHash = 2;
        private const int FlagHash = 3;
        private const int RoofHash = 4;
        private static readonly int BaseMap = Shader.PropertyToID("_BaseMap");
        private static readonly int EmissionMap = Shader.PropertyToID("_EmissionMap");

        private readonly List<Texture> _warriorsTextures;

        private readonly Material _buffedBuildingMaterial;
        private readonly Material _buffedWarriorMaterial;

        private readonly Material _baseBuildingMaterial;
        private readonly Material _baseWarriorMaterial;

        private readonly Material _baseCrystalMaterial;
        private readonly Material _baseFlagMaterial;
        private readonly Material _baseRoofMaterial;

        private readonly Warehouse _warehouse;

        private readonly Dictionary<int, List<Texture>> _allTextures = new Dictionary<int, List<Texture>>();
        private readonly Dictionary<int, MeshRenderer> _buildingsRenderer = new Dictionary<int, MeshRenderer>();
        private List<Buildings.Base> _allBuildings = new List<Buildings.Base>();


        public Outlook(Warehouse warehouse)
        {
            _warehouse = warehouse;

            _warriorsTextures = _warehouse.warriorsTextures;

            _buffedBuildingMaterial = _warehouse.buffedBuildingMaterial;
            _buffedWarriorMaterial = _warehouse.buffedWarriorMaterial;
            
            _baseBuildingMaterial = _warehouse.baseBuildingMaterial;
            _baseWarriorMaterial = _warehouse.baseWarriorMaterial;
            
            _baseCrystalMaterial = _warehouse.baseCrystalMaterial;
            _baseFlagMaterial = _warehouse.baseFlagMaterial;
            _baseRoofMaterial = _warehouse.baseRoofMaterial;
            
            _allTextures.Add(CrystalBaseHash,_warehouse.crystalBaseTextures);
            _allTextures.Add(CrystalEmissionHash,_warehouse.crystalEmissionTextures);
            _allTextures.Add(RoofHash,_warehouse.roofTextures);
            _allTextures.Add(FlagHash,_warehouse.flagTextures);
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
            Type type = building.Type;
            int index = building.ID.GetHashCode();

            _buildingsRenderer[index].materials = GetMaterials(type, team);
        }

        private Material[] GetMaterials(Type type, int team)
        {
            switch (type)
            {
                case Type.Altar:
                    Material crystalMat = new Material(_baseCrystalMaterial);
                    crystalMat.SetTexture(BaseMap, _allTextures[CrystalBaseHash][team]);
                    crystalMat.SetTexture(EmissionMap, _allTextures[CrystalEmissionHash][team]);
                    return new[] {crystalMat, _baseBuildingMaterial, _baseBuildingMaterial};
                case Type.Spawner:
                    Material flagSpawnerMat = new Material(_baseFlagMaterial)
                    {
                        mainTexture = _allTextures[FlagHash][team]
                    };
                    Material roofMat = new Material(_baseRoofMaterial)
                    {
                        mainTexture =  _allTextures[RoofHash][team]
                    };
                    return new[] { roofMat, flagSpawnerMat, _baseBuildingMaterial, _baseBuildingMaterial };
                case Type.Attacker:
                    Material flagMat = new Material(_baseFlagMaterial)
                    {
                        mainTexture = _allTextures[FlagHash][team]
                    };
                    return new[] { flagMat, _baseBuildingMaterial, _baseBuildingMaterial};
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private void SetUnitOutlook(Buildings.Base building, Base unit)
        {
            int team = (int) building.Team;
            //also we can add all rockets materials to list 

            unit.GetComponent<Warrior>().skinnedMeshRenderer.materials = GetUnitMaterials(building.IsBuffed, team);
        }

        private Material[] GetUnitMaterials(bool isBuffed, int team)
        {
            Material mainMaterial = new Material(_baseWarriorMaterial)
            {
                mainTexture = _warriorsTextures[team]
            };
            Material secondMat = isBuffed ? _buffedWarriorMaterial : mainMaterial;

            return new[] {mainMaterial, secondMat};
        }

        private void SetBuff(Buildings.Base building)
        {
            int index = building.ID.GetHashCode();
            SetBuffMaterial(index, _buffedBuildingMaterial);
        }

        private void UnSetBuff(Buildings.Base building)
        {
            int index = building.ID.GetHashCode();
            SetBuffMaterial(index, _baseBuildingMaterial);
        }

        private void SetBuffMaterial(int index, Material newMaterial)
        {
            Material[] materials = _buildingsRenderer[index].materials;
            materials[materials.Length-1] = newMaterial;
            _buildingsRenderer[index].materials = materials;
        }
        
    }
}