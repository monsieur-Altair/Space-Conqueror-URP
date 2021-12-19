using System;
using System.Collections.Generic;
using UnityEngine;
using Type = Planets.Type;

namespace Managers
{    
    [DefaultExecutionOrder(1000)]
    public class Outlook : MonoBehaviour
    {
        private List<List<Texture>> _allTextures=new List<List<Texture>>();
        
        [SerializeField] private List<Texture> scientificTextures;
        [SerializeField] private List<Texture> attackerTextures;
        [SerializeField] private List<Texture> spawnerTextures;
        
        
        [SerializeField] private List<Texture> rocketsTextures;
        
        [SerializeField] private Material buffedPlanetMaterial;
        [SerializeField] private Material buffedRocketMaterial;
        
        [SerializeField] private Material basePlanetMaterial;
        [SerializeField] private Material baseRocketMaterial;
        
        private readonly Dictionary<int, MeshRenderer> _planetsRenderer = new Dictionary<int, MeshRenderer>();

        private List<Planets.Base> _allPlanets => Main.Instance.AllPlanets;
        public static Outlook Instance { get; private set; }

        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }
        private Outlook()
        {
            
        }
        
        public void Start()
        {
            FillList();
            SetAllOutlooks();
            _allTextures.Add(scientificTextures);
            _allTextures.Add(spawnerTextures);
            _allTextures.Add(attackerTextures);
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
                Decompose(planet);
            }
        }

        private void Decompose(Planets.Base planet)
        {
            var index = planet.ID.GetHashCode();
            var circle = planet.transform.GetChild(0);
            _planetsRenderer.Add(index,circle.GetComponent<MeshRenderer>());
        }
        
        public void SetOutlook(Planets.Base planet)
        {
            var team = (int)planet.Team;
            int index = planet.ID.GetHashCode();
            var material = new Material(basePlanetMaterial);
            
            switch (planet.Type)
            {
                case Type.Scientific:
                    material.mainTexture=scientificTextures[team];
                    break;
                case Type.Spawner:
                    break;
                case Type.Attacker:
                    material.mainTexture = attackerTextures[team];
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _planetsRenderer[index].material = material;
        }

        public void SetUnitOutlook(Planets.Base planet, Units.Base unit)
        {
            var team = (int) planet.Team;
            //also we can add all rockets materials to list 
            var selected = planet.IsBuffed ? buffedRocketMaterial : baseRocketMaterial;
            var material = new Material(selected)
            {
                mainTexture = rocketsTextures[team]
            };
            unit.transform.GetChild(0).GetComponent<MeshRenderer>().material = material;
        }

        public void SetBuff(Planets.Base planet)
        {
            int index = planet.ID.GetHashCode();
            var team = (int) planet.Team;
            var type = (int) planet.Type;
            var material = new Material(buffedPlanetMaterial)
            {
                mainTexture = _allTextures[type][team]
            };
            _planetsRenderer[index].material=material;
        }
        
        public void UnSetBuff(Planets.Base planet)
        {
            int index = planet.ID.GetHashCode();
            var team = (int) planet.Team;
            var type = (int) planet.Type;
            var material = new Material(basePlanetMaterial)
            {
                mainTexture = _allTextures[type][team]
            };
            _planetsRenderer[index].material = material;
        }
        
    }
}