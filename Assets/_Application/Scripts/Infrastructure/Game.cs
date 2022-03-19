using _Application.Scripts.Control;
using _Application.Scripts.Infrastructure.Services;
using _Application.Scripts.Infrastructure.States;
using UnityEngine;

namespace _Application.Scripts.Infrastructure
{
    public class Game
    {
        public readonly StateMachine StateMachine;
        public Game(MonoBehaviour behaviour) => 
            StateMachine = new StateMachine(new SceneLoader(behaviour), AllServices.Instance);
    }
}