using System;
using System.Collections.Generic;
using Planets;
using UnityEngine;
using Type = Planets.Type;

namespace Managers
{    
    //[DefaultExecutionOrder(1000)]
    public class Outlook : MonoBehaviour
    {
        private readonly List<List<Texture>> _allTextures=new List<List<Texture>>();
        
        [SerializeField] private List<Texture> scientificTextures;
        [SerializeField] private List<Texture> attackerTextures;
        [SerializeField] private List<Texture> spawnerTextures;
        
        
        [SerializeField] private List<Texture> rocketsTextures;
        
        [SerializeField] private Material buffedPlanetMaterial;
        [SerializeField] private Material buffedRocketMaterial;
        
        [SerializeField] private Material basePlanetMaterial;
        [SerializeField] private Material baseRocketMaterial;

        [SerializeField] private Material glassMaterial;
        
        private readonly Dictionary<int, MeshRenderer> _planetsRenderer = new Dictionary<int, MeshRenderer>();

        private const int MainTexIndex = 0;
        private const int BuffTexIndex = 1;
        
        private List<Planets.Base> _allPlanets;
        public static Outlook Instance { get; private set; }

        public void Awake()
        {
            Debug.Log("awake outlook");
            if (Instance == null)
            {
                Instance = this;
            }
            _allTextures.Add(scientificTextures);
            _allTextures.Add(spawnerTextures);
            _allTextures.Add(attackerTextures);

        }
        private Outlook()
        {
            
        }
        
        public void OnEnable()
        {
            /*Debug.Log("enable outlook");
            PrepareLevel();*/
        }

        public void PrepareLevel()
        {
            _allPlanets=new List<Base>(Main.Instance.AllPlanets);
            _planetsRenderer?.Clear();
            FillList();
            SetAllOutlooks();
        }

        private void SetAllOutlooks()
        {
            foreach (var planet in _allPlanets)
            {
                SetOutlook(planet);
            }
        }

        private void FillList()
        {
            foreach (var planet in _allPlanets)
            {
                planet.SetOutlookManager();
                Decompose(planet);
            }
        }

        private void Decompose(Planets.Base planet)
        {
            var index = planet.ID.GetHashCode();
            var circle = planet.transform.GetChild(0);
            _planetsRenderer.Add(index, circle.GetComponent<MeshRenderer>());
        }
        
        public void SetOutlook(Planets.Base planet)
        {
            var team = (int)planet.Team;
            var type = (int) planet.Type;
            int index = planet.ID.GetHashCode();
            var mainMaterial = new Material(basePlanetMaterial)
            {
                mainTexture = _allTextures[type][team]
            };
            Material secondMaterial = (type == (int)Type.Spawner) ? glassMaterial : null;
            Material[] materials = {mainMaterial, secondMaterial};
            _planetsRenderer[index].materials = materials;
        }

        public void SetUnitOutlook(Planets.Base planet, Units.Base unit)
        {
            var team = (int) planet.Team;
            //also we can add all rockets materials to list 
            var buffedMaterial = planet.IsBuffed ? buffedRocketMaterial : null;
            
            var mainMaterial = new Material(baseRocketMaterial)
            {
                mainTexture = rocketsTextures[team]
            };

            Material[] materials = {mainMaterial, buffedMaterial};
            unit.transform.GetChild(0).GetComponent<MeshRenderer>().materials = materials;
        }

        public void SetBuff(Planets.Base planet)
        {
            int index = planet.ID.GetHashCode();
            
            var materials = _planetsRenderer[index].materials;
            materials[BuffTexIndex] = buffedPlanetMaterial;
            _planetsRenderer[index].materials = materials;
            
        }
        
        public void UnSetBuff(Planets.Base planet)
        {
            int index = planet.ID.GetHashCode();
            
            var materials = _planetsRenderer[index].materials;
            materials[BuffTexIndex] = (planet.Type == Type.Spawner) ? glassMaterial : null;
            _planetsRenderer[index].materials = materials;
        }
        
    }
}