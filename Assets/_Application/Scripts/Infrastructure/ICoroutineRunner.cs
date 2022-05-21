using System;
using System.Collections;
using _Application.Scripts.Infrastructure.Services;
using _Application.Scripts.Infrastructure.Services.Progress;
using UnityEngine;

namespace _Application.Scripts.Infrastructure
{
    public interface ICoroutineRunner : IService
    {
        Coroutine StartCoroutine(IEnumerator coroutine);
        void StopCoroutine(Coroutine coroutine);
        void StopCoroutine(IEnumerator coroutine);
        void InvokeWithDelay(Action action, float delay);
        void CancelAllInvoked();
        void StopAllCoroutines();
        void Init(IReadWriterService readWriterService);
    }
}