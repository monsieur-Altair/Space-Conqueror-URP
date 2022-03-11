using _Application.Scripts.Planets;
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
        protected NavMeshAgent Agent;

        private Vector3 _destination;
        protected Planets.Base Target;
        

        protected abstract void TargetInRange();
        public abstract void SetData(in Planets.Base.UnitInf unitInf);

        public abstract Planets.Team GetTeam();
        protected abstract void SetSpeed();
        public abstract float CalculateAttack(Team planetTeam,float defence);
        
        private const float MinDistance = 1.0f;

        public void Update()
        {
            if (Target != null)
            {
               
                float distance = Vector3.Distance(_destination, transform.position);
           
                if (distance < MinDistance)
                {
                    StopAndDestroy();
                }
            }
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

            //didn't work if i get component in start
            Agent = GetComponent<NavMeshAgent>();
            if (Agent == null)
                throw new MyException("can't get NavMeshAgent component");
            
            SetSpeed();
            Agent.SetDestination(destinationPos);
            Agent.isStopped = false;
        }
        
        public void GoTo(Planets.Base destination,Vector3 destinationPos)
        {
            Target = destination;
            GoTo(destinationPos);
        }

        public abstract float GetActualCount(float countAfterAttack);

    }
}