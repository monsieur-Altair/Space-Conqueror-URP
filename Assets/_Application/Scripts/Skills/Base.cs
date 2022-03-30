using System;
using _Application.Scripts.Infrastructure;
using _Application.Scripts.Managers;
using JetBrains.Annotations;
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
        protected Planets.Base SelectedPlanet;
        
        protected float Cooldown;
        protected bool IsOnCooldown;
        protected bool IsForAI;
        protected Planets.Team TeamConstraint;
        protected ICoroutineRunner CoroutineRunner;
        protected delegate void UniqueActionToPlanet();
        
        private DecreasingCounter _decreaseCounter;

        public int Cost { get; private set; }
        protected Vector3 SelectedScreenPos { get; private set; }

        public Base(Planets.Team? teamConstraint, [CanBeNull] DecreasingCounter function)
        {
            MainCamera=Camera.main;
            CoroutineRunner = GlobalObject.Instance;
            _decreaseCounter = function;
            SetTeamConstraint(teamConstraint);
        }

        public void Reload(Scriptables.Skill resource, float coefficient = 1.0f)
        {
            LoadResources(resource, coefficient);
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
            if (Planets.Scientific.ScientificCount > Cost && !IsOnCooldown)
                ApplySkill();
            else
                OnCanceledSkill();
        }

        public virtual void SetSkillObject(GameObject skillObject)
        {
            
        }

        public void SetTeamConstraint(Planets.Team? team)
        {
            if (team != null)
            {
                IsForAI = (team == Planets.Team.Red);
                TeamConstraint = (Planets.Team) team; 
            }
        }

        public void ExecuteForAI(Planets.Base planet)
        {
            SelectedPlanet = planet;
            if (AI.Core.ScientificCount > Cost && !IsOnCooldown) 
                ApplySkill();
        }

        protected virtual void LoadResources(Scriptables.Skill resource,
            float coefficient = 1.0f)
        {
            float decreasingCoefficient = (2.0f - coefficient);
            Cooldown = resource.cooldown * decreasingCoefficient;
            Cost = (int)(resource.cost * decreasingCoefficient);
        }

        protected Planets.Base RaycastForPlanet()
        {
            int layerMask = 1 << 0;
            layerMask = ~layerMask;
            Ray ray = MainCamera.ScreenPointToRay(SelectedScreenPos);
            return Physics.Raycast(ray, out var hit,Mathf.Infinity, layerMask) ? hit.collider.GetComponent<Planets.Base>() : null;
        }
        
        protected void ApplySkillToPlanet(UniqueActionToPlanet action)
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