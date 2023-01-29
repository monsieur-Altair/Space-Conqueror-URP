using System;
using _Application.Scripts.Infrastructure.Services.Progress;
using _Application.Scripts.Infrastructure.Services.Scriptables;
using _Application.Scripts.SavedData;
using _Application.Scripts.Scriptables;
using _Application.Scripts.Skills;
using _Application.Scripts.Units;
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

    public abstract class BaseBuilding : PooledBehaviour, IFreezable, IBuffed
    {
        [SerializeField] private Team team;
        [SerializeField] private BuildingType _buildingType;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [Space, SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private SphereCollider _sphereCollider;
        [SerializeField] private Transform _counterPoint;

        public static event Action<BaseBuilding, Team, Team> Conquered;
        public event Action<BaseBuilding, int> CountChanged;
        public event Action<BaseBuilding> TeamChanged;
        public event Action<BaseBuilding> Buffed;
        public event Action<BaseBuilding> UnBuffed;
        public event Action<BaseBuilding, BaseUnit> LaunchedUnit;
        
        protected ScriptableService ScriptableService;
        protected ProgressService ProgressService;
        
        //private const float Speed = 20.0f;
        private const float LaunchCoefficient = 0.5f;

        private UnitInf _unitInf;
        private bool _isFrozen;

        private float _produceCount;
        private float _produceTime;
        private float _defense;
        private float _reducingSpeed;
        private GlobalPool _globalPool;

        public float BuildingsRadius { get; private set; }
        public Team Team => team;
        public BuildingType BuildingType => _buildingType;
        public BaseUnit UnitPrefab { get; private set; }
        public bool IsBuffed { get; private set; }
        public float Count { get; private set; }
        public float MaxCount { get; private set; }
        public MeshRenderer MeshRenderer => _meshRenderer;
        public Transform CounterPoint => _counterPoint;


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
            BaseUnit unitPrefab, GlobalPool globalPool)
        {
            _globalPool = globalPool;
            UnitPrefab = unitPrefab;
            ScriptableService = scriptableService;
            ProgressService = progressService;
            
            Count = 0.0f;
            CountChanged?.Invoke(this, (int) Count);
            
            _unitInf = new UnitInf();
            
            BuildingsRadius = _sphereCollider.radius;

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
            
            MaxCount = infoAboutBuilding.maxCount * buildingMaxCountCoefficient;
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
                Count = MaxCount;
                CountChanged?.Invoke(this, (int) Count);
            }
        }

        public void SetCount(float count)
        {
            Count = count;
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

            BaseUnit unit = _globalPool.Get(UnitPrefab, position: launchPos, rotation: rotation);
            
            AdjustUnit(unit);
            LaunchFromCounter();
            
            unit.GoTo(destination, destPos);
        }

        public void DecreaseCounter(float value)
        {
            Count -= value;
            if (Count < 0.0f)
                Count = 0.0f;
            CountChanged?.Invoke(this ,(int)Count);
        }

        private void AdjustUnit(BaseUnit unit)
        {
            SetUnitCount();
            unit.SetData(in _unitInf);
            LaunchedUnit?.Invoke(this, unit);
        }

        public void AdjustUnit(BaseUnit unit, float supplyCoefficient)
        {
            SetUnitCount(supplyCoefficient);
            unit.SetData(in _unitInf);
            LaunchedUnit?.Invoke(this, unit);
        }

        public void AttackedByUnit(BaseUnit unit)
        {
            Team unitTeam = unit.GetTeam();
            float attack = unit.CalculateAttack(Team, _defense);
            Count += attack;
            if (Count < 0)
            {
                Team oldTeam = Team;
                Conquered?.Invoke(this, oldTeam, unitTeam);
                Count *= -1.0f;
                Count = unit.GetActualCount(Count);
                SwitchTeam(unitTeam);
            }
            CountChanged?.Invoke(this, (int)Count);
        }
        
        public void DisableCollider()
        {
            _sphereCollider.enabled = false;
        }
        
        public override void OnSpawnFromPool()
        {
            base.OnSpawnFromPool();
            
            _spriteRenderer.gameObject.SetActive(false);
            _sphereCollider.enabled = true;
        }

        protected virtual void IncreaseResources()
        {
            if (_isFrozen) 
                return;
            
            if(Count<MaxCount)
                Count += _produceCount / _produceTime * Time.deltaTime;
            else if(Count>MaxCount + 0.1f) 
                Count -= _reducingSpeed * Time.deltaTime;
            
            CountChanged?.Invoke(this, (int) Count);
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

        public void SetTeam(Team newTeam)
        {
            team = newTeam;
        }

        private void LaunchFromCounter()
        {
            Count *= (1 - LaunchCoefficient);
            CountChanged?.Invoke(this, (int) Count);
        }

        private void SetUnitCount() =>
            _unitInf.UnitCount = Count * LaunchCoefficient;

        private void SetUnitCount(float supplyCoefficient) =>
            _unitInf.UnitCount = MaxCount * supplyCoefficient;
    }
}
