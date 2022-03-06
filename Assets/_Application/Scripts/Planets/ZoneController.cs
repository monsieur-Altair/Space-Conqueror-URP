using UnityEngine;

namespace Planets
{
    [DefaultExecutionOrder(200)]
    public class ZoneController : MonoBehaviour
    {
        private SphereCollider _attackZone;
        private Planets.Team _team;
        private Managers.ObjectPool _pool;
        private Planets.Attacker _planet;
        public void Start()
        {
            _attackZone = GetComponent<SphereCollider>();
            if (_attackZone == null)
                throw new MyException("cannot get attack zone collider");
            _planet= GetComponentInParent<Planets.Attacker>();
            _team =_planet.Team;
            _pool = Managers.ObjectPool.Instance;
        }

        /*private void OnTriggerEnter(Collider other)
        {
            var unit = other.GetComponentInParent<Units.Base>();
            if (unit == null)
                throw new MyException("not attacked by a unit");
            
            if (_team != unit.getTeam())
                _planet.StartAttackingUnits(unit);
        }*/
    }
}