using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Control
{
    public class PlanetController : MonoBehaviour
    {
        private List<Planets.Base> _selectablePlanets;
        private Planets.Base _destination;
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

        public void DeleteFromList(Planets.Base planet)
        {
            _selectablePlanets.Remove(planet);
            OnCancelingSelection(planet);
        }
        
        public void HandleTouch(Touch touch)
        {
            _touch = touch;
        
            Vector2 pos = _touch.position;
            switch (_touch.phase)
            {
                case TouchPhase.Began:
                {
                    HandleClick(pos);
                    break;
                }
                case TouchPhase.Ended:
                {
                    HandleRelease(pos);
                    break;
                }
                case TouchPhase.Moved:
                {
                    HandleMultipleSelection(pos);
                    break;
                }
            }
        }

        public void HandleMouseClick()
        {
            var pos = Input.mousePosition;
            if (Input.GetMouseButtonUp(0))
            {
                //Debug.Log("release");
                HandleRelease(pos);
            }
            else if (Input.GetMouseButtonDown(0))
            {
                HandleClick(pos);
            }
            else if(Input.GetMouseButton(0))
            {
                HandleMultipleSelection(pos);
            }
        }
        
        
    
        private void HandleClick(Vector3 pos)
        {
            Planets.Base planet = RaycastForPlanet(pos);
            if (planet != null)
            {
                if (_selectablePlanets.Contains(planet) == false && planet.Team == Planets.Team.Blue)
                {
                    OnSelecting(planet);
                }
            }
        }

        private void HandleRelease(Vector3 pos)
        {
            if (RaycastForPlanet(pos) == null)
            {
                foreach (Planets.Base planet in _selectablePlanets)
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
                _selectablePlanets.Clear();
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

        private Planets.Base RaycastForPlanet(Vector3 pos)
        {
            int layerMask = 1 << 0;
            layerMask = ~layerMask;
            Ray ray = _mainCamera.ScreenPointToRay(pos);
            return Physics.Raycast(ray, out var hit,Mathf.Infinity, layerMask) 
                ? hit.collider.GetComponent<Planets.Base>() : null;
        }

        private void HandleMultipleSelection(Vector3 pos)
        {
            Planets.Base planet = RaycastForPlanet(pos);
            if (planet == null) 
                return;
            //1 - empty: only blue,
            //2 - not empty: if user has selected blue, check container + if the last isn't blue, delete them 

            Planets.Team team = planet.Team;
            int count = _selectablePlanets.Count;
            if (team == Planets.Team.Blue)
            {
                    
                if(count==0)
                    OnSelecting(planet);
                else
                {
                    Planets.Base lastPlanet = _selectablePlanets.Last();
                    if (planet == lastPlanet)
                        return;
                    
                    if (lastPlanet.Team != Planets.Team.Blue)
                    {
                        OnCancelingSelection(lastPlanet);
                        _selectablePlanets.RemoveAt(count-1);
                    }
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
            else
            {
                if (count > 0)
                {
                    var lastPlanet = _selectablePlanets.Last();
                    if (planet == lastPlanet)
                        return;
                        
                    if (lastPlanet.Team != Planets.Team.Blue)
                    {
                        OnCancelingSelection(lastPlanet);
                        _selectablePlanets.RemoveAt(count-1);
                    }
                    OnSelecting(planet);
                }
            }
        }

        private void LaunchToDestination(Planets.Base destination)
        {
            foreach (Planets.Base planet in _selectablePlanets)
            {
                OnCancelingSelection(planet);
                if(planet.Team==Planets.Team.Blue)
                    planet.LaunchUnit(destination);
            }
            
        }

        private static void DecreaseScale(Object sender, Planets.Base planet)
        {
            if(planet!=null)
                planet.transform.localScale /= 1.5f;
        }
    
        private static void IncreaseScale(Object sender, Planets.Base planet)
        {
            if(planet!=null) 
                planet.transform.localScale *= 1.5f;
        }
    
        private void AddToList(Object sender, Planets.Base planet)
        {
            _selectablePlanets.Add(planet);
        }

 
    }
}