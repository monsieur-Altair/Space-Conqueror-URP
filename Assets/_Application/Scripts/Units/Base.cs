﻿using System;
using Pool_And_Particles;
using UnityEngine;
using UnityEngine.AI;

namespace _Application.Scripts.Units
{
    [RequireComponent(
        typeof(NavMeshAgent),
        typeof(Collider), 
        typeof(Rigidbody))]
    
    public abstract class Base : PooledBehaviour
    {
        public static event Action<Base> Launched = delegate { };
        public static event Action<Base> Approached = delegate { };
        
        public abstract float GetActualCount(float countAfterAttack);
        public abstract void SetData(in Buildings.Base.UnitInf unitInf);
        public abstract Buildings.Team GetTeam();
        public abstract float CalculateAttack(Buildings.Team buildingTeam,float defence);

        protected Buildings.Base Target;
        protected abstract void TargetInRange();
        protected abstract void SetSpeed();
        protected NavMeshAgent Agent;

        private Vector3 _destination;
        private const float MinDistance = 1.0f;

        private void Awake()
        {
            Agent = GetComponent<NavMeshAgent>();
        }

        public void Update()
        {
            if (Target != null)
            {
                float distance = Vector3.Distance(_destination, transform.position);
                if (distance < MinDistance) 
                    StopAndDestroy();
            }
        }

        public void GoTo(Buildings.Base destination,Vector3 destinationPos)
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