using _Application.Scripts.Buildings;
using _Application.Scripts.Managers;
using _Application.Scripts.Misc;
using UnityEngine;

namespace _Application.Scripts.Infrastructure.Services
{
    public class GlobalCamera : MonoBehaviourService
    {
        [SerializeField] private CameraResolution _cameraResolution;
        [SerializeField] private Camera _camera;

        public Camera MainCamera => _camera;

        public Vector3 FindSpawnPoint(Base destination) => 
            _cameraResolution.FindSpawnPoint(destination);

        public override void Init()
        {
            base.Init();

            _cameraResolution.Init(_camera);
        }

        public Vector3 GetScreenPos(Vector3 pos)
        {
            return _cameraResolution.GetScreenPos(pos);
        }
    }
}