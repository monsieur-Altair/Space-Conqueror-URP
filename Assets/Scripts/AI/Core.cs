﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


namespace AI
{ 
    public class Core : MonoBehaviour
    {
        public static float ScientificCount;
        public Vector3 MainPos { get; private set; }
        public List<List<Planets.Base>> AllPlanets { get; private set; }
        public static int Own, Enemy, Neutral;
        public static Core Instance { get; private set; }

        [SerializeField] private GameObject allActions;

        private Planets.Base _mainPlanet;
        private bool _isActive;
        private IAction _attackByRocket;
        private const float MinDelay = 4.0f;
        private const float MaxDelay = 7.0f;

        
        public void Awake()
        {
            if (Instance == null)
                Instance = this;
            
            AllPlanets = new List<List<Planets.Base>>();
            

            Own = (int) Planets.Team.Red;
            Enemy = (int) Planets.Team.Blue;
            Neutral = (int) Planets.Team.White;

            Planets.Base.Conquered += AdjustPlanetsList;
        }

        public void Init(List<Planets.Base> planets)
        {
            AllPlanets.Clear();
            
            for (int i = 0; i < 3; i++)
                AllPlanets.Add(new List<Planets.Base>());


            foreach (Planets.Base planet in planets)
            {
                AllPlanets[(int)planet.Team].Add(planet);
            }

            _mainPlanet = AllPlanets[Own][0];
            MainPos = _mainPlanet.transform.position;

            _attackByRocket = allActions.GetComponent<AttackSomePlanet>();
            _attackByRocket.InitAction();
            if (_attackByRocket==null)
            {
                throw new MyException("attack by rocket = null");
            }

            ScientificCount = 0.0f;/////////////////////////////////////////////
        }
        
        //attack after lost
        //firstly attack neutral
        //attack immediately 
        //firstly attack scientific
        
        public void Enable()
        {
            _isActive = true;
            StartCoroutine(DoSomeAction());
        }

        public void Disable()
        {
            _isActive = false;
        }

        private IEnumerator DoSomeAction()
        {
            while (_isActive)
            {
                float delay = Random.Range(MinDelay, MaxDelay);
                yield return new WaitForSeconds(delay);
                if(_isActive)
                    _attackByRocket.Execute();
            }
        }

        private void AdjustPlanetsList(Planets.Base planet, Planets.Team oldTeam, Planets.Team newTeam)
        {
            AllPlanets[(int) oldTeam].Remove(planet);
            AllPlanets[(int) newTeam].Add(planet);
        }
    }
}
