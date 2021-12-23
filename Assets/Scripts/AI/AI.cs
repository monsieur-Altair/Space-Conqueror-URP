using System;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{ 
    public class AI : MonoBehaviour
    {
        //private List<Planets.Base> _ownPlanets=new List<Planets.Base>();
        //private List<Planets.Base> _enemyPlanets=new List<Planets.Base>();
        //private List<Planets.Base> _neutralPlanets=new List<Planets.Base>();
        [SerializeField] private Planets.Base mainPlanet;
        [SerializeField] private GameObject allActions;
        
        public Vector3 MainPos { get; private set; }
        public List<List<Planets.Base>> AllPlanets { get; private set; }// = new List<List<Planets.Base>>();
        public static int Own, Enemy, Neutral;
        private bool _isActive = false;
        public static AI Instance { get; private set; }

        private IAction _action; 
       
        
        
        public void Awake()
        {
            if (Instance == null)
                Instance = this;
            
            AllPlanets = new List<List<Planets.Base>>();
            MainPos = mainPlanet.transform.position;

            Own = (int) Planets.Team.Red;
            Enemy = (int) Planets.Team.Blue;
            Neutral = (int) Planets.Team.White;

            Planets.Base.Conquered += AdjustPlanetsList;
        }

        public void Init(List<Planets.Base> planets)
        {
            for (var i = 0; i < 3; i++)
                AllPlanets.Add(new List<Planets.Base>());
            
            foreach (var planet in planets)
            {
                AllPlanets[(int)planet.Team].Add(planet);
            }
            



            _action = allActions.GetComponent<AttackSomePlanet>();
            _action.InitAction();
            if (_action==null)
            {
                throw new MyException("action = null");
            }
        }

        //attack one to one
        //attack many to one
        //attack after lost
        //firstly attack neutral
        //attack immediately 
        //firstly attack scientific
        
        
        public void Enable()
        {
            _isActive = true;
            //DoSomeAction();
            InvokeRepeating(nameof(DoSomeAction),2,2);
        }

        private void DoSomeAction()
        {
            FindAction().Execute();
        }

        private IAction FindAction()
        {
            return _action;
        }

        private void AdjustPlanetsList(Planets.Base planet, Planets.Team oldTeam, Planets.Team newTeam)
        {
            AllPlanets[(int) oldTeam].Remove(planet);
            AllPlanets[(int) newTeam].Add(planet);
        }
        
    }
}
