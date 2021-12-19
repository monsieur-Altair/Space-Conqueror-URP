using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Control
{
    public class PlanetController : MonoBehaviour
    {
        private List<Planets.Base> _selectablePlanets;
        private Planets.Base _destination = null;
        //public UnityEvent onSelectPlanet;
        private event Action<Object, Planets.Base> CancelingSelection;
        private event Action<Object, Planets.Base> Selecting;
        private Camera _mainCamera;
        private Touch _touch;
    
        public void Start()
        {
            _mainCamera = Camera.main;
            if (_mainCamera == null)
                throw new MyException("can't get camera: " + name);
            _selectablePlanets = new List<Planets.Base>();
            CancelingSelection += DecreaseScale;
            Selecting += IncreaseScale;
            Selecting += AddToList;
        }
    
        public void HandleTouch(Touch touch)
        {
            _touch = touch;
        
            //Debug.Log("handle planet touch\n");

            switch (_touch.phase)
            {
                case TouchPhase.Began:
                {
                    HandleClick();
                    break;
                }
                case TouchPhase.Ended:
                {
                    HandleRelease();
                    break;
                }
                case TouchPhase.Moved:
                {
                    HandleMultipleSelection();
                    break;
                }
            }
        }
    
        private void HandleClick()
        {
            var planet = RaycastForPlanet();
            if (planet != null)
            {
                if (_selectablePlanets.Contains(planet) == false)
                {
                    OnSelecting(planet);
                }
            }
        }

        private void HandleRelease()
        {
            if (RaycastForPlanet() == null)
            {
                foreach (var planet in _selectablePlanets)
                {
                    OnCancelingSelection(planet);
                }
                _selectablePlanets.Clear();
                return;
            }
        
            int count = _selectablePlanets.Count;
            if (count > 1)
            {
                _destination = _selectablePlanets[count - 1];
                OnCancelingSelection(_destination);
                _selectablePlanets.Remove(_destination);
                LaunchToDestination(_destination);
            }
        }

        private void OnSelecting(Planets.Base planet)
        {
            Selecting?.Invoke(this, planet);
        }
    
        private void OnCancelingSelection(Planets.Base planet)
        {
            CancelingSelection?.Invoke(this, planet);
        }

        private Planets.Base RaycastForPlanet()
        {
            var ray = _mainCamera.ScreenPointToRay(_touch.position);
            return Physics.Raycast(ray, out var hit) ? hit.collider.GetComponent<Planets.Base>() : null;
        }

        private void HandleMultipleSelection()
        {
            var planet = RaycastForPlanet();
            if (planet != null)
            {
                if (_selectablePlanets.Contains(planet))
                {
                    _selectablePlanets.Remove(planet);
                    _selectablePlanets.Add(planet);
                }
                else
                {
                    OnSelecting(planet);
                }

            }
        }

        private void LaunchToDestination(Planets.Base destination)
        {
            foreach (var planet in _selectablePlanets)
            {
                OnCancelingSelection(planet);
                planet.LaunchUnit(destination);
            }
            _selectablePlanets.Clear();
        }

        private void DecreaseScale(Object sender, Planets.Base planet)
        {
            planet.transform.localScale /= 1.5f;
        }
    
        private void IncreaseScale(Object sender, Planets.Base planet)
        {
            planet.transform.localScale *= 1.5f;
        }
    
        private void AddToList(Object sender, Planets.Base planet)
        {
            _selectablePlanets.Add(planet);
        }

 
    }
}