using System;
using UnityEngine;

namespace _Application.Scripts.Managers
{
    [RequireComponent(typeof(Collider))]
    public class BuildingPoint : MonoBehaviour
    {
        [SerializeField] private GameObject _spriteGO;

        public event Action<BuildingPoint> Clicked = delegate { };

        public void SetOutlook(bool isActive)
        {
            _spriteGO.SetActive(isActive);            
        }
        
        private void OnMouseDown()
        {
            Clicked(this);
        }
    }
}