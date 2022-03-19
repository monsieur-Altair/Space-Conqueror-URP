﻿using System.Collections.Generic;
using _Application.Scripts.Misc;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Application.Scripts.Managers
{
    public class UI
    {
       // public static UI Instance { get; private set; }
        
        private readonly Canvas _canvas;
        private readonly ObjectPool _pool;
        private readonly Warehouse _warehouse;

        private readonly List<Color> _counterBackground;
        private readonly List<Color> _counterForeground;

        private List<Planets.Base> _allPlanets = new List<Planets.Base>();
        private readonly List<GameObject> _countersList = new List<GameObject>();
        private readonly Dictionary<int, Image> _backgrounds = new Dictionary<int, Image>();
        private readonly Dictionary<int, TextMeshProUGUI> _foregrounds = new Dictionary<int, TextMeshProUGUI>();

        private GameObject _nextLevelButton;
        private GameObject _retryLevelButton;
        private GameObject _scientificBar;
        private GameObject _teamBar;
        
        private readonly Vector3 _offset = new Vector3(0, -35, 0);

        public UI(Canvas canvas, ObjectPool pool, Warehouse warehouse)
        {
            _canvas = canvas;
            _pool = pool;
            _warehouse = warehouse;

            _counterBackground = _warehouse.counterBackground;
            _counterForeground = _warehouse.counterForeground;
        }

        public void PrepareLevel()
        {
            ClearList();
            _allPlanets = new List<Planets.Base>(Main.Instance.AllPlanets);
            FillLists();
        }
        
        public void SetButtons(GameObject retryButton, GameObject nextLevelButton)
        {
            _retryLevelButton = retryButton;
            _nextLevelButton = nextLevelButton;

            _retryLevelButton.SetActive(false);
            _nextLevelButton.SetActive(false);
        }

        public void SetUIBehaviours(TeamManager teamManager ,UnityAction retryLevelBehaviour, UnityAction loadNextLevelBehaviour)
        {
            _retryLevelButton.GetComponent<Button>().onClick.AddListener(retryLevelBehaviour);
            _nextLevelButton.GetComponent<Button>().onClick.AddListener(loadNextLevelBehaviour);

            teamManager.TeamCountUpdated += _teamBar.GetComponent<TeamProgressBar>().FillTeamCount;
            Planets.Scientific.ScientificCountUpdated += _scientificBar.GetComponent<ScientificBar>().FillScientificCount;
        }

        public void RemoveBehaviours(TeamManager teamManager)
        {
            Planets.Base.Conquered -= teamManager.UpdateObjectsCount;
            //_teamManager.TeamCountUpdated -= _teamBar.GetComponent<TeamProgressBar>().FillTeamCount;
            //null refs?
            //Planets.Scientific.ScientificCountUpdated -= _scientificBar.GetComponent<ScientificBar>().FillScientificCount;
        }
        
        public void SetBars(GameObject scientificBar, GameObject teamBar)
        {
            _scientificBar = scientificBar;
            _teamBar = teamBar;
        }

        public void ShowGameplayUI()
        {
            _nextLevelButton.SetActive(false);
            _retryLevelButton.SetActive(false);
            
            _scientificBar.SetActive(true);
            _teamBar.SetActive(true);
        }

        public void ShowGameOverUI(bool isWin)
        {
            if (isWin)
                _nextLevelButton.SetActive(true);
            else
                _retryLevelButton.SetActive(true);
            
            _scientificBar.SetActive(false);
            _teamBar.SetActive(false);
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
                SetCounter(planet, (int) planet.Count);
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
            Vector3 screenPos = CameraResolution.GetScreenPos(pos);
            return screenPos + _offset;
        }

        private void SetCounterColor(Planets.Base planet)
        {
            int team = (int) planet.Team;
            int index = planet.ID.GetHashCode();
            _foregrounds[index].color = _counterForeground[team];
            _backgrounds[index].color = _counterBackground[team];
        }
        
        private void SetCounter(Planets.Base planet, int value)
        {
            int index = planet.ID.GetHashCode();
            _foregrounds[index].text = value.ToString();
        }

    }
}