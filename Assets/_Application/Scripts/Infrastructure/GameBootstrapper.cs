﻿using System.Security.Cryptography;
using _Application.Scripts.Infrastructure.States;
using _Application.Scripts.UI;
using _Application.Scripts.UI.Windows;
using UnityEngine;
using UnityEngine.UI;

namespace _Application.Scripts.Infrastructure
{
    public class GameBootstrapper : MonoBehaviour
    {
        private Game _game;
        
        private void Awake()
        {
            _game = new Game(this);
            _game.StateMachine.Enter<BootstrapState>();
            UISystem.ShowWindow<StartUpWindow>();
            UISystem.GetWindow<StartUpWindow>().PlayButton.onClick.AddListener(EnterLoadLevel);
            UISystem.GetWindow<StartUpWindow>().ToStatsButton.onClick.AddListener(OnStatsClicked);
            UISystem.GetWindow<StatisticWindow>().BackToGameButton.onClick.AddListener(BackToStartUp);
            DontDestroyOnLoad(this);
        }

        private static void BackToStartUp()
        {
            UISystem.ReturnToPreviousWindow();
            UISystem.ShowWindow<StartUpWindow>();
        }

        private void EnterLoadLevel() => 
            _game.StateMachine.Enter<LoadLevelState, string>("Main");

        private static void OnStatsClicked()
        {
            UISystem.ReturnToPreviousWindow();
            UISystem.ShowWindow<StatisticWindow>();
        }
    }
}