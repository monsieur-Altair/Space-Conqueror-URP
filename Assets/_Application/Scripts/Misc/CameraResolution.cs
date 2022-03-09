using UnityEngine;

namespace _Application.Scripts.Misc
{
    public class CameraResolution : MonoBehaviour
    {
        public Vector2 defaultResolution = new Vector2(720, 1280);
        [Range(0f, 1f)] public float widthOrHeight;
 
        private static Camera camera;
     
        private float _initialSize;
        private float _targetAspect;
 
        private float _initialFov;
        private float _horizontalFov = 120f;
 
        private void Start()
        {
            camera = GetComponent<Camera>();
            _initialSize = camera.orthographicSize;
 
            _targetAspect = defaultResolution.x / defaultResolution.y;
 
            _initialFov = camera.fieldOfView;
            _horizontalFov = CalcVerticalFov(_initialFov, 1 / _targetAspect);
        }
 
        private void Update()
        {
            if (camera.orthographic)
            {
                float constantWidthSize = _initialSize * (_targetAspect / camera.aspect);
                camera.orthographicSize = Mathf.Lerp(constantWidthSize, _initialSize, widthOrHeight);
            }
            else
            {
                float constantWidthFov = CalcVerticalFov(_horizontalFov, camera.aspect);
                camera.fieldOfView = Mathf.Lerp(constantWidthFov, _initialFov, widthOrHeight);
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
            
            if(camera==null)
                return;
            
            Plane plane = new Plane(Vector3.up, new Vector3(0, 0, 0));
            Ray ray = camera.ViewportPointToRay(new Vector3(0,0,0));
            if (plane.Raycast(ray, out float distance))
            {
                Vector3 botLeft = ray.GetPoint(distance);
                min = camera.WorldToViewportPoint(botLeft).z;
            }
            
            ray = camera.ViewportPointToRay(new Vector3(1,1,0));
            if (plane.Raycast(ray, out float distance1))
            {
                Vector3 topRight = ray.GetPoint(distance1);
                max = camera.WorldToViewportPoint(topRight).z;
            }
        }
        
        public static Vector3 FindOffset(Vector3 worldPos, float minDepth, float maxDepth)// go to camera resolution
        {
            int coefficient = camera.pixelHeight / camera.pixelWidth;
            Vector3 screenPos = camera.WorldToScreenPoint(worldPos);
            float depth = screenPos.z;
            float offsetY=(minDepth-depth)/ (maxDepth-minDepth)*80.0f;
            float offsetX=(minDepth-depth)/ (maxDepth-minDepth)*(80.0f/coefficient);
  
            Vector3 res = new Vector3(offsetX, offsetY, 0);
     
            return res;
        }

    }
}
