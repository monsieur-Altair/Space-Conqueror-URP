using System;
using Pool_And_Particles;
using UnityEngine;

namespace _Application.Scripts.Managers
{
    [CreateAssetMenu(fileName = "pool objects", menuName = "Resources/pool objects", order = 0)]
    public class PoolObjects : ScriptableObject
    {
        [SerializeField] private MyDictionary<PoolObjectType, Pool> _pooledPrefabs;

        public MyDictionary<PoolObjectType, Pool> PooledPrefabs => _pooledPrefabs;
    }
    
    [Serializable]
    public class Pool
    {
        public PooledBehaviour prefab;
        public int size;
    }
    
    public enum PoolObjectType
    {
        AltarWarrior = 0,
        SpawnerWarrior = 1,
        AttackerWarrior = 2,
        AltarBuilding = 3,
        SpawnerBuilding = 4,
        AttackerBuilding = 5,
        Counter = 6, 
        Rain = 7, 
        Indicator = 8,
        Ice = 9,
        WorldArrow = 10,
        
    }
}