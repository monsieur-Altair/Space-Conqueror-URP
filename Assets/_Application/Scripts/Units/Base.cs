﻿using _Application.Scripts.Planets;
using UnityEngine;
using UnityEngine.AI;

namespace _Application.Scripts.Units
{
    [RequireComponent(
        typeof(NavMeshAgent),
        typeof(Collider), 
        typeof(Rigidbody))]
    
    public abstract class Base : MonoBehaviour
    {
        public abstract float GetActualCount(float countAfterAttack);
        public abstract void SetData(in Planets.Base.UnitInf unitInf);
        public abstract Planets.Team GetTeam();
        public abstract float CalculateAttack(Team planetTeam,float defence);

        protected Planets.Base Target;
        protected abstract void TargetInRange();
        protected abstract void SetSpeed();
        protected NavMeshAgent Agent;

        private Vector3 _destination;
        private const float MinDistance = 1.0f;


        public void Update()
        {
            if (Target != null)
            {
                float distance = Vector3.Distance(_destination, transform.position);
                if (distance < MinDistance) 
                    StopAndDestroy();
            }
        }

        public void GoTo(Planets.Base destination,Vector3 destinationPos)
        {
            Target = destination;
            GoTo(destinationPos);
        }

        public void StopAndDestroy()
        {
            if(Agent.isActiveAndEnabled)
                Agent.isStopped = true;
            TargetInRange();
        }

        private void GoTo(Vector3 destinationPos)
        {
            _destination = destinationPos;
            Agent = GetComponent<NavMeshAgent>();
            
            SetSpeed();
            Agent.SetDestination(destinationPos);
            Agent.isStopped = false;
        }
    }
}