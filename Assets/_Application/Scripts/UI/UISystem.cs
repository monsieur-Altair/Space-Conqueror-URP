using System;
using System.Collections.Generic;
using _Application.Scripts.UI.Windows;
using _Application.Scripts.UI.Windows.Tutorial;
using UnityEngine;

namespace _Application.Scripts.UI
{
    public class UISystem : MonoBehaviour
    {
        [SerializeField] 
        private Transform _windowsContainer;

        [SerializeField] 
        private Canvas _canvas;

        private readonly Stack<Window> _openedWindows = new Stack<Window>(new HashSet<Window>(new Stack<Window>()));
        private readonly Dictionary<Type, Window> _windows = new Dictionary<Type, Window>();

        public static UISystem Instance { get; private set; }
        public Window CurrentWindow { get; private set; }
        public Canvas GameCanvas => _canvas;
        public static bool IsTutorialDisplayed { get; private set; }


        public void Setup()
        {
            if (Instance == null)
            {
                Instance = this;

                GetAllWindows();
                
                SubscribeEvents();

                ShowWindow<EmptyWindow>();

                DontDestroyOnLoad(this);
            }
        }

        private void OnDestroy() => 
            UnsubscribeEvents();

        public static void ShowWindow<TWindow>() where TWindow : Window => 
            Instance._windows[typeof(TWindow)].Open();

        public static void ReturnToPreviousWindow() => 
            Instance._openedWindows.Peek().Close();

        public static TWindow GetWindow<TWindow>() where TWindow : Window => 
            Instance._windows[typeof(TWindow)] as TWindow;

        private void SubscribeEvents()
        {
            Window.Opened += Window_Opened;
            Window.Closed += Window_Closed;
            AnimatedWindow.FadeCompleted += AnimatedWindow_FadeCompleted;
        }

        private void UnsubscribeEvents()
        {
            Window.Opened -= Window_Opened;
            Window.Closed -= Window_Closed;
            AnimatedWindow.FadeCompleted -= AnimatedWindow_FadeCompleted;
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
            _openedWindows.Push(window);
            window.transform.SetAsLastSibling();
            CurrentWindow = window;
        }

        private void Window_Closed(Window closedWindow)
        {
            Window poppedWindow = _openedWindows.Pop();
            if (poppedWindow.GetType() != closedWindow.GetType())
                throw new InvalidOperationException("was popped wrong window");
            
            poppedWindow.transform.SetAsFirstSibling();
            CurrentWindow = _openedWindows.Peek();
        }

        private static void AnimatedWindow_FadeCompleted() => 
            IsTutorialDisplayed = false;

        public static void ShowTutorialWindow(int levelsNumber)
        {
            switch (levelsNumber)
            {
                case 0:
                    ShowWindow<Tutorial0Window>();
                    break;
                case 1:
                    ShowWindow<Tutorial1Window>();
                    break;
                case 2:
                    ShowWindow<Tutorial2Window>();
                    break;
                case 3:
                    ShowWindow<Tutorial3Window>();
                    break;
                case 4:
                    ShowWindow<Tutorial4Window>();
                    break;
                case 5:
                    ShowWindow<Tutorial5Window>();
                    break;
            }

            IsTutorialDisplayed = true;
        }
    }
}