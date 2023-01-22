using System;
using UnityEngine;

namespace _Application.Scripts.Managers
{
    [RequireComponent(typeof(Collider))]
    public class BuildingPoint : MonoBehaviour
    {
        public event Action<BuildingPoint> Clicked = delegate { };
        
        private void OnMouseDown()
        {
            Clicked(this);
        }
    }
}