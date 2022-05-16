using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Application.Scripts.UI.Windows
{
    [RequireComponent(typeof(GraphicRaycaster),typeof(Image))]
    public class ClickedWindow : Window, IPointerDownHandler, IPointerUpHandler
    {
        public event Action PointerDown = delegate { };
        public event Action PointerUp = delegate { };

        public void OnPointerDown(PointerEventData eventData) => 
            PointerDown();

        public void OnPointerUp(PointerEventData eventData) => 
            PointerUp();
    }
}