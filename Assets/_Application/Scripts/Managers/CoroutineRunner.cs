using System;
using System.Collections;
using _Application.Scripts.Infrastructure.Services;
using _Application.Scripts.Infrastructure.Services.Progress;
using Pool_And_Particles;
using UnityEngine;

namespace _Application.Scripts.Managers
{
    public class CoroutineRunner : MonoBehaviourService
    {
        private ReadWriterService _readWriterService;
        
        public override void Init()
        {
            base.Init();
            
            _readWriterService = AllServices.Get<ReadWriterService>();
        }

        public void InvokeWithDelay(Action action, float delay) => 
            StartCoroutine(WaitAndDo(delay, action));

        public void CancelAllInvoked() =>
            StopAllCoroutines();    

        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus == false)
            {
                _readWriterService.WriteProgress();
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                _readWriterService.WriteProgress();
            }
        }

        private static IEnumerator WaitAndDo(float delay, Action action)
        {
            yield return new WaitForSeconds(delay);
            action?.Invoke();
        }
    }
}