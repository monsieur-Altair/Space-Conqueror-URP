using System;
using _Application.Scripts.Buildings;
using _Application.Scripts.Infrastructure;
using _Application.Scripts.Infrastructure.Services.Factory;
using _Application.Scripts.Managers;
using UnityEngine;

namespace _Application.Scripts.Skills
{
    public abstract class Base : ISkill
    {
        public event Action CanceledSkill;
        public event Func<PoolObjectType, Vector3, Quaternion, Units.Base> NeedObjectFromPool;
        public delegate void DecreasingCounter(float count);
        
        protected abstract void CancelSkill();
        protected abstract void ApplySkill();
        
        protected Camera MainCamera;
        protected Buildings.Base SelectedBuilding;
        
        protected float Cooldown;
        protected bool IsOnCooldown;
        protected bool IsForAI;
        protected Team TeamConstraint;
        protected ICoroutineRunner CoroutineRunner;
        protected delegate void UniqueActionToBuilding();
        
        private DecreasingCounter _decreaseCounter;

        public int Cost { get; private set; }
        protected Vector3 SelectedScreenPos { get; private set; }

        public void Construct(IGameFactory gameFactory, Scriptables.Skill resource)
        {
            MainCamera = Camera.main;
            CoroutineRunner = GlobalObject.Instance;
            
            LoadResources(gameFactory, resource);
        }

        public void Refresh()
        {
            if (IsOnCooldown)
            {
                CoroutineRunner.CancelAllInvoked();
                CancelSkill();
            }
        }
        
        public void SetDecreasingFunction(DecreasingCounter function) => 
            _decreaseCounter = function;

        public void ExecuteForPlayer(Vector3 pos)
        {
            SelectedScreenPos = pos;
            if (Altar.ManaCount > Cost && !IsOnCooldown)
                ApplySkill();
            else
                OnCanceledSkill();
        }

        public void SetTeamConstraint(Team team)
        {
            IsForAI = (team == Team.Red);
            TeamConstraint = team;
        }

        public void ExecuteForAI(Buildings.Base planet)
        {
            SelectedBuilding = planet;
            if (AI.Core.ManaCount > Cost && !IsOnCooldown) 
                ApplySkill();
        }

        protected virtual void LoadResources(IGameFactory gameFactory, Scriptables.Skill resource)
        {
            Cooldown = resource.cooldown;
            Cost = resource.cost;
        }

        protected Buildings.Base RaycastForBuilding()
        {
            int layerMask = 1 << 0;
            layerMask = ~layerMask;
            Ray ray = MainCamera.ScreenPointToRay(SelectedScreenPos);
            return Physics.Raycast(ray, out var hit,Mathf.Infinity, layerMask) ? hit.collider.GetComponent<Buildings.Base>() : null;
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
                CanceledSkill?.Invoke();
        }

        protected Units.Base OnNeedObjectFromPool(PoolObjectType type, Vector3 pos, Quaternion rotation) => 
            NeedObjectFromPool?.Invoke(type, pos, rotation);
    }
}