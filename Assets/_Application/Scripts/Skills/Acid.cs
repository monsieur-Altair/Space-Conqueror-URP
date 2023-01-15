using System.Collections;
using _Application.Scripts.Buildings;
using _Application.Scripts.Managers;
using _Application.Scripts.Scriptables;
using Pool_And_Particles;
using UnityEngine;

namespace _Application.Scripts.Skills
{
    public class Acid : Base
    {
        private readonly Vector3 _offset = new Vector3(0.8f, 3, 0);
        private readonly Quaternion _rotation = Quaternion.Euler(-90f,0,0);

        private readonly ParticleSystem _rainParticles;
        private readonly GameObject _rain;
        
        private Coroutine _damagingByAcid;
        private float _hitDuration;
        private float _duration;
        private float _hitDamage;
        private int _hitCount;

        public Acid(GlobalPool pool ,Team teamConstraint, DecreasingCounter function) 
            : base(pool, teamConstraint, function)
        {
            _rain = pool.GetObject(PoolObjectType.Rain, Vector3.zero, Quaternion.identity).gameObject;
            _rainParticles = _rain.transform.GetChild(0).GetComponent<ParticleSystem>();
        }

        protected override void LoadResources(Skill resource, float coefficient = 1.0f)
        {
            base.LoadResources(resource, coefficient);
            Scriptables.Acid res = resource as Scriptables.Acid;
            if (res != null)
            {
               // float decreasingCoefficient = 2.0f - coefficient; 
                _duration = res.duration;//*decreasingCoefficient
                _hitCount = res.hitCount;
                _hitDuration = _duration / _hitCount;
                _hitDamage = res.damage * coefficient / _hitCount;
            }
        }

        protected override void CancelSkill()
        {
            if(_rainParticles.isPlaying)
                _rainParticles.Stop();
            CoroutineRunner.StopCoroutine(_damagingByAcid);
            SelectedBuilding = null;
            IsOnCooldown = false;
            OnCanceledSkill();
        }

        protected override void ApplySkill()
        {
            if(!IsForAI)
                SelectedBuilding = RaycastForBuilding();
            
            if (SelectedBuilding!=null && SelectedBuilding.Team != TeamConstraint) 
                ApplySkillToBuilding(StartRain);
            else
                OnCanceledSkill();
        }

        private void StartRain()
        {
            _rain.SetActive(true);
            _rain.transform.position = SelectedBuilding.transform.position +_offset;
            _rain.transform.rotation = _rotation;
            _rainParticles.Play();
            _damagingByAcid = CoroutineRunner.StartCoroutine(DamageBuildingByRain());
        }

        private IEnumerator DamageBuildingByRain()
        {
            var count = 0;
            while (count != _hitCount)
            {
                SelectedBuilding.DecreaseCounter(_hitDamage);
                yield return new WaitForSeconds(_hitDuration);
                count++;
            }
            _rainParticles.Stop();
            SelectedBuilding = null;
        }
    }
}