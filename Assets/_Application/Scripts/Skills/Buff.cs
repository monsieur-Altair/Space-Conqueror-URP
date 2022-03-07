namespace _Application.Scripts.Skills
{
    public class Buff : Base
    {
        private float _buffPercent;

        protected override void LoadResources()
        {
            base.LoadResources();
            var res = resource as Scriptables.Buff;
            if(res!=null)
                _buffPercent = res.buffPercent;
        }

        protected override void ApplySkill()
        {
            if(!IsForAI)
                SelectedPlanet = RaycastForPlanet();

            if (SelectedPlanet!=null && SelectedPlanet.Team == TeamConstraint)
                ApplySkillToPlanet(BuffPlanet);
            else
                OnCanceledSkill();
        }

        private void BuffPlanet()
        {
            SelectedPlanet.Buff(_buffPercent);
        }
        
        protected override void CancelSkill()
        {
            IsOnCooldown = false;
            SelectedPlanet.UnBuff(_buffPercent);
            SelectedPlanet = null;
            OnCanceledSkill();
        }
    }
}