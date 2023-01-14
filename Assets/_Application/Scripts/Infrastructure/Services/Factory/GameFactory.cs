﻿using System.Collections.Generic;
using _Application.Scripts.AI;
using _Application.Scripts.Control;
using _Application.Scripts.Infrastructure.Services.Progress;
using _Application.Scripts.Infrastructure.Services.Scriptables;
using _Application.Scripts.Managers;
using _Application.Scripts.UI;
using _Application.Scripts.UI.Windows;
using UnityEngine;

namespace _Application.Scripts.Infrastructure.Services.Factory
{
    public class GameFactory : IService
    {
        public List<IProgressReader> ProgressReaders { get; } 
        public List<IProgressWriter> ProgressWriters { get; }

        private readonly ScriptableService _scriptableService;
        private readonly CoreConfig _coreConfig;

        public GameFactory(ScriptableService scriptableService, CoreConfig coreConfig)
        {
            ProgressReaders = new List<IProgressReader>();
            ProgressWriters = new List<IProgressWriter>();

            _scriptableService = scriptableService;
            _coreConfig = coreConfig;
        }

        public GameLoopManager CreateWorld()
        {
            ObjectPool objectPool = AllServices.Get<ObjectPool>();
            CoroutineRunner coroutineRunner = AllServices.Get<CoroutineRunner>();
            
            Warehouse warehouse = _coreConfig.Warehouse;

            GlobalCamera globalCamera = AllServices.Get<GlobalCamera>();
            AISkillController aiAISkillController = 
                new AISkillController(objectPool,_scriptableService , objectPool, globalCamera); 
            
            Core aiManager = new Core(coroutineRunner, aiAISkillController);
            
            Outlook outlookManager = new Outlook(warehouse);
            UserControl userControl = AllServices.Get<UserControl>();
            CounterSpawner.Init(warehouse, objectPool, UISystem.GetWindow<GameplayWindow>().CounterContainer, globalCamera);
            
            
            GameLoopManager gameLoopManager = new GameLoopManager(AllServices.Get<LevelManager>(), 
                new TeamManager(AllServices.Get<ProgressService>()), 
                coroutineRunner, aiManager, objectPool, outlookManager, userControl,
                AllServices.Get<ScriptableService>(),
                AllServices.Get<ProgressService>(),
                AllServices.Get<AudioManager>());
            
            ProgressReaders.Add(gameLoopManager);
            ProgressWriters.Add(gameLoopManager);

            return gameLoopManager;
        }

        public void CreateAndRegisterMonoBeh<T>(T prefab) where T : MonoBehaviourService
        {
            T service = Object.Instantiate(prefab);
            service.Init();
            AllServices.Register(service);
        }
    }
}