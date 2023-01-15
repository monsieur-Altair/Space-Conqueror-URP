using System.Collections.Generic;
using _Application.Scripts.Infrastructure.Services;
using _Application.Scripts.Managers;
using _Application.Scripts.Misc;
using Pool_And_Particles;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Application.Scripts.UI
{
    public static class CounterSpawner
    {
        private static List<Color> _counterBackground;
        private static List<Color> _counterForeground;
        private static Vector3 _offset = new Vector3(0, -35, 0);
        private static Vector3 _baseCounterScale = new Vector3(1, 1, 1);
        private static List<Buildings.Base> _allBuildings = new List<Buildings.Base>();
        private static List<GameObject> _countersList = new List<GameObject>();
        private static Dictionary<int, Image> _backgrounds = new Dictionary<int, Image>();
        private static Dictionary<int, TextMeshProUGUI> _foregrounds = new Dictionary<int, TextMeshProUGUI>();
        private static Transform _container;
        private static GlobalPool _pool;
        private static GlobalCamera _globalCamera;

        public static void Init(Warehouse warehouse, GlobalPool pool, Transform container, GlobalCamera globalCamera)
        {
            _globalCamera = globalCamera;
            _counterBackground = warehouse.counterBackground;
            _counterForeground = warehouse.counterForeground;
            _container = container;
            _pool = pool;
        }
        
        public static void FillLists(List<Buildings.Base> allBuildings)
        {
            _allBuildings = new List<Buildings.Base>(allBuildings);

            foreach (Buildings.Base building in _allBuildings)
            {
                building.CountChanged += SetCounter;
                building.TeamChanged += SetCounterColor;

                GameObject counter = SpawnCounterTo(building);
                DecomposeCounter(counter, building);
                
                SetCounterColor(building);
                SetCounter(building, (int) building.Count);
            }
        }
        
        private static void DecomposeCounter(GameObject counter, Buildings.Base building)
        {
            int index = building.ID.GetHashCode();
            _foregrounds.Add(index, counter.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>());
            _backgrounds.Add(index, counter.GetComponentInChildren<Image>());
        }

        private static GameObject SpawnCounterTo(Buildings.Base building)
        {
            Vector3 counterPos = GetCounterPos(building);
            GameObject counter = _pool.GetObject(PoolObjectType.Counter, counterPos, Quaternion.identity).gameObject;
            counter.transform.SetParent(_container);
            counter.transform.localScale = _baseCounterScale;
            _countersList.Add(counter);
            return counter;
        }

        private static Vector3 GetCounterPos(Buildings.Base building)
        {
            Vector3 pos = building.transform.position;
            Vector3 screenPos = _globalCamera.GetScreenPos(pos);
            return screenPos + _offset;
        }

        private static void SetCounterColor(Buildings.Base building)
        {
            int team = (int) building.Team;
            int index = building.ID.GetHashCode();
            _foregrounds[index].color = _counterForeground[team];
            _backgrounds[index].color = _counterBackground[team];
        }

        private static void SetCounter(Buildings.Base building, int value)
        {
            int index = building.ID.GetHashCode();
            _foregrounds[index].text = value.ToString();
        }

        public static void ClearList()
        {
            foreach (GameObject counter in _countersList) 
                counter.gameObject.SetActive(false);
            
            foreach (Buildings.Base building in _allBuildings)
            {
                building.CountChanged -= SetCounter;
                building.TeamChanged -= SetCounterColor;
            }
            _allBuildings.Clear();
            _foregrounds.Clear();
            _backgrounds.Clear();
        }
    }
}