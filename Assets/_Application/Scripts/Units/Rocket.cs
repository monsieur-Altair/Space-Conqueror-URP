
using _Application.Scripts.Planets;
using _Application.Scripts.Skills;
using Base = _Application.Scripts.Units.Base;

namespace Units
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

        public void OnDisable()
        {
            Ice.DeletingFreezingZone -= Unfreeze;
        }

        public override void SetData(in _Application.Scripts.Planets.Base.UnitInf unitInf)
        {
            _unitInf = unitInf;
        }

        public override Team GetTeam() => _unitInf.UnitTeam;

        protected override void SetSpeed()
        {
            Agent.speed = _unitInf.UnitSpeed;
        }

        public override float CalculateAttack(Team planetTeam, float defence)
        {
            //damage in percent
            if (_unitInf.UnitTeam == planetTeam)
                return _unitInf.UnitCount;
            return -1.0f*_unitInf.UnitDamage / (100.0f * defence) * _unitInf.UnitCount;
        }

        public override float GetActualCount(float countAfterAttack)
        {
            return countAfterAttack / (_unitInf.UnitDamage/100.0f);
        }

        public void Freeze()
        {
            Agent.isStopped = true;
        }

        public void Unfreeze()
        { 
            Agent.isStopped = false;
        }
        
        
    }
}