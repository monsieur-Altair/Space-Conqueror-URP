using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


namespace AI
{ 
    public class Core : MonoBehaviour
    {
        private Planets.Base mainPlanet;
        [SerializeField] private GameObject allActions;
        
        public Vector3 MainPos { get; private set; }
        public List<List<Planets.Base>> AllPlanets { get; private set; }
        public static int Own, Enemy, Neutral;
        private bool _isActive = false;
        public static Core Instance { get; private set; }

        private IAction _attackByRocket;
        public static float ScientificCount;

        private const float MinDelay = 4.0f;
        private const float MaxDelay = 7.0f;

        //[SerializeField]private int actionSkillPercent = 25;
        
        
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
            
            for (var i = 0; i < 3; i++)
                AllPlanets.Add(new List<Planets.Base>());


            int ii = 0;
            foreach (var planet in planets)
            {
                AllPlanets[(int)planet.Team].Add(planet);
                Debug.Log(ii++ +"team="+planet.Team);
            }

            Debug.Log("Own="+ AllPlanets[Own].Count);
            mainPlanet = AllPlanets[Own][0];
            MainPos = mainPlanet.transform.position;

            _attackByRocket = allActions.GetComponent<AttackSomePlanet>();
            _attackByRocket.InitAction();
            if (_attackByRocket==null)
            {
                throw new MyException("attack by rocket = null");
            }

            ScientificCount = 50.0f;/////////////////////////////////////////////
            Debug.Log("Init is ended");
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
                var delay = Random.Range(MinDelay, MaxDelay);
                yield return new WaitForSeconds(delay);
                if(_isActive)
                    _attackByRocket.Execute();
            }
        }

        /*private IAction FindAction()
        {
            var decisionValue = Random.Range(1,101);//1-100
            if (decisionValue <= actionSkillPercent)
                return _attackBySkill;
            return _attackByRocket;
        }*/

        private void AdjustPlanetsList(Planets.Base planet, Planets.Team oldTeam, Planets.Team newTeam)
        {
            AllPlanets[(int) oldTeam].Remove(planet);
            AllPlanets[(int) newTeam].Add(planet);
        }

        /*public void IncreaseScientificCount(float value)
        {
            
        }*/
        
    }
}
