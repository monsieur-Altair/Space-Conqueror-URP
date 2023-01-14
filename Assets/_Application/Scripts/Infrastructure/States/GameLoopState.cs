using _Application.Scripts.Infrastructure.Services.Factory;
using _Application.Scripts.Managers;

namespace _Application.Scripts.Infrastructure.States
{
    public class GameLoopState : IState
    {
        private readonly StateMachine _stateMachine;
        private readonly GameLoopManager _gameLoopManager;

        public GameLoopState(StateMachine stateMachine, GameFactory gameFactory)
        {
            _stateMachine = stateMachine;
            _gameLoopManager = gameFactory.CreateWorld();
        }

        public void Exit()
        {
            
        }

        public void Enter()
        {
            _gameLoopManager.StartGame();
        }
    }
}