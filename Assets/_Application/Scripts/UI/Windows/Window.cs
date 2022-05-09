using System;
using UnityEngine;

namespace _Application.Scripts.UI.Windows
{
    [RequireComponent(typeof(Canvas))]
    public abstract class Window : MonoBehaviour
    {
        public static event Action<Window> Opened = delegate {  };
        public static event Action<Window> Closed = delegate {  };

        public void Open()
        {
            OnOpened();
            Opened(this);
            gameObject.SetActive(true); 
        }

        public void Close()
        {
            OnClosed();
            Closed(this);
            gameObject.SetActive(false);
        }

        public virtual void GetDependencies()
        {
            
        }

        protected virtual void OnOpened()
        {
        }

        protected virtual void OnClosed()
        {
        }

        protected virtual void OnEnable() => 
            SubscribeEvents();

        protected virtual void OnDisable() => 
            UnSubscribeEvents();

        protected virtual void UnSubscribeEvents()
        {
        }

        protected virtual void SubscribeEvents()
        {
        }
    }
}