using System;
using System.Collections;
using _Application.Scripts.Control;
using UnityEngine;

namespace _Application.Scripts.Skills
{
    public class Ice : Base
    {
        [SerializeField] private GameObject icePrefab;
        public static event Action DeletingFreezingZone;

        private const float PlanetLayHeight = 0.66f;
        private float _duration;

        private GameObject _freezingObject;
        private Zone _freezingZone;
        private Plane _plane;
        private Coroutine _spawnCoroutine;

        protected override void LoadResources()
        {
            base.LoadResources();
            Scriptables.Ice res = resource as Scriptables.Ice;
            if (res != null) 
                _duration = res.duration;

            _freezingObject = Instantiate(icePrefab);
            
            _freezingZone = _freezingObject.GetComponent<Zone>(); 
            _freezingZone.SetTriggerFunction(FreezingEnteredObjects);
            
            _freezingObject.SetActive(false);
            _plane = new Plane(Vector3.up, new Vector3(0, PlanetLayHeight, 0));
        }

        protected override void CancelSkill()
        {
            //wasn't tested well
            CancelInvoke(nameof(CancelSkill));
            StopCoroutine(_spawnCoroutine);
            DeletingFreezingZone?.Invoke();
            _freezingObject.SetActive(false);
            
            IsOnCooldown = false;
            OnCanceledSkill();
        }

        protected override void ApplySkill()
        {
            IsOnCooldown = true;
            _spawnCoroutine = StartCoroutine(SpawnFreezingZone(SelectedScreenPos));
            Planets.Scientific.DecreaseScientificCount(Cost);
            Invoke(nameof(CancelSkill), Cooldown);
        }

        private IEnumerator SpawnFreezingZone(Vector3 pos)
        {
            _freezingObject.SetActive(true);
            
            Ray ray = MainCamera.ScreenPointToRay(pos);
            if (_plane.Raycast(ray, out float distance))
                _freezingObject.transform.position = ray.GetPoint(distance);
            else
                throw new MyException("cannot calculate zone position");
            
            yield return new WaitForSeconds(_duration);
            DeletingFreezingZone?.Invoke();
            _freezingObject.SetActive(false);
        }

        private static void FreezingEnteredObjects(Collider other)
        {
            IFreezable obj = other.gameObject.GetComponent<IFreezable>();
            if (obj != null)
            {
                obj.Freeze();
                DeletingFreezingZone += obj.Unfreeze;/////////////////
            }
        }
    }
}