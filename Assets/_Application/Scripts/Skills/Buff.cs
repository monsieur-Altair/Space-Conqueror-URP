namespace _Application.Scripts.Skills
{
    public class Buff : Base
    {
        private float _buffPercent;
        private IBuffed _buffed;
        
        protected override void LoadResources()
        {
            base.LoadResources();
            Scriptables.Buff res = resource as Scriptables.Buff;
            if(res!=null)
                _buffPercent = res.buffPercent;
        }

        protected override void ApplySkill()
        {
            if (!IsForAI)
            {
                SelectedPlanet = RaycastForPlanet();
                _buffed = SelectedPlanet;
            }

            if (_buffed!=null && SelectedPlanet.Team == TeamConstraint)
                ApplySkillToPlanet(BuffPlanet);
            else
                OnCanceledSkill();
        }

        protected override void CancelSkill()
        {
            IsOnCooldown = false;
            _buffed.UnBuff(_buffPercent);
            _buffed = null;
            SelectedPlanet = null;
            OnCanceledSkill();
        }

        private void BuffPlanet() => 
            _buffed.Buff(_buffPercent);
    }
}