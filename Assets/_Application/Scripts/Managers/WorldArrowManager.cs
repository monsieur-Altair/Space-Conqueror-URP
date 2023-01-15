using System.Collections.Generic;
using _Application.Scripts.Buildings;
using _Application.Scripts.UI;
using Pool_And_Particles;
using UnityEngine;

namespace _Application.Scripts.Managers
{
    public class WorldArrowManager
    {
        private readonly Dictionary<Transform, WorldArrow> _arrows;
        private readonly GlobalPool _globalPool;
        private readonly WorldArrow _worldArrowPrefab;
        private Plane _plane;
        private Camera _mainCamera;

        public WorldArrowManager(GlobalPool globalPool, WorldArrow worldArrowPrefab, Camera mainCamera)
        {
            _mainCamera = mainCamera;
            _worldArrowPrefab = worldArrowPrefab;
            _globalPool = globalPool;
            _arrows = new Dictionary<Transform, WorldArrow>();
            _plane = new Plane(Vector3.up, Vector3.zero);
        }

        public void AddArrow(Transform startPoint)
        {
            WorldArrow newArrow = _globalPool.Get(_worldArrowPrefab);
            newArrow.Enable(startPoint);
            _arrows.Add(startPoint, newArrow);
        }

        public void UpdateAll(Vector3 screenPoint)
        {
            Ray ray = _mainCamera.ScreenPointToRay(screenPoint);
            if (!_plane.Raycast(ray, out float distance))
                return;

            Vector3 endPos = ray.GetPoint(distance);
            
            foreach (WorldArrow arrow in _arrows.Values) 
                arrow.Align(endPos);
        }

        public void DisableArrow(Transform startPoint)
        {
            WorldArrow arrow = _arrows[startPoint];
            arrow.Disable();
            _globalPool.Free(arrow);
        }

        public void DisableAll()
        {
            foreach (WorldArrow arrow in _arrows.Values)
            {
                arrow.Disable();
                _globalPool.Free(arrow);
            }
            
            _arrows.Clear();
        }
    }
}