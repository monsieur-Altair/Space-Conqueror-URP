using System;
using System.Collections;
using UnityEngine;

namespace _Application.Scripts.Infrastructure
{
    public interface ICoroutineRunner
    {
        Coroutine StartCoroutine(IEnumerator coroutine);
        void StopCoroutine(Coroutine coroutine);
        void StopCoroutine(IEnumerator coroutine);
        public void InvokeWithDelay(Action action, float delay);
        public void CancelAllInvoked();
    }
}