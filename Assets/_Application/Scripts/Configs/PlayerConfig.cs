using System;
using System.Collections;
using System.Linq;
using _Application.Scripts.Buildings;
using _Application.Scripts.Control;
using _Application.Scripts.Scriptables;
using UnityEngine;

namespace _Application.Scripts.Managers
{
    [CreateAssetMenu (fileName = "player",menuName = "Resources/Player Config")]
    public class PlayerConfig : ScriptableObject
    {
        [SerializeField] private MyDictionary<SkillName, Skill> _skills;
        [Space, SerializeField] private MyDictionary<BuildingType, Building> _buildings;
        [Space, SerializeField] private MyDictionary<BuildingType, Unit> _units;
        [Space, SerializeField] private Mana _mana;

        public MyDictionary<SkillName, Skill> Skills => _skills;
        public MyDictionary<BuildingType, Building> Buildings => _buildings;
        public MyDictionary<BuildingType, Unit> Units => _units;
        public Mana ManaConfig => _mana;
    }
    
    [Serializable]
    public class Pair<TKey, TValue> where TKey : Enum 
    {
        [SerializeField] private TKey _key;
        [SerializeField] private TValue _value;

        public TValue Value => _value;
        public TKey Key => _key;
    }

    [Serializable] 
    public class MyDictionary<TKey, TValue> where TKey : Enum 
    {
        [SerializeField] private Pair<TKey, TValue>[] _pairs;
            
        public TValue GetValue(TKey key)
        {
            return _pairs.First(pair => pair.Key.Equals(key)).Value;
        }
        public Pair<TKey, TValue>[] Pairs => _pairs;
    } 
}