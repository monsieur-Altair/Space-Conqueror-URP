using System;
using System.Collections.Generic;
using _Application.Scripts.Buildings;
using _Application.Scripts.Infrastructure.Services;
using _Application.Scripts.Units;
using UnityEngine;

namespace _Application.Scripts.Managers
{
    public class OutlookService : IService

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
        private List<BaseBuilding> _allBuildings = new List<BaseBuilding>();


        public OutlookService(Warehouse warehouse)
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

            _allTextures.Add(CrystalBaseHash, _warehouse.crystalBaseTextures);
            _allTextures.Add(CrystalEmissionHash, _warehouse.crystalEmissionTextures);
            _allTextures.Add(RoofHash, _warehouse.roofTextures);
            _allTextures.Add(FlagHash, _warehouse.flagTextures);
        }

        public void PrepareLevel(List<BaseBuilding> planets)
        {
            ClearLists();
            _allBuildings = new List<BaseBuilding>(planets);
            FillList();
        }

        private void ClearLists()
        {
            foreach (BaseBuilding building in _allBuildings)
            {
                building.Buffed -= SetBuff;
                building.UnBuffed -= UnSetBuff;
                building.LaunchedUnit -= SetUnitOutlook;
                building.TeamChanged -= SetOutlook;
            }

            _allBuildings.Clear();
        }

        private void FillList()
        {
            foreach (BaseBuilding building in _allBuildings)
            {
                //building.SetOutlookManager();
                building.Buffed += SetBuff;
                building.UnBuffed += UnSetBuff;
                building.LaunchedUnit += SetUnitOutlook;
                building.TeamChanged += SetOutlook;
                SetOutlook(building);
            }
        }

        private void SetOutlook(BaseBuilding building)
        {
            int team = (int) building.Team;
            SetOutlook(building, team);
        }
        
        public void SetOutlook(BaseBuilding building, int team)
        {
            BuildingType buildingType = building.BuildingType;
            building.MeshRenderer.materials = GetMaterials(buildingType, team);
        }

        private Material[] GetMaterials(BuildingType buildingType, int team)
        {
            switch (buildingType)
            {
                case BuildingType.Altar:
                    Material crystalMat = new Material(_baseCrystalMaterial);
                    crystalMat.SetTexture(BaseMap, _allTextures[CrystalBaseHash][team]);
                    crystalMat.SetTexture(EmissionMap, _allTextures[CrystalEmissionHash][team]);
                    return new[] {crystalMat, _baseBuildingMaterial, _baseBuildingMaterial};
                case BuildingType.Spawner:
                    Material flagSpawnerMat = new Material(_baseFlagMaterial)
                    {
                        mainTexture = _allTextures[FlagHash][team]
                    };
                    Material roofMat = new Material(_baseRoofMaterial)
                    {
                        mainTexture = _allTextures[RoofHash][team],
                        color = GetColor(team),
                    };
                    return new[] {roofMat, flagSpawnerMat, _baseBuildingMaterial, _baseBuildingMaterial};
                case BuildingType.Attacker:
                    Material flagMat = new Material(_baseFlagMaterial)
                    {
                        mainTexture = _allTextures[FlagHash][team]
                    };
                    return new[] {flagMat, _baseBuildingMaterial, _baseBuildingMaterial};
                default:
                    throw new ArgumentOutOfRangeException(nameof(buildingType), buildingType, null);
            }
        }

        private static Color GetColor(int team) => 
            team == (int)Team.Red ? new Color(0.7f,0.7f,0.7f) : Color.white;

        private void SetUnitOutlook(BaseBuilding building, BaseUnit unit)
        {
            int team = (int) building.Team;
            //also we can add all rockets materials to list 

            unit.SkinnedMeshRenderer.materials = GetUnitMaterials(building.IsBuffed, team);
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

        private void SetBuff(BaseBuilding building)
        {
            SetBuffMaterial(building, _buffedBuildingMaterial);
        }

        private void UnSetBuff(BaseBuilding building)
        {
            SetBuffMaterial(building, _baseBuildingMaterial);
        }

        private void SetBuffMaterial(BaseBuilding building, Material newMaterial)
        {
            Material[] materials = building.MeshRenderer.materials;
            materials[materials.Length - 1] = newMaterial;
            building.MeshRenderer.materials = materials;
        }
    }
}