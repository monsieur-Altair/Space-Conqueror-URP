using _Application.Scripts.Infrastructure.Services.Progress;
using _Application.Scripts.SavedData;
using TMPro;

namespace _Application.Scripts.Infrastructure.States
{
    public class ReadProgressState : IState
    {
        private readonly StateMachine _stateMachine;
        private readonly IProgressService _progressService;
        private readonly IReadWriterService _readWriterService;

        public ReadProgressState(StateMachine stateMachine, 
            IProgressService progressService, IReadWriterService readWriterService)
        {
            _stateMachine = stateMachine;
            _progressService = progressService;
            _readWriterService = readWriterService;
        }

        public void Exit()
        {
            
        }

        public void Enter()
        {
            _progressService.PlayerProgress = _readWriterService.ReadProgress() ?? new PlayerProgress(2);
            //_stateMachine.Enter<LoadLevelState,string>("Main");
        }
    }
}