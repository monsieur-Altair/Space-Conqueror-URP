﻿using System;
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

        public void OnDisable()
        {
            Skills.Ice.DeletingFreezingZone -= Unfreeze;
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

        public override float CalculateAttack(Planets.Team planetTeam, float defence)
        {
            //damage in percent
            if (_unitInf.Team == planetTeam)
                return _unitInf.UnitCount;
            return -1.0f*_unitInf.Damage / (100.0f * defence) * _unitInf.UnitCount;
        }

        public override float GetActualCount(float countAfterAttack)
        {
            return countAfterAttack / (_unitInf.Damage/100.0f);
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