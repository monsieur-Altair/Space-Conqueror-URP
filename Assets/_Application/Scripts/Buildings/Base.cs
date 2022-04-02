using System;
using _Application.Scripts.Infrastructure.Services.Progress;
using _Application.Scripts.Infrastructure.Services.Scriptables;
using _Application.Scripts.Managers;
using _Application.Scripts.SavedData;
using _Application.Scripts.Skills;
using UnityEngine;
using Ice = _Application.Scripts.Skills.Ice;

namespace _Application.Scripts.Buildings
{
    public enum Team
    {
        Blue = 0,
        Red = 1,
        White = 2,
    }

    public enum Type
    {
        Altar = 0,
        Spawner = 1,
        Attacker = 2
    }

    [RequireComponent(typeof(Collider))]//,typeof(Rigidbody))]
    public abstract class Base : MonoBehaviour, IFreezable, IBuffed
    {
        [SerializeField] private Team team;
        [SerializeField] private Type type;

        public static event Action<Base, Team, Team> Conquered;
        public event Action<Base, int> CountChanged;
        public event Action<Base> TeamChanged;
        public event Action<Base> Buffed;
        public event Action<Base> UnBuffed;
        public event Action<Base, Units.Base> LaunchedUnit;
        public event Func<PoolObjectType, Vector3, Quaternion, Units.Base> LaunchingUnit;
        
        private static int _id;

        protected IScriptableService ScriptableService;
        protected IProgressService ProgressService;
        
        //private const float Speed = 20.0f;
        private const float LaunchCoefficient = 0.5f;

        private UnitInf _unitInf;
        private bool _isFrozen;
        
        private float _count;
        private float _maxCount;
        private float _produceCount;
        private float _produceTime;
        private float _defense;
        private float _reducingSpeed;


        public int ID { get; private set; }
        public float BuildingsRadius { get; private set; }
        public Team Team { get; private set; }
        public Type Type { get; private set; }
        public bool IsBuffed { get; private set; }
        public float Count => _count;


        public struct UnitInf
        {
            public float UnitSpeed { get; internal set; }
            public float UnitDamage { get; internal set;}
            public float UnitCount { get; internal set; }
            public Team UnitTeam { get; internal set; }
        }

        public void Update()
        {
            if (Team != Team.White)
                IncreaseResources();
        }

        public void OnDestroy() => 
            Ice.DeletingFreezingZone -= Unfreeze;

        public void Construct(IScriptableService scriptableService, IProgressService progressService)
        {
            ScriptableService = scriptableService;
            ProgressService = progressService;
            
            _count = 0.0f;
            CountChanged?.Invoke(this, (int) _count);
            
            ID = _id++;

            _unitInf = new UnitInf();
            
            BuildingsRadius = GetComponent<SphereCollider>().radius;

            Team availableTeam = (team == Team.White) ? Team.Red : team;

            Scriptables.Building infoAboutBuilding = ScriptableService.GetBuildingInfo(availableTeam, type);
            Scriptables.Unit infoAboutUnit = ScriptableService.GetUnitInfo(availableTeam, type);
            
            LoadResources(infoAboutBuilding, infoAboutUnit);
        }

        protected virtual void LoadResources(Scriptables.Building infoAboutBuilding, Scriptables.Unit infoAboutUnit)
        {
            PlayerProgress playerProgress = ProgressService.PlayerProgress;
            
            Team = team;
            Type = type;
            
            //bad practice
            float buildingDefenceCoefficient = (team == Team.Blue)
                ? playerProgress.GetAchievedUpgrade(UpgradeType.BuildingDefence).upgradeCoefficient
                : 1.0f;

            float buildingMaxCountCoefficient = (team == Team.Blue)
                ? ProgressService.PlayerProgress.GetAchievedUpgrade(UpgradeType.BuildingMaxCount).upgradeCoefficient
                : 1.0f;
            //
            
            _maxCount = infoAboutBuilding.maxCount * buildingMaxCountCoefficient;
            _produceCount = infoAboutBuilding.produceCount;
            _produceTime = infoAboutBuilding.produceTime;
            _defense = infoAboutBuilding.defense * buildingDefenceCoefficient;
            _reducingSpeed = infoAboutBuilding.reducingSpeed;
            
            
            
            //bad practice
            float unitSpeedCoefficient = (team == Team.Blue)
                ? playerProgress.GetAchievedUpgrade(UpgradeType.UnitSpeed).upgradeCoefficient
                : 1.0f;

            float unitAttackCoefficient = (team == Team.Blue)
                ? ProgressService.PlayerProgress.GetAchievedUpgrade(UpgradeType.UnitAttack).upgradeCoefficient
                : 1.0f;
            //
            
            _unitInf.UnitSpeed = infoAboutUnit.speed * unitSpeedCoefficient;
            _unitInf.UnitDamage = infoAboutUnit.damage * unitAttackCoefficient;
            _unitInf.UnitTeam = Team;

            if (Team == Team.White)
            {
                _count = _maxCount;
                CountChanged?.Invoke(this, (int) _count);
            }
        }

        public void Buff(float percent)
        {
            IsBuffed = true;
            Buffed?.Invoke(this);
            _unitInf.UnitDamage *= (1 + percent / 100.0f);
        }

        public void UnBuff(float percent)
        {
            _unitInf.UnitDamage /= (1 + percent / 100.0f);
            IsBuffed = false;
            UnBuffed?.Invoke(this);
        }

        public void Freeze() => 
            _isFrozen = true;

        public void Unfreeze() => 
            _isFrozen = false;

        public void LaunchUnit(Base destination)
        {
            CalculateLaunchPositions( out Vector3 launchPos,out Vector3 destPos,this,destination);

            PoolObjectType poolObjectType = (PoolObjectType)((int) Type);
            Quaternion rotation = Quaternion.LookRotation(destPos - launchPos);

            Units.Base unit = LaunchingUnit?.Invoke(poolObjectType, launchPos, rotation);
            if(unit==null)
                return;

            AdjustUnit(unit);
            LaunchFromCounter();
            
            unit.GoTo(destination, destPos);
        }


        public void DecreaseCounter(float value)
        {
            _count -= value;
            if (_count < 0.0f)
                _count = 0.0f;
            CountChanged?.Invoke(this ,(int)_count);
        }

        private void AdjustUnit(Units.Base unit)
        {
            SetUnitCount();
            LaunchedUnit?.Invoke(this, unit);
            unit.SetData(in _unitInf);
        }

        public void AdjustUnit(Units.Base unit, float supplyCoefficient)
        {
            SetUnitCount(supplyCoefficient);
            LaunchedUnit?.Invoke(this, unit);
            unit.SetData(in _unitInf);
        }

        public void AttackedByUnit(Units.Base unit)
        {
            Team unitTeam = unit.GetTeam();
            float attack = unit.CalculateAttack(Team, _defense);
            _count += attack;
            if (_count < 0)
            {
                Team oldTeam = Team;
                Conquered?.Invoke(this, oldTeam, unitTeam);
                _count *= -1.0f;
                _count = unit.GetActualCount(_count);
                SwitchTeam(unitTeam);
            }
            CountChanged?.Invoke(this, (int)_count);
        }

        protected virtual void IncreaseResources()
        {
            if (_isFrozen) 
                return;
            
            if(_count<_maxCount)
                _count += _produceCount / _produceTime * Time.deltaTime;
            else if(_count>_maxCount + 0.1f) 
                _count -= _reducingSpeed * Time.deltaTime;
            
            CountChanged?.Invoke(this, (int) _count);
        }

        private static void CalculateLaunchPositions(out Vector3 st, out Vector3 dest, Base stBase, Base destBase)
        {
            Vector3 stPos = stBase.transform.position;
            Vector3 destPos = destBase.transform.position;
            Vector3 offset = (destPos - stPos).normalized;
            st = stPos + offset * stBase.BuildingsRadius;
            dest = destPos - offset * destBase.BuildingsRadius;
        }

        private void SwitchTeam(Team newTeam)
        {
            team = newTeam;

            Scriptables.Building infoAboutBuilding = ScriptableService.GetBuildingInfo(newTeam , type);
            Scriptables.Unit infoAboutUnit = ScriptableService.GetUnitInfo(newTeam, type);

            LoadResources(infoAboutBuilding, infoAboutUnit);

            TeamChanged?.Invoke(this);
        }

        private void LaunchFromCounter()
        {
            _count *= (1 - LaunchCoefficient);
            CountChanged?.Invoke(this, (int) _count);
        }

        private void SetUnitCount() =>
            _unitInf.UnitCount = _count * LaunchCoefficient;

        private void SetUnitCount(float supplyCoefficient) =>
            _unitInf.UnitCount = _maxCount * supplyCoefficient;
    }
}
