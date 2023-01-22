using System.Collections;
using _Application.Scripts.Infrastructure.Services.Factory;
using _Application.Scripts.Misc;
using _Application.Scripts.SavedData;
using UnityEngine;

namespace _Application.Scripts.Infrastructure.Services.Progress
{
    public class ReadWriterService : IService
    {
        private readonly GameFactory _gameFactory;
        private readonly ProgressService _progressService;

        private const string PlayerProgressKey = "PlayerProgress";

        public ReadWriterService(ProgressService progressService, GameFactory gameFactory)
        {
            _progressService = progressService;
            _gameFactory = gameFactory;
        }

        public static PlayerProgress ReadProgress() => 
            PlayerPrefs.GetString(PlayerProgressKey)?.ConvertFromJson<PlayerProgress>();

        public void WriteProgress()
        {
            foreach (IProgressWriter progressWriter in _gameFactory.ProgressWriters)
                progressWriter.WriteProgress(_progressService.PlayerProgress);
            
            PlayerPrefs.SetString(PlayerProgressKey, _progressService.PlayerProgress.ConvertToJson());
            PlayerPrefs.Save();
        }
    }
}