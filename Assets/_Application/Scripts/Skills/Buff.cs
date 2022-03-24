using System.Collections;
using _Application.Scripts.Infrastructure.Services.Factory;
using _Application.Scripts.Scriptables;
using UnityEngine;

namespace _Application.Scripts.Skills
{
    public class Buff : Base
    {
        private float _duration;
        private float _buffPercent;
        private IBuffed _buffedEntity;
        private Coroutine _buffCoroutine;

        protected override void LoadResources(IGameFactory gameFactory, Skill resource)
        {
            base.LoadResources(gameFactory, resource);
            Scriptables.Buff res = resource as Scriptables.Buff;
            if (res != null)
            {
                _duration = res.duration;
                _buffPercent = res.buffPercent;
            }
        }

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