using System;
using _Application.Scripts.Misc;
using Pool_And_Particles;
using UnityEngine;

namespace _Application.Scripts.UI
{
    public class WorldArrow : PooledBehaviour
    {
        [SerializeField] private Transform _arrow;

        private Transform _startPoint;
        private Transform _endPoint;

        public void Enable(Transform startPoint)
        {
            _arrow.transform.position = startPoint.position;
            _startPoint = startPoint;
            _arrow.localScale = _arrow.localScale.With(x: 0.0f);
        }

        public void Disable()
        {
            _startPoint = null;
        }
        
        public void Align(Vector3 endPos)
        {
            Align(_startPoint.position.ToXZ(), endPos.ToXZ());
        }
        
        private void Align(Vector2 startXZ, Vector2 endXZ)
        {
            float distance = Vector2.Distance(startXZ, endXZ);
            _arrow.localScale = _arrow.localScale.With(x: distance);
            double angle = Math.Atan2(endXZ.y - startXZ.y, endXZ.x - startXZ.x);
            _arrow.localRotation = Quaternion.Euler(0,  (-1) * (float) (Mathf.Rad2Deg * angle) ,0);
        }
    }
}