using UnityEngine;
using Object = UnityEngine.Object;

namespace _Application.Scripts.Infrastructure.States
{
    public class LoadLevelState : IStateWithPayload<string>
    {
        private readonly StateMachine _stateMachine;
        private readonly SceneLoader _sceneLoader;
        //private curtain prefab
        public LoadLevelState(StateMachine stateMachine, SceneLoader sceneLoader)
        {
            _stateMachine = stateMachine;
            _sceneLoader = sceneLoader;
        }
        
        public void Enter(string payload)
        {
            _sceneLoader.Load(payload, onLoaded: OnLoaded);
            //show curtain
        }

        public void Exit()
        {
            //hide curtain
        }

        private void OnLoaded()
        {
            //inst planets and ui   
        }

        private GameObject Inst(string path)
        {
            GameObject prefab = Resources.Load<GameObject>(path);
            return Object.Instantiate(prefab);
        }
    }
}