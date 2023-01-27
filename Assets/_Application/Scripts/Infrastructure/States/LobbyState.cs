using _Application.Scripts.Buildings;
using _Application.Scripts.Managers;
using _Application.Scripts.UI;
using _Application.Scripts.UI.Windows;
using UnityEngine;

namespace _Application.Scripts.Infrastructure.States
{
    public class LobbyState : IState
    {
        private readonly StateMachine _stateMachine;
        private readonly LobbyManager _lobbyManager;


        public LobbyState(StateMachine stateMachine, LobbyManager lobbyManager)
        {
            _lobbyManager = lobbyManager;
            _stateMachine = stateMachine;
        }

        public void Enter()
        {
            UISystem.ShowWindow<LobbyWindow>();
            
            _lobbyManager.OnEnter();
            _lobbyManager.PointClicked += ShowWindow;
        }

        public void Exit()
        {
            _lobbyManager.PointClicked -= ShowWindow;
            _lobbyManager.OnExit();
            
            UISystem.CloseWindow<LobbyWindow>();
        }

        private void ShowWindow(BuildingType boughtType, int index)
        {
            UISystem.CloseWindow<LobbyWindow>();
            UISystem.ShowPayloadedWindow<BuyBuildingWindow, BuyBuildingWindowPayload>(new BuyBuildingWindowPayload()
            {
                BuildingType = boughtType,
                Index =  index,
            });
        }
    }

    public class BuyBuildingWindowPayload
    {
        public BuildingType BuildingType;
        public int Index;
    }
}