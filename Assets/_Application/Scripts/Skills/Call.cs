using System.Collections;
using _Application.Scripts.Managers;
using _Application.Scripts.Misc;
using UnityEngine;

namespace _Application.Scripts.Skills
{
    public class Call : Base
    {
        [SerializeField] 
        private GameObject indicatorPrefab;
        
        private GameObject _indicator;
        private readonly Vector3 _indicatorOffset = new Vector3(0, 1.9f, 0);
        
        private float _callPercent;

        protected override void LoadResources()
        {
            base.LoadResources();
            Scriptables.Call res = resource as Scriptables.Call;
            if (res != null)
                _callPercent = res.callPercent;
            
            _indicator = Instantiate(indicatorPrefab );
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
            SelectedPlanet = null;
            IsOnCooldown = false;
            OnCanceledSkill();
        }

        private void CallSupply()
        {
            Vector3 launchPos = CameraResolution.FindSpawnPoint(SelectedPlanet);
            Vector3 destPos = CalculateDestPos(launchPos, SelectedPlanet);
            ObjectPool.PoolObjectType poolObjectType = (ObjectPool.PoolObjectType) ((int) SelectedPlanet.Type);
            Units.Base unit = ObjectPool.GetObject(poolObjectType,
                    launchPos,
                    Quaternion.LookRotation(destPos - launchPos))
                .GetComponent<_Application.Scripts.Units.Base>();
            SelectedPlanet.AdjustUnit(unit);
            unit.GoTo(SelectedPlanet, destPos);

            StartCoroutine(DisplayIndicator());
        }

        private IEnumerator DisplayIndicator()
        {
            _indicator.SetActive(true);
            _indicator.transform.position = SelectedPlanet.transform.position + _indicatorOffset;
            yield return new WaitForSeconds(1.5f);
            _indicator.SetActive(false);
        }
        
        private Vector3 CalculateDestPos(in Vector3 launchPos, Planets.Base destinationPlanet)
        {
            Vector3 destPos = destinationPlanet.transform.position;
            Vector3 offset = (destPos - launchPos).normalized;
            return destPos - offset * destinationPlanet.OrbitRadius;
        }
    }
}