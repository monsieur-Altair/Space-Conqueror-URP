using System;
using _Application.Scripts.Skills;
using Managers;
using Scriptables;
using UnityEngine;
using Ice = _Application.Scripts.Skills.Ice;

namespace Planets
{
    public enum Team
    {
        Blue=0,
        Red=1,
        White=2,
    }
    public enum Type
    {
        Scientific=0,
        Spawner=1,
        Attacker=2
    }
    [RequireComponent(typeof(Collider))]//,typeof(Rigidbody))]
    public abstract class Base : MonoBehaviour, IFreezable, IBuffable
    {
        [SerializeField] private Team team;
        [SerializeField] private Type type;
        [SerializeField] private Unit resourceUnit;
        [SerializeField] private Planet resourcePlanet;

        private const float LaunchCoefficient = 0.5f;

        private Managers.Main _main;
        private Managers.Outlook _outlook;
        private Managers.UI _ui;
        private Managers.ObjectPool _pool;

        private const float Speed = 20.0f;
        private float _count;
        private UnitInf _unitInf;
        private bool _isFrozen;

        public static event Action<Planets.Base, Team, Team> Conquered;
        private static int _id = 0;
        
        public int ID { get; private set; }
        public float OrbitRadius { get; private set; }
        
        protected float MaxCount { get; private set; }
        protected float ProduceCount { get; private set; }
        protected float ProduceTime { get; private set; }
        protected float Defense { get; private set; }
        protected float ReducingSpeed { get; private set; }
        
        
        public Team Team { get; private set; }
        public Type Type { get; private set; }
        
        public bool IsBuffed { get; private set; }
        //public bool isSelected = false;


        public struct UnitInf
        {
            public float Speed { get; internal set; }
            public float Damage { get; internal set;}
            public float UnitCount { get; internal set; }
            public Team Team { get; internal set; }
        }
        

        public void Init()
        {
            _count = 0.0f;
            ID = _id++;
            
            if (resourceUnit == null) 
                throw new MyException("resource is not loaded: "+name);
            
            _unitInf = new UnitInf();
            _main = Managers.Main.Instance;
            _pool = Managers.ObjectPool.Instance;
            OrbitRadius = GetComponent<SphereCollider>().radius;
            LoadResources();
        }
        public void Update()
        {
            Move();
            if(Team!=Team.White)
                IncreaseResources();
        }

        public void SetUIManager()
        {
            _ui = Managers.UI.Instance;
        }

        public void SetOutlookManager()
        {
            _outlook=Managers.Outlook.Instance;
        }
        
        
        public void LateUpdate()
        {
            if(_ui!=null)
                DisplayUI();
        }


        protected virtual void LoadResources()
        {

            MaxCount = resourcePlanet.maxCount;
            ProduceCount = resourcePlanet.produceCount;
            ProduceTime = resourcePlanet.produceTime;
            Defense = resourcePlanet.defense;
            ReducingSpeed = resourcePlanet.reducingSpeed;
            Team = team;
            Type = type;
            
            _unitInf.Speed = resourceUnit.speed;
            _unitInf.Damage = resourceUnit.damage;
            _unitInf.Team = Team;
            
            if(Team==Team.White)
                _count = MaxCount;
        }

        public void OnDestroy()
        {
            Ice.DeletingFreezingZone -= Unfreeze;
        }
        
        protected virtual void Move()
        {
            if(!_isFrozen)
                transform.Rotate(Vector3.up, Speed*Time.deltaTime,Space.World);
        }

        protected virtual void IncreaseResources()
        {
            if (_isFrozen) 
                return;
            
            if(_count<MaxCount)
                _count += ProduceCount / ProduceTime * Time.deltaTime;
            else if(_count>MaxCount + 0.1f)
            {
                _count -= ReducingSpeed * Time.deltaTime;
            }
        }

        protected virtual void DisplayUI()
        {
            _ui.SetUnitCounter(this,(int)_count);
        }

        public void LaunchUnit(Planets.Base destination)
        {
            CalculateLaunchPositions( out var launchPos,out var destPos,this,destination);
            
            #region Object pooling
            
            ObjectPool.PoolObjectType poolObjectType = (ObjectPool.PoolObjectType)((int) Type);
            
            _Application.Scripts.Units.Base unit = _pool.GetObject(
                poolObjectType,
                launchPos, 
                Quaternion.LookRotation(destPos-launchPos)
            ).GetComponent<_Application.Scripts.Units.Base>();

            #endregion
            
            AdjustUnit(unit);
            LaunchFromCounter();//decrease counter
            unit.GoTo(destination,destPos);
        }

        private static void CalculateLaunchPositions(out Vector3 st, out Vector3 dest, Base stBase, Base destBase)
        {
            //st=start, dest=destination
            Vector3 stPos = stBase.transform.position;
            Vector3 destPos = destBase.transform.position;
            Vector3 offset = (destPos - stPos).normalized;
            st = stPos + offset * stBase.OrbitRadius;
            dest = destPos - offset * destBase.OrbitRadius;
        }

        

        public void DecreaseCounter(float value)
        {
            _count -= value;
            if (_count < 0.0f)
                _count = 0.0f;
        }
        
        public void AdjustUnit(_Application.Scripts.Units.Base unit)
        {
            SetUnitCount();
            _outlook.SetUnitOutlook(this, unit);
            unit.SetData(in _unitInf);
        }


        public void AttackedByUnit(_Application.Scripts.Units.Base unit)
        {
            Team unitTeam = unit.GETTeam();
            float attack=unit.CalculateAttack(Team,Defense);
            _count += attack;
            if (_count < 0)
            {
                Team oldTeam = Team;
                Team newTeam = unitTeam;
                OnConquered(oldTeam,newTeam);
                _count *= -1.0f;
                _count = unit.GetActualCount(_count);
                _main.UpdateObjectsCount(Team,unitTeam);
                SwitchTeam(unitTeam);
                _main.CheckGameOver();
            }
        }

        public void Buff(float percent)
        {
            IsBuffed = true;
            _outlook.SetBuff(this);
            _unitInf.Damage *= (1 + percent / 100.0f);
        }

        public void UnBuff(float percent)
        {
            _unitInf.Damage /= (1 + percent / 100.0f);
            IsBuffed = false;
            _outlook.UnSetBuff(this);
        }

        public void Freeze()
        {
            _isFrozen = true;
        }

        public void Unfreeze()
        {
            _isFrozen = false;
        }

        private void SwitchTeam(Planets.Team newTeam)
        {
            
            team = newTeam;
            //reset resources
            LoadResources();

            _outlook.SetOutlook(this);
            _ui.SetUnitCounterColor(this);
        }

        private void LaunchFromCounter()
        {
            _count *= (1-LaunchCoefficient);
        }
        
        private void SetUnitCount()
        {
            _unitInf.UnitCount = _count*LaunchCoefficient;
        }

        private void OnConquered(Team oldTeam, Team newTeam)
        {
            Conquered?.Invoke(this, oldTeam, newTeam);
        }
    }
}
