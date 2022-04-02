using System.Collections;
using _Application.Scripts.Infrastructure.Services.Factory;
using _Application.Scripts.Managers;
using _Application.Scripts.Misc;
using _Application.Scripts.Scriptables;
using UnityEngine;

namespace _Application.Scripts.Skills
{
    public class Call : Base
    {
        private GameObject _indicator;
        private readonly Vector3 _indicatorOffset = new Vector3(0, 1.9f, 0);
        private Coroutine _displayingIndicatorCor;
        private Units.Base _sentUnit;
        private float _callPercent;

        public Call(Team teamConstraint, DecreasingCounter function) : base(teamConstraint, function)
        {
        }

        public override void SetSkillObject(GameObject skillObject)
        {
            _indicator = skillObject;
            _indicator.SetActive(false);
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
            _indicator.SetActive(false);
            SelectedBuilding = null;
            IsOnCooldown = false;
            OnCanceledSkill();
        }

        private void CallSupply()
        {
            Vector3 launchPos = CameraResolution.FindSpawnPoint(SelectedBuilding);
            Vector3 destPos = CalculateDestPos(launchPos, SelectedBuilding);
            
            PoolObjectType poolObjectType = (PoolObjectType) ((int) SelectedBuilding.Type);
            
            _sentUnit = OnNeedObjectFromPool(poolObjectType,launchPos, Quaternion.LookRotation(destPos - launchPos));
            
            SelectedBuilding.AdjustUnit(_sentUnit, _callPercent / 100.0f);
            _sentUnit.GoTo(SelectedBuilding, destPos);

            _displayingIndicatorCor = CoroutineRunner.StartCoroutine(DisplayIndicator());
        }

        private IEnumerator DisplayIndicator()
        {
            _indicator.SetActive(true);
            _indicator.transform.position = SelectedBuilding.transform.position + _indicatorOffset;
            yield return new WaitForSeconds(1.5f);
            _indicator.SetActive(false);
        }

        private static Vector3 CalculateDestPos(in Vector3 launchPos, Buildings.Base destinationPlanet)
        {
            Vector3 destPos = destinationPlanet.transform.position;
            Vector3 offset = (destPos - launchPos).normalized;
            return destPos - offset * destinationPlanet.BuildingsRadius;
        }
    }
}