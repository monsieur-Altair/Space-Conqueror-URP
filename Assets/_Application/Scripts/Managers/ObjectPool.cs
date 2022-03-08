using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Application.Scripts.Managers
{
    public class ObjectPool : MonoBehaviour
    {
        private Dictionary<int, Queue<GameObject>> _poolDictionary;

        public enum PoolObjectType
        {
            ScientificRocket = 0,
            SpawnerRocket = 1,
            AttackerRocket = 2,
            ScientificPlanet = 3,
            SpawnerPlanet = 4,
            AttackerPlanet = 5,
            Counter = 6
        }

        [Serializable]
        public class Pool
        {
            public PoolObjectType poolObjectType;
            public GameObject prefab;
            public int size;
        }
        
        public List<Pool> pools;
        public static ObjectPool Instance;
        public void Awake()
        {
            if (Instance == null)
                Instance = this;
            _poolDictionary = new Dictionary<int, Queue<GameObject>>();
        }

        public void Start()
        {
            foreach (Pool pool in pools)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>(); 
                for (int i = 0; i < pool.size; i++)
                {
                    GameObject unit = Instantiate(pool.prefab,transform);
                    unit.SetActive(false);
                    objectPool.Enqueue(unit);
                }
                _poolDictionary.Add(pool.poolObjectType.GetHashCode(),objectPool);
            }
        }

        public GameObject GetObject(PoolObjectType type, Vector3 position, Quaternion rotation)
        {
            int hash=type.GetHashCode();
            if (_poolDictionary.ContainsKey(hash) == false)
                throw new MyException("doesn't exist key value");

            GameObject obj = _poolDictionary[hash].Dequeue();
            obj.SetActive(true);
            obj.transform.position = position;
            obj.transform.rotation = rotation;

            _poolDictionary[hash].Enqueue(obj);

            return obj;
        }

        public void DisableAllUnitsInScene()
        {
            foreach (KeyValuePair<int, Queue<GameObject>> pair in _poolDictionary)
            {
                foreach (GameObject unit in pair.Value)
                {
                    if(unit.activeSelf)
                        unit.SetActive(false);
                }
            }
        }
    }
}