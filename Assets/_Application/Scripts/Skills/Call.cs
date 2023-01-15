using System.Collections;
using _Application.Scripts.Buildings;
using _Application.Scripts.Infrastructure.Services;
using _Application.Scripts.Managers;
using _Application.Scripts.Scriptables;
using Pool_And_Particles;
using Unity.Mathematics;
using UnityEngine;

namespace _Application.Scripts.Skills
{
    public class Call : Base
    {
        private readonly GlobalCamera _globalCamera;
        private PooledBehaviour _indicator;
        private readonly Vector3 _indicatorOffset = new Vector3(0, 1.9f, 0);
        private Coroutine _displayingIndicatorCor;
        private Units.Base _sentUnit;
        private float _callPercent;
        private bool _isIndicatorActive;

        public Call( GlobalPool pool, Team teamConstraint, DecreasingCounter function, GlobalCamera globalCamera) 
            : base(pool, teamConstraint, function)
        {
            _globalCamera = globalCamera;
        }

        protected override void LoadResources(Skill resource, float coefficient = 1.0f)
        {
            base.LoadResources(resource, coefficient);
            Scriptables.Call res = resource as Scriptables.Call;
            if (res != null)
                _callPercent = res.callPercent * coefficient;
        }

        protected override void ApplySkill()
        {
            if(!IsForAI)
                SelectedBuilding = RaycastForBuilding();
            
            if (SelectedBuilding!=null && SelectedBuilding.Team == TeamConstraint)
                ApplySkillToBuilding(CallSupply);
            else
                OnCanceledSkill();
        }

        protected override void CancelSkill()
        {
            CoroutineRunner.StopCoroutine(_displayingIndicatorCor);
            if (_sentUnit != null)
                _sentUnit.StopAndDestroy();
            
            FreeIndicator();
            
            SelectedBuilding = null;
            IsOnCooldown = false;
            OnCanceledSkill();
        }

        private void FreeIndicator()
        {
            if (_isIndicatorActive)
                _globalPool.Free(_indicator);
        }

        private void CallSupply()
        {
            Vector3 launchPos = _globalCamera.FindSpawnPoint(SelectedBuilding);
            Vector3 destPos = CalculateDestPos(launchPos, SelectedBuilding);
            
            _sentUnit = _globalPool.Get(SelectedBuilding.UnitPrefab, launchPos, Quaternion.LookRotation(destPos - launchPos));

            SelectedBuilding.AdjustUnit(_sentUnit, _callPercent / 100.0f);
            _sentUnit.GoTo(SelectedBuilding, destPos);

            _displayingIndicatorCor = CoroutineRunner.StartCoroutine(DisplayIndicator());
        }
        

        private IEnumerator DisplayIndicator()
        {
            _indicator = _globalPool.GetObject(PoolObjectType.Indicator, 
                pos: SelectedBuilding.transform.position + _indicatorOffset, 
                rot: Quaternion.identity);
            _isIndicatorActive = true;
            yield return new WaitForSeconds(1.5f);
            FreeIndicator();
        }

        private static Vector3 CalculateDestPos(in Vector3 launchPos, Buildings.Base destinationPlanet)
        {
            Vector3 destPos = destinationPlanet.transform.position;
            Vector3 offset = (destPos - launchPos).normalized;
            return destPos - offset * destinationPlanet.BuildingsRadius;
        }
    }
}