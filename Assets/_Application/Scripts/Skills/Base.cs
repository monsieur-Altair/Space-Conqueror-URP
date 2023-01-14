using System;
using _Application.Scripts.Infrastructure.Services;
using _Application.Scripts.Managers;
using JetBrains.Annotations;
using UnityEngine;

namespace _Application.Scripts.Skills
{
    public abstract class Base : ISkill
    {
        public event Action CanceledSkill = delegate {  };
        public static event Action<int> SkillIsAppliedForPlayer = delegate {  };
        public event Func<PoolObjectType, Vector3, Quaternion, Units.Base> NeedObjectFromPool = delegate { return null; };
        
        public delegate void DecreasingCounter(float count);
        
        protected abstract void CancelSkill();
        protected abstract void ApplySkill();

        protected readonly Camera MainCamera;
        protected Buildings.Base SelectedBuilding;

        protected float Cooldown;
        protected bool IsOnCooldown;
        protected bool IsForAI;
        protected Buildings.Team TeamConstraint;
        protected readonly CoroutineRunner CoroutineRunner;


        protected delegate void UniqueActionToBuilding();

        private readonly DecreasingCounter _decreaseCounter;

        public int Cost { get; private set; }
        protected Vector3 SelectedScreenPos { get; private set; }


        protected Base(Buildings.Team? teamConstraint, [CanBeNull] DecreasingCounter function)
        {
            MainCamera = AllServices.Get<GlobalCamera>().MainCamera;
            CoroutineRunner = AllServices.Get<CoroutineRunner>();
            _decreaseCounter = function;
            SetTeamConstraint(teamConstraint);
        }

        public void Reload(Scriptables.Skill resource, float coefficient = 1.0f) => 
            LoadResources(resource, coefficient);

        protected virtual void LoadResources(Scriptables.Skill resource, float coefficient = 1.0f)
        {
            float decreasingCoefficient = (2.0f - coefficient);
            Cooldown = resource.cooldown * decreasingCoefficient;
            Cost = (int)(resource.cost * decreasingCoefficient);
        }

        public void Refresh()
        {
            if (IsOnCooldown)
            {
                CoroutineRunner.CancelAllInvoked();
                CancelSkill();
            }
        }

        public void ExecuteForPlayer(Vector3 pos)
        {
            SelectedScreenPos = pos;
            if (Buildings.Altar.ManaCount > Cost && !IsOnCooldown)
            {
                ApplySkill();
                SkillIsAppliedForPlayer(Cost);
            }
            else
                OnCanceledSkill();
        }

        public void ExecuteForAI(Buildings.Base planet)
        {
            SelectedBuilding = planet;
            if (AI.Core.ManaCount > Cost && !IsOnCooldown) 
                ApplySkill();
        }

        protected Buildings.Base RaycastForBuilding()
        {
            int layerMask = 1 << 0;
            layerMask = ~layerMask;
            Ray ray = MainCamera.ScreenPointToRay(SelectedScreenPos);
            return Physics.Raycast(ray, out RaycastHit hit,Mathf.Infinity, layerMask) 
                ? hit.collider.GetComponent<Buildings.Base>() 
                : null;
        }

        protected void ApplySkillToBuilding(UniqueActionToBuilding action)
        {
            IsOnCooldown = true;
            _decreaseCounter(Cost);
            action();
            CoroutineRunner.InvokeWithDelay(CancelSkill,Cooldown);
        }

        protected void OnCanceledSkill()
        {
            if(!IsOnCooldown)
                CanceledSkill();
        }

        protected Units.Base OnNeedObjectFromPool(PoolObjectType type, Vector3 pos, Quaternion rotation) => 
            NeedObjectFromPool(type, pos, rotation);

        private void SetTeamConstraint(Buildings.Team? team)
        {
            if (team != null)
            {
                IsForAI = (team == Buildings.Team.Red);
                TeamConstraint = (Buildings.Team) team;
            }
        }
    }
}