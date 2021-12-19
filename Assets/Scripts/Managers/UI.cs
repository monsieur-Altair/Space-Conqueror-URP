using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    [DefaultExecutionOrder(600)]
    public class UI : MonoBehaviour
    {
        public static UI Instance { get; private set; }
        
        [SerializeField] private GameObject counterPrefab;
        [SerializeField] private Canvas canvas;
        
        [SerializeField] private List<Color> counterBackground;
        [SerializeField] private List<Color> counterForeground;

        [SerializeField] private TextMeshProUGUI textScientific;
        
        private List<Planets.Base> _allPlanets => Main.Instance.AllPlanets;
        private Camera _mainCamera;
        
        private float _maxDepth;
        private float _minDepth;

        // private Dictionary<int,GameObject> _counter=new Dictionary<int, GameObject>();
        private readonly Dictionary<int,Image> _backgrounds=new Dictionary<int, Image>();
        private readonly Dictionary<int,TextMeshProUGUI> _foregrounds=new Dictionary<int, TextMeshProUGUI>();

        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }
        private Vector3 _offsetCamera;
        private Vector3 _offsetCounter;

        private void Start()
        {
            Control.SkillController.GetCameraDepths(out _minDepth, out _maxDepth);

            _mainCamera=Camera.main;
            if (_mainCamera == null)
                throw new MyException("can't get camera component");
            
            _offsetCamera = new Vector3(0, -1.0f, 0);
            _offsetCounter = new Vector3(0, 0, 0);
            FillLists();
            SetAllCountersColor();
        }
        
        private void FillLists()
        {
            if (Camera.main == null)
                throw new MyException("main camera = null");
            foreach (var planet in _allPlanets)
            {
                var pos = planet.transform.position;
                var counter = Instantiate(counterPrefab, canvas.transform);
                _offsetCamera = FindOffset(pos);
                var screenPos = _mainCamera.WorldToScreenPoint(pos+_offsetCounter);
                //Debug.Log("screen = "+screenPos);
                counter.transform.position = screenPos + _offsetCamera;
                var index = planet.ID.GetHashCode();
                //_counter.Add(index,counter);
                _foregrounds.Add(index, counter.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>());
                _backgrounds.Add(index, counter.GetComponentInChildren<Image>());
            }
        }
        

        private Vector3 FindOffset(Vector3 worldPos)
        {
            int coef = _mainCamera.pixelHeight / _mainCamera.pixelWidth;
            var screenPos = _mainCamera.WorldToScreenPoint(worldPos);
            var depth = screenPos.z;
            var offsetY=(_minDepth-depth)/ (_maxDepth-_minDepth)*80.0f;
            var offsetX=(_minDepth-depth)/ (_maxDepth-_minDepth)*(80.0f/coef);
            //var offsetY=(MINDepth-depth)/ (MAXDepth-MINDepth);
            var res = new Vector3(offsetX, offsetY, 0);
            //Debug.Log(res+" "+MAXDepth+" "+MINDepth);
            return res;
        }
        
        
        private void SetAllCountersColor()
        {
            foreach (var planet in _allPlanets)
            {
                SetUnitCounterColor(planet);
            }
            
            textScientific.color=Color.blue;
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

        public void SetScientificCounter(int value)
        {
            textScientific.text = value.ToString();
        }
    }
}