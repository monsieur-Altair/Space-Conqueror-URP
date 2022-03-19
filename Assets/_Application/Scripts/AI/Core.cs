using System.Collections;
using System.Collections.Generic;
using _Application.Scripts.Planets;
using UnityEngine;
using Random = UnityEngine.Random;


namespace AI
{ 
    public class Core : MonoBehaviour
    {
        public static float ScientificCount;
        public Vector3 MainPos { get; private set; }
        public List<List<Base>> AllPlanets { get; private set; }
        public static int Own, Enemy, Neutral;
        public static Core Instance { get; private set; }

        [SerializeField] private GameObject allActions;

        private Base _mainPlanet;
        private bool _isActive;
        private IAction _attackByRocket;
        private const float MinDelay = 4.0f;
        private const float MaxDelay = 7.0f;

        
        public void Awake()
        {
            if (Instance == null)
                Instance = this;
            
            AllPlanets = new List<List<Base>>();

            Own = (int) Team.Red;
            Enemy = (int) Team.Blue;
            Neutral = (int) Team.White;

            Base.Conquered += AdjustPlanetsList;
        }

        public void Init(List<Base> planets)
        {
            AllPlanets.Clear();
            
            for (int i = 0; i < 3; i++)
                AllPlanets.Add(new List<Base>());


            foreach (Base planet in planets)
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

        private void AdjustPlanetsList(Base planet, Team oldTeam, Team newTeam)
        {
            AllPlanets[(int) oldTeam].Remove(planet);
            AllPlanets[(int) newTeam].Add(planet);
        }
    }
    
    //attack after lost
    //firstly attack neutral
    //attack immediately 
    //firstly attack scientific
}
