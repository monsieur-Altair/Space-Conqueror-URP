using System.Collections;
using _Application.Scripts.Infrastructure.Factory;
using _Application.Scripts.Infrastructure.Services;
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

        protected override void LoadResources(IGameFactory gameFactory, Skill resource)
        {
            base.LoadResources(gameFactory, resource);
            Scriptables.Call res = resource as Scriptables.Call;
            if (res != null)
                _callPercent = res.callPercent;
            
            _indicator = gameFactory.CreateIndicator();
            
            _indicator.SetActive(false);
        }
      
        protected override void ApplySkill()
        {
            if(!IsForAI)
                SelectedPlanet = RaycastForPlanet();
            
            if (SelectedPlanet!=null && SelectedPlanet.Team == TeamConstraint)
                ApplySkillToPlanet(CallSupply);
            else
                OnCanceledSkill();
        }

        protected override void CancelSkill()
        {
            CoroutineRunner.StopCoroutine(_displayingIndicatorCor);
            if (_sentUnit != null)
                _sentUnit.StopAndDestroy();
            _indicator.SetActive(false);
            SelectedPlanet = null;
            IsOnCooldown = false;
            OnCanceledSkill();
        }

        private void CallSupply()
        {
            Vector3 launchPos = CameraResolution.FindSpawnPoint(SelectedPlanet);
            Vector3 destPos = CalculateDestPos(launchPos, SelectedPlanet);
            
            ObjectPool.PoolObjectType poolObjectType = (ObjectPool.PoolObjectType) ((int) SelectedPlanet.Type);
            
            _sentUnit = ObjectPool.GetObject(poolObjectType,
                    launchPos,
                    Quaternion.LookRotation(destPos - launchPos))
                .GetComponent<Units.Base>();
            
            SelectedPlanet.AdjustUnit(_sentUnit, _callPercent / 100.0f);
            _sentUnit.GoTo(SelectedPlanet, destPos);

            _displayingIndicatorCor = CoroutineRunner.StartCoroutine(DisplayIndicator());
        }

        private IEnumerator DisplayIndicator()
        {
            _indicator.SetActive(true);
            _indicator.transform.position = SelectedPlanet.transform.position + _indicatorOffset;
            yield return new WaitForSeconds(1.5f);
            _indicator.SetActive(false);
        }
        
        private static Vector3 CalculateDestPos(in Vector3 launchPos, Planets.Base destinationPlanet)
        {
            Vector3 destPos = destinationPlanet.transform.position;
            Vector3 offset = (destPos - launchPos).normalized;
            return destPos - offset * destinationPlanet.OrbitRadius;
        }
    }
}