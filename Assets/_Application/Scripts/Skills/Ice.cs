﻿using System;
using _Application.Scripts.Control;
using UnityEngine;

namespace Skills
{
    public class Ice : Base
    {
        [SerializeField] private GameObject icePrefab;
        
        private const float PlanetLayHeight = 0.66f;
        public float Duration { get; private set; }

        private GameObject _freezingObject;
        private Zone _freezingZone;
        private Plane _plane;
        
        public static event Action DeletingFreezingZone;
        protected override void LoadResources()
        {
            base.LoadResources();
            var res = resource as Scriptables.Ice;
            if (res != null)
            {
                Duration = res.duration;
            }

            _freezingObject = Instantiate(icePrefab);
            
            _freezingZone = _freezingObject.GetComponent<Zone>(); 
            _freezingZone.SetTriggerFunction(FreezingEnteredObjects);
            
            _freezingObject.SetActive(false);
            _plane = new Plane(Vector3.up, new Vector3(0, PlanetLayHeight, 0));
        }

        private void SpawnFreezingZone(Vector3 pos)
        {
            _freezingObject.SetActive(true);
            var ray = MainCamera.ScreenPointToRay(pos);
            if(_plane.Raycast(ray, out var distance))
            {
                _freezingObject.transform.position = ray.GetPoint(distance);   
            }
            else
            {
                throw new MyException("cannot calculate zone position");
            }
        }

        private static void FreezingEnteredObjects(Collider other)
        {
            var obj = other.gameObject.GetComponent<Skills.IFreezable>();
            if (obj != null)
            {
                obj.Freeze();
                DeletingFreezingZone += obj.Unfreeze;
            }
        }

        protected override void ApplySkill()
        {
            IsOnCooldown = true;
            SpawnFreezingZone(SelectedScreenPos);
            Planets.Scientific.DecreaseScientificCount(Cost);
            Invoke(nameof(CancelSkill), Cooldown);
        }

        protected override void CancelSkill()
        {
            DeletingFreezingZone?.Invoke();
            _freezingObject.SetActive(false);
            IsOnCooldown = false;
            UnblockButton();
        }
    }
}