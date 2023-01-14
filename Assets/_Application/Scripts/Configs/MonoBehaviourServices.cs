using UnityEngine;

namespace _Application.Scripts.Managers
{
    [CreateAssetMenu (fileName = "MonoBehaviourServices",menuName = "Resources/MonoBehaviourServices")]
    public class MonoBehaviourServices : ScriptableObject
    {
        [SerializeField] private AudioManager _audioManager;
        [SerializeField] private CoroutineRunner _coroutineRunner;
        [SerializeField] private ObjectPool _objectPool;
        [SerializeField] private LevelManager _levelManager;

        public AudioManager AudioManager => _audioManager;
        public CoroutineRunner CoroutineRunner => _coroutineRunner;
        public ObjectPool ObjectPool => _objectPool;
        public LevelManager LevelManager => _levelManager;
    }
}