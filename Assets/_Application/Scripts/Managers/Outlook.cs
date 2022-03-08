using System.Collections.Generic;
using _Application.Scripts.Units;
using UnityEngine;
using Type = _Application.Scripts.Planets.Type;

namespace _Application.Scripts.Managers
{    
    public class Outlook : MonoBehaviour
    {
        public static Outlook Instance { get; private set; }
        
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
        private List<Planets.Base> _allPlanets = new List<Planets.Base>();

        private const int MainTexIndex = 0;
        private const int BuffTexIndex = 1;


        public void Awake()
        {
            if (Instance == null) 
                Instance = this;
            _allTextures.Add(scientificTextures);
            _allTextures.Add(spawnerTextures);
            _allTextures.Add(attackerTextures);
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
            Material mainMaterial = new Material(basePlanetMaterial)
            {
                mainTexture = _allTextures[type][team]
            };
            Material secondMaterial = (type == (int)Type.Spawner) ? glassMaterial : null;
            Material[] materials = {mainMaterial, secondMaterial};
            _planetsRenderer[index].materials = materials;
        }

        private void SetUnitOutlook(Planets.Base planet, Base unit)
        {
            int team = (int) planet.Team;
            //also we can add all rockets materials to list 
            Material buffedMaterial = planet.IsBuffed ? buffedRocketMaterial : null;
            
            Material mainMaterial = new Material(baseRocketMaterial)
            {
                mainTexture = rocketsTextures[team]
            };

            Material[] materials = {mainMaterial, buffedMaterial};
            unit.transform.GetChild(0).GetComponent<MeshRenderer>().materials = materials;
        }

        private void SetBuff(Planets.Base planet)
        {
            int index = planet.ID.GetHashCode();
            
            Material[] materials = _planetsRenderer[index].materials;
            materials[BuffTexIndex] = buffedPlanetMaterial;
            _planetsRenderer[index].materials = materials;
        }

        private void UnSetBuff(Planets.Base planet)
        {
            int index = planet.ID.GetHashCode();
            
            if (_planetsRenderer.TryGetValue(index,out var value))
            {
                Material[] materials = value.materials;
                materials[BuffTexIndex] = (planet.Type == Type.Spawner) ? glassMaterial : null;
                value.materials = materials;
            }
        }
        
    }
}