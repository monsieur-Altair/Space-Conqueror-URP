using _Application.Scripts.Scriptables;
using UnityEngine;

namespace _Application.Scripts.Buildings
{
    public class Attacker : Base
    {
        // [SerializeField] private Unit resourceBullet;
        // private UnitInf _bulletInf;

        // public void Start() => 
        //     _bulletInf = new UnitInf();

        // protected override void LoadResources(Building infoAboutBuilding, Unit infoAboutUnit)
        // {
        //     base.LoadResources(infoAboutBuilding, infoAboutUnit);
        //
        //     // if (resourceBullet == null)
        //     //     throw new MyException("bullet info = null");
        //     //
        //     // _bulletInf.UnitDamage = resourceBullet.damage;
        //     // _bulletInf.UnitSpeed = resourceBullet.speed;
        //     // _bulletInf.UnitTeam = Team;
        //     // _bulletInf.UnitCount = 1;
        //     
        // }
        
        // //does not work correctly
        // public void StartAttackingUnits(Units.Base unit)
        // {
        //     var tr = unit.transform;
        //
        //     var radiusCurrent = GetComponent<SphereCollider>().radius;
        //
        //     var currentPos = transform.position;
        //     var destinationPos = tr.position;
        //
        //     var offset = (destinationPos - currentPos).normalized;
        //     
        //     
        //     var bullet = Pool.GetObject(
        //             Type.Spawner, 
        //         currentPos+offset*radiusCurrent, 
        //     //obj.GoTo(destinationPos);
        //     bullet.GoTo(new Vector3(1,0,-3));
        // }
    }
}