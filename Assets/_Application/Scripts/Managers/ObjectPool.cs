using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Application.Scripts.Managers
{
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
        Ice = 9
    }

    public class ObjectPool : MonoBehaviour, IObjectPool
    {
        public List<Pool> pools;
        
        private readonly Dictionary<int, Queue<GameObject>> _poolDictionary = new Dictionary<int, Queue<GameObject>>();

        [Serializable]
        public class Pool
        {
            public PoolObjectType poolObjectType;
            public GameObject prefab;
            public int size;
        }

        public void Init()
        {
            foreach (Pool pool in pools)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>(); 
                for (int i = 0; i < pool.size; i++)
                {
                    GameObject item = Instantiate(pool.prefab, transform);
                    item.SetActive(false);
                    objectPool.Enqueue(item);
                }
                _poolDictionary.Add(pool.poolObjectType.GetHashCode(),objectPool);
            }
            
            DontDestroyOnLoad(this);
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
                foreach (GameObject item in pair.Value)
                {
                    if(item.activeSelf)
                        item.SetActive(false);
                }
            }
        }
    }
}