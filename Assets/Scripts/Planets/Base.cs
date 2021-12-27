using System;
using UnityEngine; 

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
    public abstract class Base : MonoBehaviour, Skills.IFreezable, Skills.IBuffable
    {
        [SerializeField] private Team team;
        [SerializeField] private Type type;
        [SerializeField] private Resources.Unit resourceUnit;
        [SerializeField] private Resources.Planet resourcePlanet;

        private const float LaunchCoefficient = 0.5f;
        
        protected Managers.Main Main;
        private Managers.Outlook _outlook;
        protected Managers.UI UI;
        protected Managers.ObjectPool Pool;
        
        private float speed = 20.0f;
        private float _count;
        private UnitInf _unitInf;
        private bool _isFrozen=false;

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
            Debug.Log("init planet");
            _count = 0.0f;
            ID = _id++;
            
            if (resourceUnit == null) 
                throw new MyException("resource is not loaded: "+name);
            
            _unitInf = new UnitInf();
            Main = Managers.Main.Instance;
            Pool = Managers.ObjectPool.Instance;
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
            UI = Managers.UI.Instance;
        }

        public void SetOutlookManager()
        {
            _outlook=Managers.Outlook.Instance;
        }
        
        
        public void LateUpdate()
        {
            if(UI!=null)
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
            Debug.Log("DESTROYED");
            Skills.Ice.DeletingFreezingZone -= Unfreeze;
        }
        
        protected virtual void Move()
        {
            if(!_isFrozen)
                transform.Rotate(Vector3.up, speed*Time.deltaTime,Space.World);
        }

        protected virtual void IncreaseResources()
        {
            if (!_isFrozen)
            {
                if(_count<MaxCount)
                    _count += ProduceCount / ProduceTime * Time.deltaTime;
                else if(_count>MaxCount + 0.1f)
                {
                    _count -= ReducingSpeed * Time.deltaTime;
                }
                /*if (_count > MaxCount) 
                    _count = MaxCount;*/
            }
        }

        protected virtual void DisplayUI()
        {
            UI.SetUnitCounter(this,(int)_count);
        }

        public void LaunchUnit(Planets.Base destination)
        {
            CalculateLaunchPositions( out var launchPos,out var destPos,this,destination);
            
            #region Object pooling

            var unit = Pool.GetObject(
                Type,
                launchPos, 
                Quaternion.LookRotation(destPos-launchPos)
            ).GetComponent<Units.Base>();

            #endregion
            
            AdjustUnit(unit);
            LaunchFromCounter();//decrease counter
            unit.GoTo(destination,destPos);
        }

        private static void CalculateLaunchPositions(out Vector3 st, out Vector3 dest, Base stBase, Base destBase)
        {
            //st=start, dest=destination
            var stPos = stBase.transform.position;
            var destPos = destBase.transform.position;
            var offset = (destPos - stPos).normalized;
            st = stPos + offset * stBase.OrbitRadius;
            dest = destPos - offset * destBase.OrbitRadius;
        }

        private void LaunchFromCounter()
        {
            _count *= (1-LaunchCoefficient);
        }

        public void DecreaseCounter(float value)
        {
            _count -= value;
            if (_count < 0.0f)
                _count = 0.0f;
        }
        
        public void AdjustUnit(Units.Base unit)
        {
            SetUnitCount();
            _outlook.SetUnitOutlook(this, unit);
            unit.SetData(in _unitInf);
        }
        
        private void SetUnitCount()
        {
            _unitInf.UnitCount = _count*LaunchCoefficient;
        }


        public void AttackedByUnit(Units.Base unit)
        {
            var unitTeam = unit.GETTeam();
            var attack=unit.CalculateAttack(Team,Defense);
            _count += attack;
            if (_count < 0)
            {
                var oldTeam = Team;
                var newTeam = unitTeam;
                OnConquered(oldTeam,newTeam);
                _count *= -1.0f;
                _count = unit.GetActualCount(_count);
                Main.UpdateObjectsCount(Team,unitTeam);
                SwitchTeam(unitTeam);
                Main.CheckGameOver();
                //IsBuffed = false;
            }
        }

        private void SwitchTeam(Planets.Team newTeam)
        {
            
            team = newTeam;
            //reset resources
            LoadResources();

            _outlook.SetOutlook(this);
            UI.SetUnitCounterColor(this);
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

        protected void OnConquered(Team oldTeam, Team newTeam)
        {
            Conquered?.Invoke(this, oldTeam, newTeam);
        }
    }
}
