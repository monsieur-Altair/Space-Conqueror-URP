using _Application.Scripts.Infrastructure.Services.Factory;
using _Application.Scripts.Infrastructure.Services.Progress;
using _Application.Scripts.Managers;

namespace _Application.Scripts.Infrastructure.States
{
    public class LoadLevelState : IStateWithPayload<string>
    {
        private readonly StateMachine _stateMachine;
        private readonly SceneLoader _sceneLoader;
        private readonly IGameFactory _gameFactory;
        private readonly IProgressService _progressService;
        //private curtain prefab
        public LoadLevelState(StateMachine stateMachine, SceneLoader sceneLoader, 
            IGameFactory gameFactory, IProgressService progressService)
        {
            _stateMachine = stateMachine;
            _sceneLoader = sceneLoader;
            _gameFactory = gameFactory;
            _progressService = progressService;
        }
        
        public void Enter(string payload)
        {
            _gameFactory.CleanUp();
            _sceneLoader.Load(payload, onLoaded: OnLoaded);
            //show curtain
        }

        public void Exit()
        {
            //hide curtain
        }

        private void OnLoaded()
        {
            Main mainManager = _gameFactory.CreateWorld();

            foreach (IProgressReader progressReader in _gameFactory.ProgressReaders)
                progressReader.ReadProgress(_progressService.PlayerProgress);

            mainManager.StartGame();
            _stateMachine.Enter<GameLoopState>();
        }
    }
}