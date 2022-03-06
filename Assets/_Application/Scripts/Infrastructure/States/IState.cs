namespace _Application.Scripts.Infrastructure.States
{
    public interface IState : IBaseState
    {
        public void Enter();
    }
    public interface IBaseState
    {
        public void Exit();
    }
    public interface IStateWithPayload<TPayload> : IBaseState
    {
        public void Enter(TPayload payload);
    }
}