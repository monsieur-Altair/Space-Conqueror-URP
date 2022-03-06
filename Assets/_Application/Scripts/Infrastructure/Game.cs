using _Application.Scripts.Control;
using _Application.Scripts.Infrastructure.States;
using UnityEngine;

namespace _Application.Scripts.Infrastructure
{
    public class Game
    {
        public static IInputService InputService;
        public readonly StateMachine StateMachine;
        public Game(ICoroutineRunner coroutineRunner)
        {
            StateMachine = new StateMachine(new SceneLoader(coroutineRunner));
        }
        
    }
}