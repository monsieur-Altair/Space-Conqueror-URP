using System.Collections;
using _Application.Scripts.Buildings;
using _Application.Scripts.Scriptables;
using Pool_And_Particles;
using UnityEngine;

namespace _Application.Scripts.Skills
{
    public class Buff : Base
    {
        private float _duration;
        private float _buffPercent;

        private IBuffed _buffedEntity;
        private Coroutine _buffCoroutine;

        public Buff(GlobalPool globalPool, Team teamConstraint, DecreasingCounter function) 
            : base(globalPool, teamConstraint, function)
        {
        }

        protected override void LoadResources(Skill resource, float coefficient = 1.0f)
        {
            base.LoadResources(resource, coefficient);
            Scriptables.Buff res = resource as Scriptables.Buff;
            if (res != null)
            {
                _duration = res.duration * coefficient;
                _buffPercent = res.buffPercent * coefficient;
            }
        }

        // public override void SetSkillObject(GameObject skillObject)
        // {
        // }

        protected override void ApplySkill()
        {
            if (!IsForAI)
            {
                SelectedBuilding = RaycastForBuilding();
                _buffedEntity = SelectedBuilding;
            }

            if (_buffedEntity != null && SelectedBuilding.Team == TeamConstraint)
                ApplySkillToBuilding(BuffBuilding);
            else
                OnCanceledSkill();
        }

        protected override void CancelSkill()
        {
            CoroutineRunner.StopCoroutine(_buffCoroutine);
            
            if (_buffedEntity.IsBuffed)
                _buffedEntity.UnBuff(_buffPercent);
            _buffedEntity = null;
            SelectedBuilding = null;
            
            IsOnCooldown = false;
            OnCanceledSkill();
        }

        private void BuffBuilding() => 
            _buffCoroutine = CoroutineRunner.StartCoroutine(BuffThenUnBuff());

        private IEnumerator BuffThenUnBuff()
        {
            _buffedEntity.Buff(_buffPercent);
            yield return new WaitForSeconds(_duration);
            _buffedEntity.UnBuff(_buffPercent);
        }
    }
}