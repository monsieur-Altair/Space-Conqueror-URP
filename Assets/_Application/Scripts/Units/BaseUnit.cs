using System;
using Pool_And_Particles;
using UnityEngine;
using UnityEngine.AI;

namespace _Application.Scripts.Units
{
    [RequireComponent(
        typeof(NavMeshAgent),
        typeof(Collider), 
        typeof(Rigidbody))]
    
    public abstract class BaseUnit : PooledBehaviour
    {
        public static event Action<BaseUnit> Launched = delegate { };
        public static event Action<BaseUnit> Updated = delegate { };
        public static event Action<BaseUnit> Approached = delegate { };
        
        [SerializeField] protected Transform _counterPoint;
        [SerializeField] protected float _radius;
        [SerializeField] SkinnedMeshRenderer _skinnedMeshRenderer;

        public abstract float GetActualCount(float countAfterAttack);
        public abstract void SetData(in Buildings.BaseBuilding.UnitInf unitInf);
        public abstract Buildings.Team GetTeam();
        public abstract float CalculateAttack(Buildings.Team buildingTeam,float defence);

        protected Buildings.BaseBuilding Target;
        protected abstract void TargetInRange();
        protected abstract void SetSpeed();
        protected NavMeshAgent Agent;

        private Vector3 _destination;
        private const float MinDistance = 1.0f;
        public Buildings.BaseBuilding.UnitInf UnitInf { get; protected set; }
        public Transform CounterPoint => _counterPoint;
        public SkinnedMeshRenderer SkinnedMeshRenderer => _skinnedMeshRenderer;


        private void Awake()
        {
            Agent = GetComponent<NavMeshAgent>();
        }

        public void Update()
        {
            if (Target != null)
            {
                OnUpdate();

                float distance = Vector3.Distance(_destination, transform.position);
                if (distance < MinDistance) 
                    StopAndDestroy();
            }
        }

        protected virtual void OnUpdate()
        {
        }

        protected void OnUpdated()
        {
            Updated(this);
        }

        public void GoTo(Buildings.BaseBuilding destination,Vector3 destinationPos)
        {
            Target = destination;
            GoTo(destinationPos);
        }

        public void StopAndDestroy()
        {
            if(Agent.isActiveAndEnabled)
                Agent.isStopped = true;
            TargetInRange();
            Approached(this);
        }

        public void Stop()
        {
            Target = null;
        }

        private void GoTo(Vector3 destinationPos)
        {
            _destination = destinationPos;
            SetSpeed();
            Agent.SetDestination(destinationPos);
            Agent.isStopped = false;
            Launched(this);
        }
    }
}