using _Application.Scripts.Buildings;
using UnityEngine;

namespace _Application.Scripts.Misc
{
    public class CameraResolution : MonoBehaviour
    {
        public Vector2 defaultResolution = new Vector2(720, 1280);
        [Range(0f, 1f)] public float widthOrHeight;
        
        private float _initialSize;
        private float _targetAspect;
        private float _initialFov;
        private float _horizontalFov = 120f;
        
        private Camera _mainCamera;

        private void CalculateSize()
        {
            if (_mainCamera.orthographic)
            {
                float constantWidthSize = _initialSize * (_targetAspect / _mainCamera.aspect);
                _mainCamera.orthographicSize = Mathf.Lerp(constantWidthSize, _initialSize, widthOrHeight);
            }
            else
            {
                float constantWidthFov = CalcVerticalFov(_horizontalFov, _mainCamera.aspect);
                _mainCamera.fieldOfView = Mathf.Lerp(constantWidthFov, _initialFov, widthOrHeight);
            }
        }

        public void Init(Camera mainCamera)
        {
            _mainCamera = mainCamera;
            
            _initialSize = _mainCamera.orthographicSize;
 
            _targetAspect = defaultResolution.x / defaultResolution.y;
 
            _initialFov = _mainCamera.fieldOfView;
            _horizontalFov = CalcVerticalFov(_initialFov, 1 / _targetAspect);
            
            CalculateSize();
            
            DontDestroyOnLoad(this);
        }

        private static float CalcVerticalFov(float hFovInDeg, float aspectRatio)
        { 
            float hFovInRads = hFovInDeg * Mathf.Deg2Rad;
            float vFovInRads = 2 * Mathf.Atan(Mathf.Tan(hFovInRads / 2) / aspectRatio);
            return vFovInRads * Mathf.Rad2Deg;
        }
    }
}
