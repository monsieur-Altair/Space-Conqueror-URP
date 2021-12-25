
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
            //Debug.Log("buff");
            
            //if (SelectedPlanet==null)
            if(!IsForAI)
                SelectedPlanet = RaycastForPlanet();
//            Debug.Log("Selected team="+SelectedPlanet.Team);
            if(SelectedPlanet.Team==teamConstraint)
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
            SelectedPlanet = null;
            UnblockButton();
        }
    }
}