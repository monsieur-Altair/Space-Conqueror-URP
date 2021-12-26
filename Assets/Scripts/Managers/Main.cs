using System;
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

        [SerializeField] private GameObject butObj;
        private Button _buttonToNext;
        //private AI.AI _ai;
        public List<Planets.Base> AllPlanets { get; private set; }
        private GameObject planetsLay;
        public List<int> ObjectsCount { get; private set; }

        public GameStates GameState { get; private set; }
        private ObjectPool _objectPool;
        private Levels _levelsManager;
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
            ObjectsCount=new List<int>(_teamNumber);
            for (int i = 0; i < _teamNumber;i++)
                ObjectsCount.Add(0);
            
            _levelsManager=Levels.Instance;
            if (_levelsManager == null)
                throw new MyException("cannot get level manager");

            _buttonToNext = butObj.GetComponent<Button>();
            butObj.SetActive(false);
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
            AllPlanets = planetsLay.GetComponentsInChildren<Planets.Base>().ToList();
            
            foreach (var planet in AllPlanets)
            {
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
                    butObj.SetActive(false);
                    _levelsManager.LoadCurrentLevel();
                    planetsLay = _levelsManager.GetCurrentLay();
                    this.PrepareLevel();
                    core.Init(AllPlanets);
                    core.Enable();
                    Outlook.Instance.PrepareLevel();
                    UI.Instance.PrepareLevel();
                    break;
                }
                case GameStates.GameOver:
                {
                    Debug.Log("Ended");
                    _objectPool.DisableAllUnitsInScene();
                    core.Disable();
                    butObj.SetActive(true);
                    break;
                }
            }
        }

        public void LoadNextLevel()
        {
            _levelsManager.SwitchToNextLevel();
            GameState = GameStates.Gameplay;
            UpdateState();
        }
        
        
        private void FillTeamCount()
        {
            foreach (var planet in AllPlanets)
            {
                var team = (int) planet.Team;
                //Debug.Log(team);
                ObjectsCount[team]++;
            }
        }

        public void UpdateObjectsCount(Planets.Team oldTeam, Planets.Team newTeam)
        {
            ObjectsCount[(int) oldTeam]--;
            ObjectsCount[(int) newTeam]++;
        }

        public void CheckGameOver()
        {
            var noneBlue = ObjectsCount[(int)Planets.Team.Blue]==0;
            var noneRed = ObjectsCount[(int)Planets.Team.Red]==0;
            if (noneBlue || noneRed )
            {
                GameState = GameStates.GameOver;
                UpdateState();
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