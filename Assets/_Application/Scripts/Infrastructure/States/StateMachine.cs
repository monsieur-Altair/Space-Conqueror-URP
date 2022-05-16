using System;
using System.Collections.Generic;
using _Application.Scripts.Infrastructure.Services;
using _Application.Scripts.Infrastructure.Services.Factory;
using _Application.Scripts.Infrastructure.Services.Progress;

namespace _Application.Scripts.Infrastructure.States
{
    public class StateMachine
    {
        private readonly Dictionary<Type, IBaseState> _states;
        private IBaseState _activeState;

        public StateMachine(SceneLoader sceneLoader, AllServices allServices)
        {
            _states = new Dictionary<Type, IBaseState>()
            {
                [typeof(BootstrapState)] = new BootstrapState(this, sceneLoader, allServices),
                [typeof(LoadLevelState)] = new LoadLevelState(this, sceneLoader, allServices.GetSingle<IGameFactory>(),allServices.GetSingle<IProgressService>()),
                [typeof(GameLoopState)] = new GameLoopState(this)
            };
        }

        public void Enter<TState>() where TState : class, IState
        {
            _activeState?.Exit();
            IState state = GetState<TState>();
            _activeState = state;
            state.Enter();
        }

        public void Enter<TState, TPayload>(TPayload payload) where TState : class, IStateWithPayload<TPayload>
        {
            _activeState?.Exit();
            IStateWithPayload<TPayload> state = GetState<TState>();
            _activeState = state;
            state.Enter(payload);
        }

        private TState GetState<TState>() where TState : class, IBaseState => 
            _states[typeof(TState)] as TState;
    }
}