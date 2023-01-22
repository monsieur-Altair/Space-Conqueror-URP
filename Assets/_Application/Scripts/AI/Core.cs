using System.Collections;
using System.Collections.Generic;
using _Application.Scripts.Infrastructure;
using _Application.Scripts.Managers;
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

        private readonly List<List<Buildings.BaseBuilding>> _allBuildings;
        private readonly IAction _attackByWarrior;
        private readonly CoroutineRunner _coroutineRunner;

        private const float MinDelay = 4.0f;
        private const float MaxDelay = 7.0f;

        
        public Core(CoroutineRunner coroutineRunner, AISkillController aiSkillController)
        {
            _coroutineRunner = coroutineRunner;
            _allBuildings = new List<List<Buildings.BaseBuilding>>();
            _attackByWarrior = new AttackSomeBuilding(_coroutineRunner, aiSkillController);
            
            Buildings.BaseBuilding.Conquered += AdjustBuildingsList;
        }

        public void Init(List<Buildings.BaseBuilding> planets)
        {
            _allBuildings.Clear();
            
            for (int i = 0; i < 3; i++)
                _allBuildings.Add(new List<Buildings.BaseBuilding>());
            
            foreach (Buildings.BaseBuilding planet in planets) 
                _allBuildings[(int) planet.Team].Add(planet);

            Buildings.BaseBuilding mainBuilding = _allBuildings[Own][0];
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
                yield return new WaitForSeconds(delay);
                _attackByWarrior.Execute();
            }
        }

        private void AdjustBuildingsList(Buildings.BaseBuilding building, Buildings.Team oldTeam, Buildings.Team newTeam)
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
