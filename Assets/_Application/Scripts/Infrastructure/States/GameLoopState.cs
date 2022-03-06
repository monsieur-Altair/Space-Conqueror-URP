namespace _Application.Scripts.Infrastructure.States
{
    public class GameLoopState : IState
    {
        private readonly StateMachine _stateMachine;

        public GameLoopState(StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void Exit()
        {
            
        }

        public void Enter()
        {
            
        }
    }
}