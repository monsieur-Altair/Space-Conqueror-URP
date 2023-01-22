using _Application.Scripts.Skills;
using UnityEngine;

namespace _Application.Scripts.Units
{
    public class Warrior : BaseUnit, IFreezable
    {
        public SkinnedMeshRenderer skinnedMeshRenderer;
        
        private Buildings.BaseBuilding.UnitInf _unitInf;

        protected override void TargetInRange()
        {
            Ice.DeletingFreezingZone -= Unfreeze;
        
            if(Target!=null)
                Target.AttackedByUnit(this);
       
            Target = null;
        }

        public void OnDisable() => 
            Ice.DeletingFreezingZone -= Unfreeze;

        public override void SetData(in Buildings.BaseBuilding.UnitInf unitInf) => 
            _unitInf = unitInf;

        public override Buildings.Team GetTeam() => 
            _unitInf.UnitTeam;

        protected override void SetSpeed() => 
            Agent.speed = _unitInf.UnitSpeed;

        public override float CalculateAttack(Buildings.Team buildingTeam, float defence)
        {
            if (_unitInf.UnitTeam == buildingTeam)
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