using System;
using _Application.Scripts.Infrastructure.Services.Progress;
using _Application.Scripts.Infrastructure.Services.Scriptables;
using _Application.Scripts.SavedData;
using _Application.Scripts.Scriptables;
using _Application.Scripts.Skills;
using DG.Tweening;
using Pool_And_Particles;
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

    public enum BuildingType
    {
        Altar = 0,
        Spawner = 1,
        Attacker = 2,
        None = 3,
    }

    [RequireComponent(typeof(Collider))]//,typeof(Rigidbody))]
    public abstract class BaseBuilding : PooledBehaviour, IFreezable, IBuffed
    {
        [SerializeField] private Team team;
        [SerializeField] private BuildingType _buildingType;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [Space, SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private Collider _collider;

        public static event Action<BaseBuilding, Team, Team> Conquered;
        public event Action<BaseBuilding, int> CountChanged;
        public event Action<BaseBuilding> TeamChanged;
        public event Action<BaseBuilding> Buffed;
        public event Action<BaseBuilding> UnBuffed;
        public event Action<BaseBuilding, Units.BaseUnit> LaunchedUnit;
        
        private static int _id;

        protected ScriptableService ScriptableService;
        protected ProgressService ProgressService;
        
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
        private GlobalPool _globalPool;


        public int ID { get; private set; }
        public float BuildingsRadius { get; private set; }
        public Team Team => team;
        public BuildingType BuildingType => _buildingType;
        public Units.BaseUnit UnitPrefab { get; private set; }
        public bool IsBuffed { get; private set; }
        public float Count => _count;
        public MeshRenderer MeshRenderer => _meshRenderer;


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

        public void Construct(ScriptableService scriptableService, ProgressService progressService, 
            Units.BaseUnit unitPrefab, GlobalPool globalPool)
        {
            _globalPool = globalPool;
            UnitPrefab = unitPrefab;
            ScriptableService = scriptableService;
            ProgressService = progressService;
            
            _count = 0.0f;
            CountChanged?.Invoke(this, (int) _count);
            
            ID = _id++;

            _unitInf = new UnitInf();
            
            BuildingsRadius = GetComponent<SphereCollider>().radius;

            Team availableTeam = (team == Team.White) ? Team.Red : team;

            Building infoAboutBuilding = ScriptableService.GetBuildingInfo(availableTeam, _buildingType);
            Unit infoAboutUnit = ScriptableService.GetUnitInfo(availableTeam, _buildingType);
            
            LoadResources(infoAboutBuilding, infoAboutUnit);
            
            Deselect();
        }

        protected virtual void LoadResources(Building infoAboutBuilding, Unit infoAboutUnit)
        {
            PlayerProgress playerProgress = ProgressService.PlayerProgress;
  
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

        public void Select(Color color)
        {
            _spriteRenderer.color = color;
            _spriteRenderer.gameObject.SetActive(true);
            _spriteRenderer.transform
                .DORotate(new Vector3(0, 360f, 0), 30f, RotateMode.WorldAxisAdd)
                .SetLoops(-1, LoopType.Incremental)
                .SetEase(Ease.Linear);
        }

        public void Deselect()
        {
            _spriteRenderer.transform.DOKill();
            _spriteRenderer.gameObject.SetActive(false);
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

        public void LaunchUnit(BaseBuilding destination)
        {
            CalculateLaunchPositions( out Vector3 launchPos,out Vector3 destPos,this,destination);

            Quaternion rotation = Quaternion.LookRotation(destPos - launchPos);

            Units.BaseUnit unit = _globalPool.Get(UnitPrefab, position: launchPos, rotation: rotation);
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

        private void AdjustUnit(Units.BaseUnit unit)
        {
            SetUnitCount();
            LaunchedUnit?.Invoke(this, unit);
            unit.SetData(in _unitInf);
        }

        public void AdjustUnit(Units.BaseUnit unit, float supplyCoefficient)
        {
            SetUnitCount(supplyCoefficient);
            LaunchedUnit?.Invoke(this, unit);
            unit.SetData(in _unitInf);
        }

        public void AttackedByUnit(Units.BaseUnit unit)
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

        public void DisableCollider()
        {
            _collider.enabled = false;
        }
        
        public override void OnSpawnFromPool()
        {
            base.OnSpawnFromPool();
            
            _spriteRenderer.gameObject.SetActive(false);
            _collider.enabled = true;
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

        private static void CalculateLaunchPositions(out Vector3 st, out Vector3 dest, BaseBuilding stBaseBuilding, BaseBuilding destBaseBuilding)
        {
            Vector3 stPos = stBaseBuilding.transform.position;
            Vector3 destPos = destBaseBuilding.transform.position;
            Vector3 offset = (destPos - stPos).normalized;
            st = stPos + offset * stBaseBuilding.BuildingsRadius;
            dest = destPos - offset * destBaseBuilding.BuildingsRadius;
        }

        private void SwitchTeam(Team newTeam)
        {
            team = newTeam;

            Building infoAboutBuilding = ScriptableService.GetBuildingInfo(newTeam , _buildingType);
            Unit infoAboutUnit = ScriptableService.GetUnitInfo(newTeam, _buildingType);

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
