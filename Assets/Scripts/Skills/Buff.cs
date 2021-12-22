
using Planets;
using UnityEngine;

namespace Skills
{
    public class Buff : Base
    {
        public float BuffPercent { get; private set; }

        protected override void LoadResources()
        {
            base.LoadResources();
            var res = resource as Resources.Buff;
            if(res!=null)
                BuffPercent = res.buffPercent;
        }

        protected override void ApplySkill()
        {
            SelectedPlanet = RaycastForPlanet();
            if(SelectedPlanet.Team==Team.Blue)
                ApplySkillToPlanet(BuffPlanet);
        }

        private void BuffPlanet()
        {
            SelectedPlanet.Buff(BuffPercent);
        }
        
        protected override void CancelSkill()
        {
            IsOnCooldown = false;
            SelectedPlanet.UnBuff(BuffPercent);
            UnblockButton();
        }
    }
}