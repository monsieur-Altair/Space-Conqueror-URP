using _Application.Scripts.Skills;
using UnityEngine;

namespace _Application.Scripts.Units
{
    public class Warrior : BaseUnit, IFreezable
    {
        protected override void TargetInRange()
        {
            Ice.DeletingFreezingZone -= Unfreeze;
        
            if(Target!=null)
                Target.AttackedByUnit(this);
       
            Stop();
        }

        protected override void SetSpeed()
        {
            _speed = UnitInf.UnitSpeed;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            _counterPoint.position = transform.position + Vector3.back * _radius;
            
            OnUpdated();
        }

        public void OnDisable() => 
            Ice.DeletingFreezingZone -= Unfreeze;

        public override void SetData(in Buildings.BaseBuilding.UnitInf unitInf) => 
            UnitInf = unitInf;

        public override Buildings.Team GetTeam() => 
            UnitInf.UnitTeam;
        
        public override float CalculateAttack(Buildings.Team buildingTeam, float defence)
        {
            if (UnitInf.UnitTeam == buildingTeam)
                return UnitInf.UnitCount;
            return -1.0f * UnitInf.UnitDamage / (100.0f * defence) * UnitInf.UnitCount;
        }

        public override float GetActualCount(float countAfterAttack) => 
            countAfterAttack / (UnitInf.UnitDamage/100.0f);

        public void Freeze() => 
            _canMove = false;

        public void Unfreeze() => 
            _canMove = true;
    }
}