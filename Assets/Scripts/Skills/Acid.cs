using System.Collections;
using System.IO;
using Planets;
using UnityEngine;
using UnityEngine.VFX;

namespace Skills
{
    public class Acid : Base
    {

        [SerializeField] private GameObject acidPrefab;
        private GameObject _acidRain;
        private ParticleSystem _acidParticles;
        
        private readonly Vector3 _offset = new Vector3(1, 3, 0);
        public float HitDuration { get; private set; }
        public float Duration { get; private set; }
        public float HitDamage { get; private set; }
        public int HitCount { get; private set; }
        protected override void LoadResources()
        {
            base.LoadResources();
            var res = resource as Resources.Acid;
            if (res != null)
            {
                Duration =res.duration;
                HitCount = res.hitCount;
                HitDuration = Duration / HitCount;
                HitDamage = res.damage / HitCount;
            }

            _acidRain = Instantiate(acidPrefab);
            if (_acidRain == null)
                throw new MyException("cannot instantiate acid prefab");
            _acidParticles = _acidRain.transform.GetChild(0).GetComponent<ParticleSystem>();
            if (_acidParticles == null)
                throw new MyException("cannot get particle system");
        }
        
        protected override void ApplySkill()
        {
            //Debug.Log("acid");
            //if(SelectedPlanet==null)
            if(!IsForAI)
                SelectedPlanet = RaycastForPlanet();
            
            //Debug.Log("Selected team="+SelectedPlanet.Team);
            if (SelectedPlanet!=null && SelectedPlanet.Team != teamConstraint) 
                ApplySkillToPlanet(StartRain);
            else
            {
               UnblockButton();
            }
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
            while (count != HitCount)
            {
                SelectedPlanet.DecreaseCounter(HitDamage);
                yield return new WaitForSeconds(HitDuration);
                count++;
            }
            _acidParticles.Stop();
            SelectedPlanet = null;
        }

        protected override void CancelSkill()
        {
            IsOnCooldown = false;
            
            UnblockButton();
        }
    }
}