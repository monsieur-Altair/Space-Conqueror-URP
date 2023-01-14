using _Application.Scripts.Control;
using _Application.Scripts.Infrastructure.Services;
using _Application.Scripts.UI;
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
        [SerializeField] private GlobalCamera _globalCamera;
        [SerializeField] private UISystem _uiSystem;
        [SerializeField] private UserControl _userControl;

        public AudioManager AudioManager => _audioManager;
        public CoroutineRunner CoroutineRunner => _coroutineRunner;
        public ObjectPool ObjectPool => _objectPool;
        public LevelManager LevelManager => _levelManager;
        public GlobalCamera GlobalCamera => _globalCamera;
        public UISystem UISystem => _uiSystem;
        public UserControl UserControl => _userControl;
    }
}