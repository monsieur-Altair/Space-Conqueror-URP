using System.Collections;
using System.IO;
using Planets;
using UnityEngine;
using UnityEngine.VFX;

namespace Skills
{
    public class Acid : Base
    {
        [SerializeField] private VisualEffect vfxPrefab;
        private VisualEffect _acidRainEffect;
        private readonly Vector3 _offset = new Vector3(0, 1, 0);
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

            _acidRainEffect = Instantiate(vfxPrefab);
            _acidRainEffect.Stop();
        }
        
        protected override void ApplySkill()
        {
            SelectedPlanet = RaycastForPlanet();
            if(SelectedPlanet.Team!=Team.Blue)
                ApplySkillToPlanet(StartRain);
        }

        private void StartRain()
        {
            _acidRainEffect.transform.position = SelectedPlanet.transform.position +_offset;
            _acidRainEffect.Play();                                                                              
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
            _acidRainEffect.Stop();
        }

        protected override void CancelSkill()
        {
            IsOnCooldown = false;
            UnblockButton();
        }
    }
}