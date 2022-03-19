using System.Collections.Generic;
using _Application.Scripts.Units;
using UnityEngine;
using Type = _Application.Scripts.Planets.Type;

namespace _Application.Scripts.Managers
{    
    public class Outlook
    {
        private readonly List<Texture> _scientificTextures;
        private readonly List<Texture> _attackerTextures;
        private readonly List<Texture> _spawnerTextures;

        private readonly List<Texture> _rocketsTextures;

        private readonly Material _buffedPlanetMaterial;
        private readonly Material _buffedRocketMaterial;

        private readonly Material _basePlanetMaterial;
        private readonly Material _baseRocketMaterial;

        private readonly Material _glassMaterial;

        private readonly Warehouse _warehouse;
        
        private readonly List<List<Texture>> _allTextures=new List<List<Texture>>();
        private readonly Dictionary<int, MeshRenderer> _planetsRenderer = new Dictionary<int, MeshRenderer>();
        private List<Planets.Base> _allPlanets = new List<Planets.Base>();

        private const int BuffTexIndex = 1;


        public Outlook(Warehouse warehouse)
        {
            _warehouse = warehouse;

            _scientificTextures = _warehouse.scientificTextures;
            _attackerTextures = _warehouse.attackerTextures;
            _spawnerTextures = _warehouse.spawnerTextures;
            _rocketsTextures = _warehouse.rocketsTextures;

            _buffedPlanetMaterial = _warehouse.buffedPlanetMaterial;
            _buffedRocketMaterial = _warehouse.buffedRocketMaterial;
            _basePlanetMaterial = _warehouse.basePlanetMaterial;
            _baseRocketMaterial = _warehouse.baseRocketMaterial;
            _glassMaterial = _warehouse.glassMaterial;
            
            _allTextures.Add(_scientificTextures);
            _allTextures.Add(_spawnerTextures);
            _allTextures.Add(_attackerTextures);
        }
       
        public void PrepareLevel(List<Planets.Base> planets)
        {
            ClearLists();
            _allPlanets = new List<Planets.Base>(planets);
            FillList();
        }

        private void ClearLists()
        {
            foreach (Planets.Base planet in _allPlanets)
            {
                planet.Buffed -= SetBuff;
                planet.UnBuffed -= UnSetBuff;
                planet.LaunchedUnit -= SetUnitOutlook;
                planet.TeamChanged -= SetOutlook;
            }
            _allPlanets.Clear();
            _planetsRenderer.Clear();
        }

        private void FillList()
        {
            foreach (Planets.Base planet in _allPlanets)
            {
                //planet.SetOutlookManager();
                planet.Buffed += SetBuff;
                planet.UnBuffed += UnSetBuff;
                planet.LaunchedUnit += SetUnitOutlook;
                planet.TeamChanged += SetOutlook;
                
                Decompose(planet);
                
                SetOutlook(planet);
            }
        }

        private void Decompose(Planets.Base planet)
        {
            int index = planet.ID.GetHashCode();
            Transform circle = planet.transform.GetChild(0);
            _planetsRenderer.Add(index, circle.GetComponent<MeshRenderer>());
        }

        private void SetOutlook(Planets.Base planet)
        {
            int team = (int)planet.Team;
            int type = (int) planet.Type;
            int index = planet.ID.GetHashCode();
            Material mainMaterial = new Material(_basePlanetMaterial)
            {
                mainTexture = _allTextures[type][team]
            };
            Material secondMaterial = (type == (int)Type.Spawner) ? _glassMaterial : null;
            Material[] materials = {mainMaterial, secondMaterial};
            _planetsRenderer[index].materials = materials;
        }

        private void SetUnitOutlook(Planets.Base planet, Base unit)
        {
            int team = (int) planet.Team;
            //also we can add all rockets materials to list 
            Material buffedMaterial = planet.IsBuffed ? _buffedRocketMaterial : null;
            
            Material mainMaterial = new Material(_baseRocketMaterial)
            {
                mainTexture = _rocketsTextures[team]
            };

            Material[] materials = {mainMaterial, buffedMaterial};
            unit.transform.GetChild(0).GetComponent<MeshRenderer>().materials = materials;
        }

        private void SetBuff(Planets.Base planet)
        {
            int index = planet.ID.GetHashCode();
            
            Material[] materials = _planetsRenderer[index].materials;
            materials[BuffTexIndex] = _buffedPlanetMaterial;
            _planetsRenderer[index].materials = materials;
        }

        private void UnSetBuff(Planets.Base planet)
        {
            int index = planet.ID.GetHashCode();
            
            if (_planetsRenderer.TryGetValue(index,out MeshRenderer value))
            {
                Material[] materials = value.materials;
                materials[BuffTexIndex] = (planet.Type == Type.Spawner) ? _glassMaterial : null;
                value.materials = materials;
            }
        }
        
    }
}