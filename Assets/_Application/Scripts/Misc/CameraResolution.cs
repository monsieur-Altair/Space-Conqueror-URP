using System;
using UnityEngine;

namespace _Application.Scripts.Misc
{
    /// <summary>
    /// Keeps constant camera width instead of height, works for both Orthographic & Perspective cameras
    /// Made for tutorial https://youtu.be/0cmxFjP375Y
    /// </summary>

    public class CameraResolution : MonoBehaviour
    {
        public Vector2 defaultResolution = new Vector2(720, 1280);
        [Range(0f, 1f)] public float widthOrHeight = 0;
 
        private Camera _componentCamera;
     
        private float _initialSize;
        private float _targetAspect;
 
        private float _initialFov;
        private float _horizontalFov = 120f;
 
        private void Start()
        {
            _componentCamera = GetComponent<Camera>();
            _initialSize = _componentCamera.orthographicSize;
 
            _targetAspect = defaultResolution.x / defaultResolution.y;
 
            _initialFov = _componentCamera.fieldOfView;
            _horizontalFov = CalcVerticalFov(_initialFov, 1 / _targetAspect);
        }
 
        private void Update()
        {
            if (_componentCamera.orthographic)
            {
                var constantWidthSize = _initialSize * (_targetAspect / _componentCamera.aspect);
                _componentCamera.orthographicSize = Mathf.Lerp(constantWidthSize, _initialSize, widthOrHeight);
            }
            else
            {
                var constantWidthFov = CalcVerticalFov(_horizontalFov, _componentCamera.aspect);
                _componentCamera.fieldOfView = Mathf.Lerp(constantWidthFov, _initialFov, widthOrHeight);
            }
        }
 
        private static float CalcVerticalFov(float hFovInDeg, float aspectRatio)
        { 
            var hFovInRads = hFovInDeg * Mathf.Deg2Rad;
            var vFovInRads = 2 * Mathf.Atan(Mathf.Tan(hFovInRads / 2) / aspectRatio);
            return vFovInRads * Mathf.Rad2Deg;
        }
        
        public static void GetCameraDepths(out float min, out float max)
        {
            min = max = 0.0f;
            Camera camera = Camera.main;
            if (camera == null)
                throw new Exception("CANNOT GET CAMERA COMPONENT");
            
            Plane plane = new Plane(Vector3.up, new Vector3(0, 0, 0));
            Ray ray = camera.ViewportPointToRay(new Vector3(0,0,0));
            if (plane.Raycast(ray, out var distance))
            {
                Vector3 botLeft = ray.GetPoint(distance);
                min = camera.WorldToViewportPoint(botLeft).z;
            }
            
            ray = camera.ViewportPointToRay(new Vector3(1,1,0));
            if (plane.Raycast(ray, out var distance1))
            {
                Vector3 topRight = ray.GetPoint(distance1);
                max = camera.WorldToViewportPoint(topRight).z;
            }
        }

    }
}
