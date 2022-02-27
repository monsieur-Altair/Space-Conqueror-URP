using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public enum GameStates
    {
        Opening,
        Gameplay,
        GameOver
    }
    
    [DefaultExecutionOrder(500)]
    public class Main : MonoBehaviour
    {
        private readonly int _teamNumber = Enum.GetNames(typeof(Planets.Team)).Length;
        [SerializeField] private AI.Core core;

        [SerializeField] private GameObject nextLevelButton;
        [SerializeField] private GameObject retryLevelButton;
        //private AI.AI _ai;
        public List<Planets.Base> AllPlanets { get; private set; } 
        private GameObject _planetsLay;
        public static List<int> _objectsCount { get; private set; }

        public GameStates GameState { get; private set; }
        private ObjectPool _objectPool;
        private Levels _levelsManager;

        private bool _isWin = false;
        /*private UI _uiManager;
        private Outlook _outlookManager;*/

        public static Main Instance;
        public void Awake()
        {
            Debug.Log("main awake");
            if (Instance == null)
            {
                Instance = this;
            }

            //Debug.Log(_teamNumber);
            _objectsCount=new List<int>(_teamNumber);
            for (int i = 0; i < _teamNumber;i++)
                _objectsCount.Add(0);
            
            _levelsManager=Levels.Instance;
            if (_levelsManager == null)
                throw new MyException("cannot get level manager");

            nextLevelButton.SetActive(false);
            retryLevelButton.SetActive(false);
        }
        
        public void OnEnable()
        {
            Debug.Log("enable main");
            core = core.GetComponent<AI.Core>();
            if (core==null)
            {
                throw new MyException("cannot get ai component");
            }
            _objectPool = ObjectPool.Instance;
            if (_objectPool==null)
            {
                throw new MyException("cannot get object pool component");
            }
            
            /*_uiManager = UI.Instance;
            if (_uiManager == null)
                throw new MyException("cannot get ui manager");
            
            _outlookManager = Outlook.Instance;
            if (_outlookManager == null)
                throw new MyException("cannot get outlook manager");*/
            
            GameState = GameStates.Gameplay;
            UpdateState();
            //PrepareLevel();
        }

        private void PrepareLevel()
        {
            AllPlanets = _planetsLay.GetComponentsInChildren<Planets.Base>().ToList();
            
            foreach (var planet in AllPlanets)
            {
                planet.gameObject.SetActive(true);
                planet.Init();
            }
            
            FillTeamCount();
            
        }
        
        

        public void UpdateState()
        {
            switch (GameState)
            {
                case GameStates.Opening:
                {
                    break;
                }
                case GameStates.Gameplay:
                {
                    
                    nextLevelButton.SetActive(false);
                    retryLevelButton.SetActive(false);
                    StartCoroutine(StartGameplay());
                    /*_levelsManager.LoadCurrentLevel();
                    _planetsLay = _levelsManager.GetCurrentLay();
                    this.PrepareLevel();
                    core.Init(AllPlanets);
                    core.Enable();
                    Outlook.Instance.PrepareLevel();
                    UI.Instance.PrepareLevel();
                    Planets.Scientific.DischargeScientificCount();
                    Control.UserControl.Instance.isActive = true;*/
                   // _levelsManager.BakeNavMesh();
                    break;
                }
                case GameStates.GameOver:
                {
                    Control.UserControl.Instance.isActive = false;
                    Debug.Log("Ended");
                    _objectPool.DisableAllUnitsInScene();
                    core.Disable();
                    if(_isWin)
                        nextLevelButton.SetActive(true);
                    else 
                        retryLevelButton.SetActive(true);
                    break;
                }
            }
        }

        private IEnumerator StartGameplay()
        {
            yield return StartCoroutine(_levelsManager.InstantiateLevel());
            _planetsLay = _levelsManager.GetCurrentLay();
            this.PrepareLevel();
            core.Init(AllPlanets);
            core.Enable();
            Outlook.Instance.PrepareLevel();
            UI.Instance.PrepareLevel();
            Planets.Scientific.DischargeScientificCount();
            Control.UserControl.Instance.isActive = true;
        }
        
        public void LoadNextLevel()
        {
            _levelsManager.SwitchToNextLevel();
            GameState = GameStates.Gameplay;
            UpdateState();
        }

        public void RetryLevel()
        {
            _levelsManager.RestartLevel();
            GameState = GameStates.Gameplay;
            UpdateState();
        }
        
        
        private void FillTeamCount()
        {
            _objectsCount.Clear();
            for (int i = 0; i < _teamNumber;i++)
                _objectsCount.Add(0);
            foreach (var planet in AllPlanets)
            {
                var team = (int) planet.Team;
                //Debug.Log(team);
                _objectsCount[team]++;
            }
        }

        public void UpdateObjectsCount(Planets.Team oldTeam, Planets.Team newTeam)
        {
            _objectsCount[(int) oldTeam]--;
            _objectsCount[(int) newTeam]++;
        }

        public void CheckGameOver()
        {
            var noneBlue = _objectsCount[(int)Planets.Team.Blue]==0;
            var noneRed = _objectsCount[(int)Planets.Team.Red]==0;
            if (noneBlue || noneRed )
            {
                _isWin = noneRed;
                GameState = GameStates.GameOver;
                UpdateState();
               
                //Debug.Log("iswin" + _isWin);
            }
            /*if (noneBlue)
            {
                Debug.Log("Game over");
            }

            if (noneRed)
            {
                Debug.Log("Win");
            }*/
            
            
        }
    }
}