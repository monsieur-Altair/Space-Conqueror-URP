using System.Collections.Generic;
using _Application.Scripts.Infrastructure.Services;
using UnityEngine;

namespace _Application.Scripts.Managers
{
    [CreateAssetMenu (fileName = "CoreConfig",menuName = "Resources/Core Config")]
    public class CoreConfig: ScriptableObject, IService
    {
        [SerializeField] private MonoBehaviourServices _monoBehaviourServices;
        [Space, SerializeField] private bool _useTutorial;
        
        public MonoBehaviourServices MonoBehaviourServices => _monoBehaviourServices;
        public bool UseTutorial => _useTutorial;
    }
}