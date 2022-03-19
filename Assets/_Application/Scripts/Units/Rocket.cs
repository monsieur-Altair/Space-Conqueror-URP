using _Application.Scripts.Skills;

namespace _Application.Scripts.Units
{
    public class Rocket : Base, IFreezable
    {
        private _Application.Scripts.Planets.Base.UnitInf _unitInf;
        
        protected override void TargetInRange()
        {
            Ice.DeletingFreezingZone -= Unfreeze;
        
            if(Target!=null)
                Target.AttackedByUnit(this);
       
            gameObject.SetActive(false);
            Target = null;
        }

        public void OnDisable() => 
            Ice.DeletingFreezingZone -= Unfreeze;

        public override void SetData(in _Application.Scripts.Planets.Base.UnitInf unitInf) => 
            _unitInf = unitInf;

        public override Planets.Team GetTeam() => 
            _unitInf.UnitTeam;

        protected override void SetSpeed() => 
            Agent.speed = _unitInf.UnitSpeed;

        public override float CalculateAttack(Planets.Team planetTeam, float defence)
        {
            if (_unitInf.UnitTeam == planetTeam)
                return _unitInf.UnitCount;
            return -1.0f * _unitInf.UnitDamage / (100.0f * defence) * _unitInf.UnitCount;
        }

        public override float GetActualCount(float countAfterAttack) => 
            countAfterAttack / (_unitInf.UnitDamage/100.0f);

        public void Freeze() => 
            Agent.isStopped = true;

        public void Unfreeze() => 
            Agent.isStopped = false;
    }
}