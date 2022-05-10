using _Application.Scripts.Control;
using _Application.Scripts.Infrastructure.Services;
using _Application.Scripts.Infrastructure.Services.Factory;
using _Application.Scripts.Infrastructure.States;
using _Application.Scripts.Misc;
using _Application.Scripts.UI;
using UnityEngine;

namespace _Application.Scripts.Infrastructure
{
    public class Game
    {
        public readonly StateMachine StateMachine;
        public Game(MonoBehaviour behaviour)
        {
            StateMachine = new StateMachine(new SceneLoader(behaviour), AllServices.Instance);
            UISystem uiSystem = AllServices.Instance.GetSingle<IGameFactory>().CreateUISystem();
            uiSystem.Setup();
        }
    }
}