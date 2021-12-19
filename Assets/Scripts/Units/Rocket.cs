using System.Linq.Expressions;
using Planets;
using UnityEngine;

namespace Units
{
    public class Rocket : Base, Skills.IFreezable
    {
        private Planets.Base.UnitInf _unitInf;
        

        protected override void TargetInRange()
        {
            Skills.Ice.DeletingFreezingZone -= Unfreeze;
            //Debug.Log("arrived");
            if(Target!=null)
                Target.AttackedByUnit(this);
            //Destroy(gameObject);
            gameObject.SetActive(false);
            Target = null;
        }
        

        public override void SetData(in Planets.Base.UnitInf unitInf)
        {
            _unitInf = unitInf;
        }

        public override Team GETTeam() => _unitInf.Team;

        protected override void SetSpeed()
        {
            Agent.speed = _unitInf.Speed;
        }

        public override float CalculateAttack()
        {
            //damage in percent
            return _unitInf.Damage / 100.0f* _unitInf.UnitCount;
        }
        
        public void Freeze()
        {
            Agent.isStopped = true;
            //Agent.speed = 0.0f;
        }

        public void Unfreeze()
        {
            Agent.isStopped = false;
            //SetSpeed();
        }
        
        
    }
}