using UnityEngine;

namespace Planets
{
    public class Attacker : Base
    {
        private ZoneController _attackZone;
        [SerializeField] private Resources.Unit resourceBullet;
        private UnitInf _bulletInf;

        public override void Start()
        {
            _bulletInf = new UnitInf();
            
            base.Start();
        }

        
        //does not work correctly
        public void StartAttackingUnits(Units.Base unit)
        {
            var tr = unit.transform;

            var radiusCurrent = GetComponent<SphereCollider>().radius;

            var currentPos = transform.position;
            var destinationPos = tr.position;

            var offset = (destinationPos - currentPos).normalized;
            
            
            var bullet = Pool.GetObject(
                    Type.Spawner, 
                currentPos+offset*radiusCurrent, 
                tr.rotation)
                .GetComponent<Units.Base>();
            bullet.SetData(in _bulletInf);
            //obj.GoTo(destinationPos);
            bullet.GoTo(new Vector3(1,0,-3));
        }

        protected override void LoadResources()
        {
            base.LoadResources();

            if (resourceBullet == null)
                throw new MyException("bullet info = null");
            
            _bulletInf.Damage = resourceBullet.damage;
            _bulletInf.Speed = resourceBullet.speed;
            _bulletInf.Team = Team;
            _bulletInf.UnitCount = 1;
            
        }
    }
}