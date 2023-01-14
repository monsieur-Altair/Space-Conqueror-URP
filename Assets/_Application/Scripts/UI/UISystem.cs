using System;
using System.Collections.Generic;
using _Application.Scripts.Managers;
using _Application.Scripts.UI.Windows;
using _Application.Scripts.UI.Windows.Tutorial;
using UnityEngine;

namespace _Application.Scripts.UI
{
    public class UISystem : MonoBehaviourService
    {
        [SerializeField] 
        private Transform _windowsContainer;

        [SerializeField] 
        private Canvas _canvas;

        private readonly Dictionary<Type, Window> _windows = new Dictionary<Type, Window>();

        private static UISystem _instance;
        public Canvas GameCanvas => _canvas;

        public override void Init()
        {
            base.Init();
            
            _instance = this;
            
            GetAllWindows();
            SubscribeEvents();
            ShowWindow<EmptyWindow>();
        }

        private void OnDestroy() => 
            UnsubscribeEvents();

        public static void ShowWindow<TWindow>() where TWindow : Window
        {
            Window window = _instance._windows[typeof(TWindow)];
            if(window.IsOpened==false)
                window.Open();
        }
        
        public static void ShowPayloadedWindow<TWindow, TPayload>(TPayload payload) 
            where TWindow : PayloadedWindow<TPayload>
        {
            PayloadedWindow<TPayload> window = _instance._windows[typeof(TWindow)] as PayloadedWindow<TPayload>;
            if(window.IsOpened==false)
                window.Open(payload);
        }

        public static TWindow GetWindow<TWindow>() where TWindow : Window => 
            _instance._windows[typeof(TWindow)] as TWindow;
        
        public static void CloseWindow<TWindow>() where TWindow : Window
        {
            Window window = _instance._windows[typeof(TWindow)];
            if(window.IsOpened)
                window.Close();
        }

        private void SubscribeEvents()
        {
            Window.Opened += Window_Opened;
            Window.Closed += Window_Closed;
        }

        private void UnsubscribeEvents()
        {
            Window.Opened -= Window_Opened;
            Window.Closed -= Window_Closed;
        }

        private void GetAllWindows()
        {
            Window[] windowsInContainer = _windowsContainer.GetComponentsInChildren<Window>(true);
            foreach (Window window in windowsInContainer)
            {
                _windows.Add(window.GetType(), window);
                window.GetDependencies();
                window.gameObject.SetActive(false);
            }
        }

        private void Window_Opened(Window window)
        {
            window.transform.SetAsLastSibling();
        }

        private void Window_Closed(Window closedWindow)
        {
            closedWindow.transform.SetAsFirstSibling();
        }

        private static Window GetTutorialWindow(int levelsNumber)
        {
            return levelsNumber switch
            {
                0 => GetWindow<Tutorial0Window>(),
                1 => GetWindow<Tutorial1Window>(),
                2 => GetWindow<Tutorial2Window>(),
                3 => GetWindow<Tutorial3Window>(),
                4 => GetWindow<Tutorial4Window>(),
                5 => GetWindow<Tutorial5Window>(),
                _ => throw new ArgumentOutOfRangeException(nameof(levelsNumber), levelsNumber, null)
            };
        }
        
        public static void ShowTutorialWindow(int levelsNumber)
        {
            GetTutorialWindow(levelsNumber).Open();
        }

        public static void CloseTutorialWindow(int currentLevelNumber)
        {
            GetTutorialWindow(currentLevelNumber).Close();
        }
    }
}