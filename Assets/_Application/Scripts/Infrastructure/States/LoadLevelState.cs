using _Application.Scripts.Infrastructure.Factory;

namespace _Application.Scripts.Infrastructure.States
{
    public class LoadLevelState : IStateWithPayload<string>
    {
        private readonly StateMachine _stateMachine;
        private readonly SceneLoader _sceneLoader;
        private readonly IGameFactory _gameFactory;
        //private curtain prefab
        public LoadLevelState(StateMachine stateMachine, SceneLoader sceneLoader, IGameFactory gameFactory)
        {
            _stateMachine = stateMachine;
            _sceneLoader = sceneLoader;
            _gameFactory = gameFactory;
        }
        
        public void Enter(string payload)
        {
            _sceneLoader.Load(payload, onLoaded: OnLoaded);
            //show curtain
        }

        public void Exit()
        {
            //hide curtain
        }

        private void OnLoaded()
        {
            _gameFactory.CreateWorld();

            //inst planets and ui   
            //inject all dependencies in the scene
            _stateMachine.Enter<GameLoopState>();
        }
    }
}