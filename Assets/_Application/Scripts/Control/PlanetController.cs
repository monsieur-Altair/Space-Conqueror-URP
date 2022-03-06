using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Application.Scripts.Control
{
    public class PlanetController
    {
        private event Action<Planets.Base> CancelingSelection;
        private event Action<Planets.Base> PlanetSelected;
        
        private readonly List<Planets.Base> _selectablePlanets;
        private readonly Camera _mainCamera;

        public PlanetController(Camera camera)
        {
            _mainCamera = camera;
            _selectablePlanets = new List<Planets.Base>();
            CancelingSelection += DecreaseScale;
            PlanetSelected += IncreaseScale;
            PlanetSelected += AddToList;
        }

        public void HandleClick(Vector3 pos)
        {
            Planets.Base planet = RaycastForPlanet(pos);
            if (planet != null)
                if (_selectablePlanets.Contains(planet) == false && planet.Team == Planets.Team.Blue)
                    OnSelecting(planet);
        }

        public void HandleRelease(Vector3 pos)
        {
            if (RaycastForPlanet(pos) == null)
            {
                ClearAllSelection();
                return;
            }
        
            int count = _selectablePlanets.Count;
            if (count > 1) 
                StartLaunching(count);
        }

        public void HandleMultipleSelection(Vector3 pos)
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
                    MoveAllyToTheEnd(planet);
            }
            else
            {
                if (count > 0) 
                    MoveEnemyToTheEnd(planet, count);
            }
        }

        private void StartLaunching(int count)
        {
            Planets.Base destination = _selectablePlanets[count - 1];
            _selectablePlanets.Remove(destination);
            OnCancelingSelection(destination);
            LaunchToDestination(destination);
            _selectablePlanets.Clear();
        }

        private void ClearAllSelection()
        {
            foreach (Planets.Base planet in _selectablePlanets)
                OnCancelingSelection(planet);
            _selectablePlanets.Clear();
        }

        private void OnSelecting(Planets.Base planet) => 
            PlanetSelected?.Invoke(planet);

        private void OnCancelingSelection(Planets.Base planet) => 
            CancelingSelection?.Invoke(planet);

        private Planets.Base RaycastForPlanet(Vector3 pos)
        {
            int layerMask = 1 << 0;
            layerMask = ~layerMask;
            Ray ray = _mainCamera.ScreenPointToRay(pos);
            return Physics.Raycast(ray, out RaycastHit hit,Mathf.Infinity, layerMask) 
                ? hit.collider.GetComponent<Planets.Base>() : null;
        }

        private void MoveEnemyToTheEnd(Planets.Base planet, int count)
        {
            Planets.Base lastPlanet = _selectablePlanets.Last();
            if (planet == lastPlanet)
                return;

            if (lastPlanet.Team != Planets.Team.Blue)
            {
                OnCancelingSelection(lastPlanet);
                _selectablePlanets.RemoveAt(count - 1);
            }

            OnSelecting(planet);
        }

        private void MoveAllyToTheEnd(Planets.Base planet)
        {
            Planets.Base lastPlanet = _selectablePlanets.Last();
            if (planet == lastPlanet)
                return;

            if (lastPlanet.Team != Planets.Team.Blue)
                RemoveLastFromList(lastPlanet);

            if (_selectablePlanets.Contains(planet))
                PlaceInTheEnd(planet);
            else
                OnSelecting(planet);
        }

        private void PlaceInTheEnd(Planets.Base planet)
        {
            _selectablePlanets.Remove(planet);
            _selectablePlanets.Add(planet);
        }

        private void RemoveLastFromList(Planets.Base lastPlanet)
        {
            OnCancelingSelection(lastPlanet);
            _selectablePlanets.RemoveAt(_selectablePlanets.Count - 1);
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

        private static void DecreaseScale(Planets.Base planet)
        {
            if(planet!=null)
                planet.transform.localScale /= 1.5f;
        }
    
        private static void IncreaseScale(Planets.Base planet)
        {
            if(planet!=null) 
                planet.transform.localScale *= 1.5f;
        }
    
        private void AddToList(Planets.Base planet) => 
            _selectablePlanets.Add(planet);
    }
}