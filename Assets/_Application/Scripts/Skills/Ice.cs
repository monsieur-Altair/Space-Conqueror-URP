using System;
using System.Collections;
using _Application.Scripts.Buildings;
using _Application.Scripts.Control;
using _Application.Scripts.Managers;
using _Application.Scripts.Scriptables;
using Pool_And_Particles;
using UnityEngine;

namespace _Application.Scripts.Skills
{
    public class Ice : BaseSkill
    {
        public static event Action DeletingFreezingZone;

        private const float BuildingLayHeight = 0.66f;
        private float _duration;

        private readonly GameObject _freezingObject;
        private readonly Zone _freezingZone;
        private Plane _plane;
        private Coroutine _spawnCoroutine;

        public Ice(GlobalPool pool) : base(pool, null, null)
        {
            _freezingObject = pool.GetObject(PoolObjectType.Ice, Vector3.zero, Quaternion.identity).gameObject;
             
            _freezingZone = _freezingObject.GetComponent<Zone>(); 
            _freezingZone.SetTriggerFunction(FreezingEnteredObjects);
            
            _freezingObject.SetActive(false);
            _plane = new Plane(Vector3.up, new Vector3(0, BuildingLayHeight, 0));
        }
        
        
        protected override void LoadResources(Skill resource, float coefficient = 1.0f)
        {
            base.LoadResources(resource, coefficient);
            Scriptables.Ice res = resource as Scriptables.Ice;
            if (res != null) 
                _duration = res.duration * coefficient;
        }

        protected override void CancelSkill()
        {
            //wasn't tested well
            CoroutineRunner.CancelAllInvoked();
            CoroutineRunner.StopCoroutine(_spawnCoroutine);
            DeletingFreezingZone?.Invoke();
            _freezingObject.SetActive(false);
            
            IsOnCooldown = false;
            OnCanceledSkill();
        }

        protected override void ApplySkill()
        {
            IsOnCooldown = true;
            _spawnCoroutine = CoroutineRunner.StartCoroutine(SpawnFreezingZone(SelectedScreenPos));
            Altar.DecreaseManaCount(Cost);
            CoroutineRunner.InvokeWithDelay(CancelSkill,Cooldown);
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