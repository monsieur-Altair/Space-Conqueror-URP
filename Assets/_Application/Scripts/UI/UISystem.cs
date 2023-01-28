using System;
using System.Collections.Generic;
using _Application.Scripts.Managers;
using _Application.Scripts.UI.Windows;
using _Application.Scripts.UI.Windows.Tutorial;
using UnityEngine;
using UnityEngine.UI;

namespace _Application.Scripts.UI
{
    public class UISystem : MonoBehaviourService
    {
        [SerializeField] 
        private Transform _windowsContainer;

        [SerializeField] 
        private Canvas _canvas;

        [SerializeField] 
        private CanvasScaler _canvasScaler;

        private readonly Dictionary<Type, Window> _windows = new Dictionary<Type, Window>();

        private static UISystem _instance;
        private RectTransform _canvasRect;
        private Vector2 _refRes;

        public override void Init()
        {
            base.Init();
            
            _instance = this;

            SetupCanvas();
            
            GetAllWindows();
            SubscribeEvents();
            ShowWindow<EmptyWindow>();
        }

        private void SetupCanvas()
        {
            _canvasRect = _canvas.transform as RectTransform;

            Rect pixelRect = _canvas.pixelRect;
            float aspect = pixelRect.width / pixelRect.height;
            _canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            const float minAspect = 0.45f;
            const float maxAspect = 0.8f;
            _canvasScaler.matchWidthOrHeight = Mathf.Clamp01(Mathf.InverseLerp(minAspect, maxAspect, aspect));
            _refRes = new Vector2(1242, 2208);
            _refRes.y = _refRes.y / aspect * 9f / 16f;
        }

        private void OnDestroy() => 
            UnsubscribeEvents();

        public static Vector2 GetUIPosition(Camera cam, Vector3 world)
        {
            Vector3 viewport = cam.WorldToViewportPoint(world);
            Vector2 screen = new Vector2(viewport.x * _instance._refRes.x, viewport.y * _instance._refRes.y);
            return screen;
        }
        
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