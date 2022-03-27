using System.Collections;
using System.Collections.Generic;
using _Application.Scripts.Infrastructure;
using UnityEngine;
using Random = UnityEngine.Random;


namespace _Application.Scripts.AI
{ 
    public class Core
    {
        public static float ScientificCount;
        
        public const int Own = (int) Planets.Team.Red;
        public const int Enemy = (int) Planets.Team.Blue;
        public const int Neutral = (int) Planets.Team.White;

        private readonly List<List<Planets.Base>> _allPlanets;
        private readonly IAction _attackByRocket;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly SkillController _skillController;

        private const float MinDelay = 4.0f;
        private const float MaxDelay = 7.0f;

        
        public Core(ICoroutineRunner coroutineRunner, SkillController skillController)
        {
            _coroutineRunner = coroutineRunner;
            _allPlanets = new List<List<Planets.Base>>();
            _skillController = skillController;
            _attackByRocket = new AttackSomePlanet(_coroutineRunner, _skillController);
            
            Planets.Base.Conquered += AdjustPlanetsList;
        }

        public void Init(List<Planets.Base> planets)
        {
            _allPlanets.Clear();
            
            for (int i = 0; i < 3; i++)
                _allPlanets.Add(new List<Planets.Base>());
            
            foreach (Planets.Base planet in planets) 
                _allPlanets[(int) planet.Team].Add(planet);

            Planets.Base mainPlanet = _allPlanets[Own][0];
            Vector3 mainPos = mainPlanet.transform.position;

            _skillController.Refresh();
            _attackByRocket.InitAction(_allPlanets, mainPos);

            ScientificCount = 0.0f;
        }
        
        public void Enable() => 
            _coroutineRunner.StartCoroutine(DoSomeAction());

        public void Disable() => 
            _coroutineRunner.StopCoroutine(DoSomeAction());

        private IEnumerator DoSomeAction()
        {
            while (true)
            {
                float delay = Random.Range(MinDelay, MaxDelay);
                Debug.LogError($"AI thinks {delay} seconds...");
                yield return new WaitForSeconds(delay);
                _attackByRocket.Execute();
            }
        }

        private void AdjustPlanetsList(Planets.Base planet, Planets.Team oldTeam, Planets.Team newTeam)
        {
            _allPlanets[(int) oldTeam].Remove(planet);
            _allPlanets[(int) newTeam].Add(planet);
        }
    }
    
    //attack after lost
    //firstly attack neutral
    //attack immediately 
    //firstly attack scientific
}
