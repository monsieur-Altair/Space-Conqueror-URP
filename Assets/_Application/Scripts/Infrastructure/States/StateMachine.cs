using System;
using System.Collections.Generic;
using _Application.Scripts.States;

namespace _Application.Scripts.Infrastructure.States
{
    public class StateMachine
    {
        private readonly Dictionary<Type, IState> _states;
        private IState _activeState;

        public StateMachine()
        {
            _states = new Dictionary<Type, IState>();
        }

        public void Enter<TState>() where TState : IState
        {
            _activeState?.Exit();
            IState state = _states[typeof(TState)];
            _activeState = state;
            state.Enter();
        }
    }
}