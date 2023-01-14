using _Application.Scripts.Infrastructure.Services.Factory;
using _Application.Scripts.Infrastructure.Services.Progress;

namespace _Application.Scripts.Infrastructure.States
{
    public class LoadLevelState : IStateWithPayload<string>
    {
        private readonly StateMachine _stateMachine;
        private readonly SceneLoader _sceneLoader;
        private readonly GameFactory _gameFactory;
        private readonly ProgressService _progressService;
        //private curtain prefab
        public LoadLevelState(StateMachine stateMachine, SceneLoader sceneLoader, 
            GameFactory gameFactory, ProgressService progressService)
        {
            _stateMachine = stateMachine;
            _sceneLoader = sceneLoader;
            _gameFactory = gameFactory;
            _progressService = progressService;
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
            foreach (IProgressReader progressReader in _gameFactory.ProgressReaders)
                progressReader.ReadProgress(_progressService.PlayerProgress);

            _stateMachine.Enter<GameLoopState>();
        }
    }
}