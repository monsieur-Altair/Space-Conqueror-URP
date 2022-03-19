using _Application.Scripts.Infrastructure.States;
using UnityEngine;
using UnityEngine.UI;

namespace _Application.Scripts.Infrastructure
{
    public class GameBootstrapper : MonoBehaviour
    {
        [SerializeField] private Button playButton;
        private Game _game;
        

        private void Awake()
        {
            _game = new Game(this);
            _game.StateMachine.Enter<BootstrapState>();
            playButton.onClick.AddListener(EnterLoadLevel);
            DontDestroyOnLoad(this);
        }

        private void EnterLoadLevel() => 
            _game.StateMachine.Enter<LoadLevelState, string>("Main");
    }
}