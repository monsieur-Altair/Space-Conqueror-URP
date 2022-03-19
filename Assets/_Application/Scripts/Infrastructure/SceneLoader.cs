using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Application.Scripts.Infrastructure
{
    public class SceneLoader
    {
        private readonly MonoBehaviour _behaviour;
        
        public SceneLoader(MonoBehaviour behaviour) 
            => _behaviour = behaviour;
        
        public void Load(string name, Action onLoaded=null) => 
            _behaviour.StartCoroutine(LoadScene(name, onLoaded));

        private IEnumerator LoadScene(string name, Action onLoaded = null)
        {
            if (SceneManager.GetActiveScene().name == name)
            {
                onLoaded?.Invoke();
                yield break;
            }
            
            AsyncOperation waitLoading = SceneManager.LoadSceneAsync(name);

            while(!waitLoading.isDone)
            {
                yield return null;
            }
            
            onLoaded?.Invoke();
        }
    }
}