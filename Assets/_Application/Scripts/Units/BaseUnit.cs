using System;
using Pool_And_Particles;
using UnityEngine;
using UnityEngine.AI;

namespace _Application.Scripts.Units
{
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

        private Vector3 _destination;
        private Transform _transform;
        
        protected bool _canMove = true;
        protected float _speed = 1.0f;

        private const float MinDistance = 0.1f;
        public Buildings.BaseBuilding.UnitInf UnitInf { get; protected set; }
        public Transform CounterPoint => _counterPoint;
        public SkinnedMeshRenderer SkinnedMeshRenderer => _skinnedMeshRenderer;

        private void Awake()
        {
            _transform = transform;
        }

        public void Update()
        {
            if (Target != null && _canMove)
            {
                OnUpdate();

                _transform.position = MoveTowards();
                
                float distance = Vector3.Distance(_destination, _transform.position);
                if (distance < MinDistance) 
                    StopAndDestroy();
            }
        }

        private Vector3 MoveTowards()
        {
            return Vector3.MoveTowards(_transform.position, Target.transform.position, 
                _speed * Time.deltaTime);
        }

        protected virtual void OnUpdate()
        {
        }

        protected void OnUpdated()
        {
            Updated(this);
        }

        public override void OnReturnToPool()
        {
            base.OnReturnToPool();
            
            _canMove = true;
        }
        
        public void GoTo(Buildings.BaseBuilding destination,Vector3 destinationPos)
        {
            Target = destination;
            GoTo(destinationPos);
        }

        public void StopAndDestroy()
        {
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
            Launched(this);
        }
    }
}