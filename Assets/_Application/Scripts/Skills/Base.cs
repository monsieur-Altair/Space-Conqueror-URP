using System;
using Scriptables;
using UnityEngine;

namespace _Application.Scripts.Skills
{
    public abstract class Base : MonoBehaviour, ISkill
    {
        [SerializeField] 
        protected Skill resource;

        public event Action CanceledSkill;
        public delegate void DecreasingCounter(float count);
        protected abstract void CancelSkill();
        protected abstract void ApplySkill();

        protected Camera MainCamera;
        protected Managers.ObjectPool ObjectPool;
        protected Planets.Base SelectedPlanet;
        protected float Cooldown;
        protected int Cost;
        protected bool IsOnCooldown;
        protected bool IsForAI;
        protected Planets.Team TeamConstraint;
        protected delegate void UniqueActionToPlanet();
        
        
        private DecreasingCounter _decreaseCounter;


        protected Vector3 SelectedScreenPos { get; private set; }

        public void Awake()
        {
            MainCamera=Camera.main;
            ObjectPool = Managers.ObjectPool.Instance;
        }

        public void Start() => 
            LoadResources();

        public void SetDecreasingFunction(DecreasingCounter function) => 
            _decreaseCounter = function;

        public void ExecuteForPlayer(Vector3 pos)
        {
            SelectedScreenPos = pos;
            if (Planets.Scientific.ScientificCount > Cost && !IsOnCooldown)
                ApplySkill();
            else
                OnCanceledSkill();
        }

        public void SetTeamConstraint(Planets.Team team)
        {
            IsForAI = (team == Planets.Team.Red);
            TeamConstraint = team;
        }

        public void ExecuteForAI(Planets.Base planet)
        {
            SelectedPlanet = planet;
            if (AI.Core.ScientificCount > Cost && !IsOnCooldown) 
                ApplySkill();
        }

        protected virtual void LoadResources()
        {
            Cooldown = resource.cooldown;
            Cost = resource.cost;
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
            Invoke(nameof(CancelSkill), Cooldown);
        }
        
        protected void OnCanceledSkill()
        {
            if(!IsOnCooldown)
                CanceledSkill?.Invoke();
        }
    }
}