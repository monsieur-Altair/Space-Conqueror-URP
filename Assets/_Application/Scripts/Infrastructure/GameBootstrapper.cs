using System.Security.Cryptography;
using _Application.Scripts.Infrastructure.States;
using _Application.Scripts.UI;
using _Application.Scripts.UI.Windows;
using UnityEngine;
using UnityEngine.UI;

namespace _Application.Scripts.Infrastructure
{
    public class GameBootstrapper : MonoBehaviour
    {
        private Game _game;
        
        private void Awake()
        {
            _game = new Game(this);
            _game.StateMachine.Enter<BootstrapState>();
            UISystem.ShowWindow<StartUpWindow>();
            UISystem.GetWindow<StartUpWindow>().PlayButton.onClick.AddListener(EnterLoadLevel);
            DontDestroyOnLoad(this);
        }

        private void EnterLoadLevel() => 
            _game.StateMachine.Enter<LoadLevelState, string>("Main");
    }
}