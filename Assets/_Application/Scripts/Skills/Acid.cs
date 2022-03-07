using System.Collections;
using UnityEngine;

namespace _Application.Scripts.Skills
{
    public class Acid : Base
    {
        [SerializeField] 
        private GameObject acidPrefab;
        
        private readonly Vector3 _offset = new Vector3(1, 3, 0);
        private GameObject _acidRain;
        private ParticleSystem _acidParticles;
        private float _hitDuration;
        private float _duration;
        private float _hitDamage;
        private int _hitCount;
        
        protected override void LoadResources()
        {
            base.LoadResources();
            Scriptables.Acid res = resource as Scriptables.Acid;
            if (res != null)
            {
                _duration =res.duration;
                _hitCount = res.hitCount;
                _hitDuration = _duration / _hitCount;
                _hitDamage = res.damage / _hitCount;
            }
            
            _acidRain = Instantiate(acidPrefab);
            if (_acidRain == null)
                throw new MyException("cannot instantiate acid prefab");
            _acidParticles = _acidRain.transform.GetChild(0).GetComponent<ParticleSystem>();
            if (_acidParticles == null)
                throw new MyException("cannot get particle system");
        }

        protected override void CancelSkill()
        {
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
            _acidRain.transform.rotation = Quaternion.identity;
            _acidParticles.Play();                                                                              
            StartCoroutine(nameof(DamagePlanetByRain));
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