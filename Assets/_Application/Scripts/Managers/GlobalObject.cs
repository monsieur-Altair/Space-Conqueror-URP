using System;
using System.Collections;
using _Application.Scripts.Infrastructure;
using _Application.Scripts.Infrastructure.Services;
using _Application.Scripts.Infrastructure.Services.Progress;
using UnityEngine;

namespace _Application.Scripts.Managers
{
    public class GlobalObject : MonoBehaviour, ICoroutineRunner
    {
        public void Init() => 
            DontDestroyOnLoad(this);

        public void InvokeWithDelay(Action action, float delay) => 
            StartCoroutine(WaitAndDo(delay, action));

        public void CancelAllInvoked() =>
            StopAllCoroutines();    

        private void OnApplicationFocus(bool hasFocus) => 
            AllServices.Instance.GetSingle<IReadWriterService>().WriteProgress();
        
        private IEnumerator WaitAndDo(float delay, Action action)
        {
            yield return new WaitForSeconds(delay);
            action?.Invoke();
        }
    }
}