using System.Collections;
using _Application.Scripts.Planets;
using _Application.Scripts.Scriptables;
using UnityEngine;

namespace _Application.Scripts.Skills
{
    public class Acid : Base
    {
        private readonly Vector3 _offset = new Vector3(0.8f, 3, 0);
        private readonly Quaternion _rotation = Quaternion.Euler(-90f,0,0);
        
        private GameObject _acidRain;
        private Coroutine _damagingByAcid;
        private ParticleSystem _acidParticles;
        private float _hitDuration;
        private float _duration;
        private float _hitDamage;
        private int _hitCount;

        public Acid(Team teamConstraint, DecreasingCounter function) : base(teamConstraint, function)
        {
        }

        public override void SetSkillObject(GameObject skillObject)
        {
            _acidRain = skillObject;
            _acidParticles = _acidRain.transform.GetChild(0).GetComponent<ParticleSystem>();
        }

        protected override void LoadResources(Skill resource, float coefficient = 1.0f)
        {
            base.LoadResources(resource, coefficient);
            Scriptables.Acid res = resource as Scriptables.Acid;
            if (res != null)
            {
                float decreasingCoefficient = 2.0f - coefficient; 
                _duration = res.duration * decreasingCoefficient;
                _hitCount = res.hitCount;
                _hitDuration = _duration / _hitCount;
                _hitDamage = res.damage * coefficient / _hitCount;
            }
        }

        protected override void CancelSkill()
        {
            if(_acidParticles.isPlaying)
                _acidParticles.Stop();
            CoroutineRunner.StopCoroutine(_damagingByAcid);
            SelectedPlanet = null;
            IsOnCooldown = false;
            OnCanceledSkill();
        }

        protected override void ApplySkill()
        {
            if(!IsForAI)
                SelectedPlanet = RaycastForPlanet();
            
            if (SelectedPlanet!=null && SelectedPlanet.Team != TeamConstraint) 
                ApplySkillToPlanet(StartRain);
            else
                OnCanceledSkill();
        }

        private void StartRain()
        {
            _acidRain.transform.position = SelectedPlanet.transform.position +_offset;
            _acidRain.transform.rotation = _rotation;
            _acidParticles.Play();
            _damagingByAcid = CoroutineRunner.StartCoroutine(DamagePlanetByRain());
        }

        private IEnumerator DamagePlanetByRain()
        {
            var count = 0;
            while (count != _hitCount)
            {
                SelectedPlanet.DecreaseCounter(_hitDamage);
                yield return new WaitForSeconds(_hitDuration);
                count++;
            }
            _acidParticles.Stop();
            SelectedPlanet = null;
        }
    }
}