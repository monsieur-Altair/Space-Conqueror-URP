using System.Collections;
using System.Collections.Generic;
using _Application.Scripts.Infrastructure;
using UnityEngine;
using Random = UnityEngine.Random;


namespace _Application.Scripts.AI
{ 
    public class Core
    {
        public static float ManaCount;
        
        public const int Own = (int) Buildings.Team.Red;
        public const int Enemy = (int) Buildings.Team.Blue;
        public const int Neutral = (int) Buildings.Team.White;

        private readonly List<List<Buildings.Base>> _allBuildings;
        private readonly IAction _attackByWarrior;
        private readonly ICoroutineRunner _coroutineRunner;

        private const float MinDelay = 4.0f;
        private const float MaxDelay = 7.0f;

        
        public Core(ICoroutineRunner coroutineRunner, SkillController skillController)
        {
            _coroutineRunner = coroutineRunner;
            _allBuildings = new List<List<Buildings.Base>>();
            _attackByWarrior = new AttackSomeBuilding(_coroutineRunner, skillController);
            
            Buildings.Base.Conquered += AdjustBuildingsList;
        }

        public void Init(List<Buildings.Base> planets)
        {
            _allBuildings.Clear();
            
            for (int i = 0; i < 3; i++)
                _allBuildings.Add(new List<Buildings.Base>());
            
            foreach (Buildings.Base planet in planets) 
                _allBuildings[(int) planet.Team].Add(planet);

            Buildings.Base mainBuilding = _allBuildings[Own][0];
            Vector3 mainPos = mainBuilding.transform.position;

            _attackByWarrior.InitAction(_allBuildings, mainPos);

            ManaCount = 0.0f;
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
                //Debug.LogError($"AI thinks {delay} seconds...");
                yield return new WaitForSeconds(delay);
                _attackByWarrior.Execute();
            }
        }

        private void AdjustBuildingsList(Buildings.Base building, Buildings.Team oldTeam, Buildings.Team newTeam)
        {
            _allBuildings[(int) oldTeam].Remove(building);
            _allBuildings[(int) newTeam].Add(building);
        }
    }
    
    //attack after lost
    //firstly attack neutral
    //attack immediately 
    //firstly attack scientific
}
