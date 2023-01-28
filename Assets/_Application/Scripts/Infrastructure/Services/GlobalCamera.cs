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

        private float _minDepth;
        private float _maxDepth;
        
        public Camera MainCamera => _camera;

        public override void Init()
        {
            base.Init();

            _cameraResolution.Init(_camera);
            
            GetCameraDepths(out _minDepth, out _maxDepth);
        }

        public Vector3 FindSpawnPoint(BaseBuilding destination)
        {
            Vector3 destPosWorld = destination.transform.position;
            Vector3 destPosScreen = MainCamera.WorldToScreenPoint(destPosWorld);
            float destX = destPosScreen.x;
            float destY = destPosScreen.y;
            float destZ = destPosScreen.z;

            //possible points for spawn on screen coordinates
            Vector3[] possiblePoints =
            {
                new Vector3(0, destY, destZ), //from left side
                new Vector3(destX, Screen.height, _maxDepth), //from top side
                new Vector3(Screen.width, destY, destZ), //from right side
                new Vector3(destX, 0, _minDepth) //from bottom side
            };

            int minWayIndex = FindMinWay(in possiblePoints, in destPosScreen);

            Vector3 result = MainCamera.ScreenToWorldPoint(possiblePoints[minWayIndex]);
            result.y = destPosWorld.y;
            return result;   
        }
        
        private void GetCameraDepths(out float min, out float max)
        {
            min = max = 0.0f;
            
            if(MainCamera==null)
                return;
            
            Plane plane = new Plane(Vector3.up, new Vector3(0, 0, 0));
            Ray ray = MainCamera.ViewportPointToRay(new Vector3(0,0,0));
            if (plane.Raycast(ray, out float distance))
            {
                Vector3 botLeft = ray.GetPoint(distance);
                min = MainCamera.WorldToViewportPoint(botLeft).z;
            }
            
            ray = MainCamera.ViewportPointToRay(new Vector3(1,1,0));
            if (plane.Raycast(ray, out float distance1))
            {
                Vector3 topRight = ray.GetPoint(distance1);
                max = MainCamera.WorldToViewportPoint(topRight).z;
            }
        }
        
        private static int FindMinWay(in Vector3[] possiblePoints, in Vector3 destinationPos)
        {            
            int minIndex = 0;
            float min = float.MaxValue;
            int index = 0;
            foreach (Vector3 point in possiblePoints)
            {
                float distance = Vector2.Distance(point, destinationPos);
                
                if (distance < min)
                {
                    min = distance;
                    minIndex = index;
                }
                index++;
            }

            return minIndex;
        }
    }
}