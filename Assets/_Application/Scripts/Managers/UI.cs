using System.Collections.Generic;
using _Application.Scripts.Control;
using _Application.Scripts.Misc;
using Planets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class UI : MonoBehaviour
    {
        public static UI Instance { get; private set; }
        
        [SerializeField] private Canvas canvas;
        
        [SerializeField] private List<Color> counterBackground;
        [SerializeField] private List<Color> counterForeground;

        private List<Planets.Base> _allPlanets=new List<Base>();
        private Camera _mainCamera;
        private List<GameObject> _countersList;
        
        private float _maxDepth;
        private float _minDepth;
        
        private readonly Dictionary<int,Image> _backgrounds=new Dictionary<int, Image>();
        private readonly Dictionary<int,TextMeshProUGUI> _foregrounds=new Dictionary<int, TextMeshProUGUI>();
        
        private Vector3 _offsetCamera;
        private Vector3 _offsetCounter;

        private ObjectPool _pool;
        
        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            CameraResolution.GetCameraDepths(out _minDepth, out _maxDepth);

            _mainCamera=Camera.main;
            if (_mainCamera == null)
                throw new MyException("can't get camera component");
            _countersList = new List<GameObject>();
            _offsetCamera = new Vector3(0, -1.0f, 0);
            _offsetCounter = new Vector3(0, 0, 0);

            _pool = ObjectPool.Instance;
        }

        public void PrepareLevel()
        {
            _allPlanets?.Clear();
            _allPlanets = new List<Base>(Main.Instance.AllPlanets);
            FillLists();
            SetAllCountersColor();
        }
        
        private void FillLists()
        {
            _foregrounds.Clear();
            _backgrounds.Clear();
            foreach (GameObject counter in _countersList)
            {
                counter.gameObject.SetActive(false);
            }

            if (Camera.main == null)
                throw new MyException("main camera = null");
            foreach (Base planet in _allPlanets)
            {
                planet.SetUIManager();
                Vector3 counterPos = GetCounterPos(planet);
                GameObject counter = _pool.GetObject(ObjectPool.PoolObjectType.Counter, counterPos, Quaternion.identity);
                counter.transform.SetParent(canvas.transform);
                //var counter = Instantiate(counterPrefab, canvas.transform);
                _countersList.Add(counter);
                
                int index = planet.ID.GetHashCode();
                _foregrounds.Add(index, counter.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>());
                _backgrounds.Add(index, counter.GetComponentInChildren<Image>());
            }
        }

        private Vector3 GetCounterPos(Base planet)
        {
            Vector3 pos = planet.transform.position;
            _offsetCamera = FindOffset(pos);
            Vector3 screenPos = _mainCamera.WorldToScreenPoint(pos + _offsetCounter);
            return screenPos + _offsetCamera;
        }


        private Vector3 FindOffset(Vector3 worldPos)
        {
            int coefficient = _mainCamera.pixelHeight / _mainCamera.pixelWidth;
            var screenPos = _mainCamera.WorldToScreenPoint(worldPos);
            var depth = screenPos.z;
            var offsetY=(_minDepth-depth)/ (_maxDepth-_minDepth)*80.0f;
            var offsetX=(_minDepth-depth)/ (_maxDepth-_minDepth)*(80.0f/coefficient);
  
            var res = new Vector3(offsetX, offsetY, 0);
     
            return res;
        }
        
        
        private void SetAllCountersColor()
        {
            foreach (Base planet in _allPlanets)
            {
                SetUnitCounterColor(planet);
            }
        }

        public void SetUnitCounterColor(Planets.Base planet)
        {
            var team = (int) planet.Team;
            int index = planet.ID.GetHashCode();
            _foregrounds[index].color = counterForeground[team];
            _backgrounds[index].color = counterBackground[team];
        }
        
        public void SetUnitCounter(Planets.Base planet, int value)
        {
            int index = planet.ID.GetHashCode();
            _foregrounds[index].text = value.ToString();
        }

    }
}