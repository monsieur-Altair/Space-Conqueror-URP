using _Application.Scripts.Buildings;
using UnityEngine;

namespace _Application.Scripts.Misc
{
    public class CameraResolution : MonoBehaviour
    {
        public Vector2 defaultResolution = new Vector2(720, 1280);
        [Range(0f, 1f)] public float widthOrHeight;
 
        private static Camera _camera;
        private static float _minDepth;
        private static float _maxDepth;

        private float _initialSize;
        private float _targetAspect;

        private float _initialFov;
        private float _horizontalFov = 120f;


        private void Start()
        {
            _camera = GetComponent<Camera>();
            _initialSize = _camera.orthographicSize;
 
            _targetAspect = defaultResolution.x / defaultResolution.y;
 
            _initialFov = _camera.fieldOfView;
            _horizontalFov = CalcVerticalFov(_initialFov, 1 / _targetAspect);
            
            GetCameraDepths(out _minDepth, out _maxDepth);
        }
 
        private void Update()
        {
            if (_camera.orthographic)
            {
                float constantWidthSize = _initialSize * (_targetAspect / _camera.aspect);
                _camera.orthographicSize = Mathf.Lerp(constantWidthSize, _initialSize, widthOrHeight);
            }
            else
            {
                float constantWidthFov = CalcVerticalFov(_horizontalFov, _camera.aspect);
                _camera.fieldOfView = Mathf.Lerp(constantWidthFov, _initialFov, widthOrHeight);
            }
        }
 
        private static float CalcVerticalFov(float hFovInDeg, float aspectRatio)
        { 
            float hFovInRads = hFovInDeg * Mathf.Deg2Rad;
            float vFovInRads = 2 * Mathf.Atan(Mathf.Tan(hFovInRads / 2) / aspectRatio);
            return vFovInRads * Mathf.Rad2Deg;
        }
        
        public static void GetCameraDepths(out float min, out float max)
        {
            min = max = 0.0f;
            
            if(_camera==null)
                return;
            
            Plane plane = new Plane(Vector3.up, new Vector3(0, 0, 0));
            Ray ray = _camera.ViewportPointToRay(new Vector3(0,0,0));
            if (plane.Raycast(ray, out float distance))
            {
                Vector3 botLeft = ray.GetPoint(distance);
                min = _camera.WorldToViewportPoint(botLeft).z;
            }
            
            ray = _camera.ViewportPointToRay(new Vector3(1,1,0));
            if (plane.Raycast(ray, out float distance1))
            {
                Vector3 topRight = ray.GetPoint(distance1);
                max = _camera.WorldToViewportPoint(topRight).z;
            }
        }
        
        public static Vector3 FindOffset(Vector3 worldPos)// go to camera resolution
        {
            int coefficient = _camera.pixelHeight / _camera.pixelWidth;
            Vector3 screenPos = _camera.WorldToScreenPoint(worldPos);
            float depth = screenPos.z;
            float offsetY=(_minDepth-depth)/ (_maxDepth-_minDepth)*80.0f;
            float offsetX=(_minDepth-depth)/ (_maxDepth-_minDepth)*(80.0f/coefficient);
  
            Vector3 res = new Vector3(offsetX, offsetY, 0);
     
            return res;
        }

        //calculate a min way on SCREEN (NOT WORLD) coordinates for supply
        public static Vector3 FindSpawnPoint(Base destination)
        {
            Vector3 destPosWorld = destination.transform.position;
            Vector3 destPosScreen = _camera.WorldToScreenPoint(destPosWorld);
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

            Vector3 result = _camera.ScreenToWorldPoint(possiblePoints[minWayIndex]);
            result.y = destPosWorld.y;
            return result;
        }

        //find launch point by calculating distance between possible points and destination point
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

        public static Vector3 GetScreenPos(Vector3 pos) => 
            _camera.WorldToScreenPoint(pos);
    }
}
