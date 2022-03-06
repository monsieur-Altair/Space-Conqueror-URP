﻿using System.Collections.Generic;
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
        
        [SerializeField] private GameObject counterPrefab;
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
                Destroy(counter);
            }

            if (Camera.main == null)
                throw new MyException("main camera = null");
            foreach (var planet in _allPlanets)
            {
                planet.SetUIManager();
                var pos = planet.transform.position;
                var counter = Instantiate(counterPrefab, canvas.transform);
                _countersList.Add(counter);
                _offsetCamera = FindOffset(pos);
                var screenPos = _mainCamera.WorldToScreenPoint(pos+_offsetCounter);
                counter.transform.position = screenPos + _offsetCamera;
                var index = planet.ID.GetHashCode();
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
  
            var res = new Vector3(offsetX, offsetY, 0);
     
            return res;
        }
        
        
        private void SetAllCountersColor()
        {
            foreach (var planet in _allPlanets)
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