using System;
using _Application.Scripts.Misc;
using UnityEngine;

namespace _Application.Scripts.Test
{
    public class TestArrow : MonoBehaviour
    {
        [SerializeField] private Transform _point0;
        [SerializeField] private Transform _point1;

        [SerializeField] private Transform _arrow;
        [SerializeField] private float _angularSpeed = 5f;
        [SerializeField] private float _radius = 5f;
        private float _angle;

        private void Update()
        {
            AlignArrow(_point0, _point1);
            Rotate();
        }

        private void Rotate()
        {
            _angle += _angularSpeed * Time.deltaTime;
            _point1.position = new Vector3(Mathf.Cos(_angle),0,Mathf.Sin(_angle)) * _radius;
        }

        private void AlignArrow(Transform start, Transform end)
        {
            Vector2 startXZ = start.position.ToXZ();
            Vector2 endXZ = end.position.ToXZ();
            float distance = Vector2.Distance(startXZ, endXZ);
            _arrow.localScale = _arrow.localScale.With(x: distance);
            double angle = Math.Atan2(endXZ.y - startXZ.y, endXZ.x - startXZ.x);
            _arrow.localRotation = Quaternion.Euler(0,  (-1) * (float) (Mathf.Rad2Deg * angle) ,0);
        }
    }
}