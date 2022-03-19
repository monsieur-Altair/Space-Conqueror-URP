using System;
using System.Collections;
using _Application.Scripts.Infrastructure;
using UnityEngine;

namespace _Application.Scripts.Managers
{
    public class GlobalObject : MonoBehaviour, ICoroutineRunner
    {
        public static GlobalObject Instance;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        public void InvokeWithDelay(Action action, float delay) => 
            StartCoroutine(WaitAndDo(delay, action));

        public void CancelAllInvoked() =>
            StopAllCoroutines();    

        private IEnumerator WaitAndDo(float delay, Action action)
        {
            yield return new WaitForSeconds(delay);
            action?.Invoke();
        }
    }
}