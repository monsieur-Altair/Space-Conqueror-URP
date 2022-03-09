using System.Collections.Generic;
using _Application.Scripts.Misc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Application.Scripts.Managers
{
    public class UI : MonoBehaviour
    {
        public static UI Instance { get; private set; }
        
        private Canvas _canvas;
        
        [SerializeField] private List<Color> counterBackground;
        [SerializeField] private List<Color> counterForeground;

        private List<Planets.Base> _allPlanets = new List<Planets.Base>();
        private Camera _mainCamera;
        private List<GameObject> _countersList;
        
        private float _maxDepth;
        private float _minDepth;

        private readonly Dictionary<int, Image> _backgrounds = new Dictionary<int, Image>();
        private readonly Dictionary<int, TextMeshProUGUI> _foregrounds = new Dictionary<int, TextMeshProUGUI>();
        
        private Vector3 _offsetCamera;
        private Vector3 _offsetCounter;

        private ObjectPool _pool;
        
        public void Awake()
        {
            if (Instance == null) 
                Instance = this;

            _canvas = GameObject.FindGameObjectWithTag("CanvasTag").GetComponent<Canvas>();
            _pool = ObjectPool.Instance;
            _mainCamera = Camera.main;

            CameraResolution.GetCameraDepths(out _minDepth, out _maxDepth);

            _countersList = new List<GameObject>();
            _offsetCamera = new Vector3(0, -1.0f, 0);
            _offsetCounter = new Vector3(0, 0, 0);

        }

        public void PrepareLevel()
        {
            ClearList();
            _allPlanets = new List<Planets.Base>(Main.Instance.AllPlanets);
            FillLists();
        }

        private void ClearList()
        {
            foreach (GameObject counter in _countersList) 
                counter.gameObject.SetActive(false);
            
            foreach (Planets.Base planet in _allPlanets)
            {
                planet.CountChanged -= SetCounter;
                planet.TeamChanged -= SetCounterColor;
            }
            _allPlanets.Clear();
            _foregrounds.Clear();
            _backgrounds.Clear();
            //_countersList.Clear();
        }

        private void FillLists()
        {
            foreach (Planets.Base planet in _allPlanets)
            {
                planet.CountChanged += SetCounter;
                planet.TeamChanged += SetCounterColor;

                GameObject counter = SpawnCounterTo(planet);
                DecomposeCounter(counter, planet);
                SetCounterColor(planet);
            }
        }

        private void DecomposeCounter(GameObject counter, Planets.Base planet)
        {
            int index = planet.ID.GetHashCode();
            _foregrounds.Add(index, counter.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>());
            _backgrounds.Add(index, counter.GetComponentInChildren<Image>());
        }
        
        private GameObject SpawnCounterTo(Planets.Base planet)
        {
            Vector3 counterPos = GetCounterPos(planet);
            GameObject counter = _pool.GetObject(ObjectPool.PoolObjectType.Counter, counterPos, Quaternion.identity);
            counter.transform.SetParent(_canvas.transform);
            _countersList.Add(counter);
            return counter;
        }
        
        private Vector3 GetCounterPos(Planets.Base planet)
        {
            Vector3 pos = planet.transform.position;
            _offsetCamera = CameraResolution.FindOffset(pos, _minDepth, _maxDepth);
            Vector3 screenPos = _mainCamera.WorldToScreenPoint(pos + _offsetCounter);
            return screenPos + _offsetCamera;
        }

        private void SetCounterColor(Planets.Base planet)
        {
            int team = (int) planet.Team;
            int index = planet.ID.GetHashCode();
            _foregrounds[index].color = counterForeground[team];
            _backgrounds[index].color = counterBackground[team];
        }
        
        private void SetCounter(Planets.Base planet, int value)
        {
            int index = planet.ID.GetHashCode();
            _foregrounds[index].text = value.ToString();
        }

    }
}